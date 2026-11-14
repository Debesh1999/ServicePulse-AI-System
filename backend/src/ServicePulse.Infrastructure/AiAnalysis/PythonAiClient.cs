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
    private readonly TimeSpan _timeout = TimeSpan.FromSeconds(30);

    public PythonAiClient(HttpClient http, ILogger<PythonAiClient> logger)
    {
        _http = http;
        _logger = logger;
        _http.Timeout = _timeout;
    }

    public async Task<AiAnalysisResult> AnalyzeAsync(string transcriptText, string language = "en")
    {
        var payload = new { text = transcriptText, language };
        var cts = new CancellationTokenSource(_timeout);

        try
        {
            var response = await _http.PostAsJsonAsync("/analyze", payload, cts.Token);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cts.Token);
                _logger.LogWarning("AI analysis call failed with status {StatusCode}: {Body}", response.StatusCode, errorContent);
                return FailedResult(Guid.NewGuid(), $"Analysis service unavailable ({response.StatusCode})", response.StatusCode.ToString(), "TimeoutOrHttpError");
            }

            var result = await response.Content.ReadFromJsonAsync<AnalyzeResponse>(cancellationToken: cts.Token);
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
                result.ErrorMessage,
                result.Provider ?? "Unknown",
                result.FallbackReason);
        }
        catch (OperationCanceledException ex) when (ex.CancellationToken == cts.Token)
        {
            _logger.LogWarning("AI analysis timed out after {Timeout} seconds", _timeout.TotalSeconds);
            return FailedResult(Guid.NewGuid(), "AI analysis timed out", "Timeout", "Timeout");
        }
        catch (HttpRequestException ex)
        {
            _logger.LogWarning(ex, "AI analysis HTTP request failed");
            return FailedResult(Guid.NewGuid(), "AI service connection failed", "ConnectionError", "ConnectionError");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to parse AI response");
            return FailedResult(Guid.NewGuid(), ex.Message, "ParseError", "ParseError");
        }
    }

    private static AiAnalysisResult FailedResult(Guid id, string message, string status = "Failed", string? fallbackReason = null) =>
        new(id, "Unknown", "Unknown", 0, string.Empty, [], "unknown", status, message, "RuleBased", fallbackReason);

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

        [JsonPropertyName("fallback_reason")]
        public string? FallbackReason { get; set; }
    }
}
