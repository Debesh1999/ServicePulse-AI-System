using ServicePulse.Application.Services;

namespace ServicePulse.Tests;

public class FakeAiAnalyzer : IAiAnalyzerService
{
    public AiAnalysisResult? LastResult { get; set; } =
        new(Guid.NewGuid(), "Positive", "Low", 85m, "Good call", ["greeting"], "test-v1", "Succeeded", null);

    public Task<AiAnalysisResult> AnalyzeAsync(string transcriptText, string language = "en")
        => Task.FromResult(LastResult!);
}
