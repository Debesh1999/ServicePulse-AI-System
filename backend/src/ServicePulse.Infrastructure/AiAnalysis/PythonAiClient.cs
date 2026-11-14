using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ServicePulse.Application.Services;

namespace ServicePulse.Infrastructure.AiAnalysis;

public sealed class PythonAiClientOptions
{
    public const string Key = "AiService";
    public string BaseUrl { get; set; } = "http://localhost:8000";
}

public sealed class PythonAiClient : IAiAnalyzerService
{
    private readonly HttpClient _http;
    private readonly ILogger<PythonAiClient> _logger;

    public PythonAiClient(HttpClient http, ILogger<PythonAiClient> logger)
    {
        _http = http;
        _logger = logger;
    }

    public async Task<AiAnalysisResult> AnalyzeAsync(string transcriptText, string language = "en")
    {
        var payload = new { text = transcriptText, language };
        var response = await _http.PostAsJsonAsync("/analyze", payload);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            _logger.LogWarning("AI analysis call failed with status {StatusCode}: {Body}", response.StatusCode, errorContent);
            return FailedResult(Guid.NewGuid(), $"Analysis service unavailable ({response.StatusCode})", response.StatusCode.ToString());
        }

        try
        {
            var result = await response.Content.ReadFromJsonAsync<AnalyzeResponse>();
            if (result == null)
                throw new InvalidOperationException("Empty AI response body");

            _logger.LogInformation(
                "AI analysis completed: sentiment={Sentiment}, risk={Risk}, score={Score}, provider={Provider}",
                result.Sentiment, result.RiskLevel, result.QualityScore, result.Provider);

            return new AiAnalysisResult(
                Guid.NewGuid(),
                result.Sentiment ?? "Neutral",
                result.RiskLevel ?? "Medium",
                (decimal)(result.QualityScore ?? 50),
                result.Summary ?? string.Empty,
                result.Keywords ?? [],
                result.AnalyzerVersion ?? "unknown",
                result.AnalysisStatus ?? "Succeeded",
                result.ErrorMessage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to parse AI response");
            return FailedResult(Guid.NewGuid(), ex.Message);
        }
    }

    private static AiAnalysisResult FailedResult(Guid id, string message, string status = "Failed") =>
        new(id, "Unknown", "Unknown", 0, string.Empty, [], "unknown", status, message);

    internal sealed class AnalyzeResponse
    {
        [JsonPropertyName("sentiment")]
        public string? Sentiment { get; set; }

        [JsonPropertyName("risk_level")]
        public string? RiskLevel { get; set; }

        [JsonPropertyName("quality_score")]
        public double? QualityScore { get; set; }

        [JsonPropertyName("summary")]
        public string? Summary { get; set; }

        [JsonPropertyName("keywords")]
        public List<string>? Keywords { get; set; }

        [JsonPropertyName("analyzer_version")]
        public string? AnalyzerVersion { get; set; }

        [JsonPropertyName("analysis_status")]
        public string? AnalysisStatus { get; set; }

        [JsonPropertyName("error_message")]
        public string? ErrorMessage { get; set; }

        [JsonPropertyName("provider")]
        public string? Provider { get; set; }

        [JsonPropertyName("coaching_suggestion")]
        public string? CoachingSuggestion { get; set; }

        [JsonPropertyName("confidence")]
        public double? Confidence { get; set; }
    }
}
