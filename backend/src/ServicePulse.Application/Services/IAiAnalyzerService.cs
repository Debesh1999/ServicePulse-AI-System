namespace ServicePulse.Application.Services;

public record AiAnalysisResult(
    Guid Id,
    string Sentiment,
    string RiskLevel,
    decimal QualityScore,
    string Summary,
    List<string> Keywords,
    string AnalyzerVersion,
    string AnalysisStatus,
    string? ErrorMessage
);

public interface IAiAnalyzerService
{
    Task<AiAnalysisResult> AnalyzeAsync(string transcriptText, string language = "en");
}
