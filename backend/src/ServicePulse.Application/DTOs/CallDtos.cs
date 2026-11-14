namespace ServicePulse.Application.DTOs;

public record CallDto(
    Guid Id,
    string AgentName,
    string CustomerName,
    DateTime StartedAtUtc,
    int DurationSeconds,
    string Status
);

public record PagedCallListDto(
    List<CallDto> Calls,
    int TotalCount,
    int Page,
    int PageSize
);

public record CallDetailDto(
    Guid Id,
    string AgentName,
    string CustomerName,
    string? ExternalCallId,
    DateTime StartedAtUtc,
    int DurationSeconds,
    string Status,
    string? TranscriptText,
    AnalysisResultDto? Analysis,
    ManagerReviewDto? Review
);

public record AnalysisResultDto(
    Guid Id,
    string AnalyzerVersion,
    string Sentiment,
    string RiskLevel,
    decimal QualityScore,
    string? Summary,
    List<string> DetectedKeywords,
    string AnalysisStatus,
    string? ErrorMessage
);

public record ManagerReviewDto(
    Guid Id,
    string ReviewerDisplayName,
    string ReviewStatus,
    decimal? ManagerScore,
    string? CoachingNotes,
    string? OverrideReason,
    DateTime ReviewedAtUtc
);

public record CreateCallRequest(
    string AgentName,
    string CustomerName,
    string? ExternalCallId,
    DateTime StartedAtUtc,
    int DurationSeconds,
    string TranscriptText
);

// Manager Review DTOs
public record CreateManagerReviewRequest(
    Guid CallId,
    string ReviewStatus,
    decimal? ManagerScore,
    string? CoachingNotes,
    string? OverrideReason
);

public record UpdateManagerReviewRequest(
    string ReviewStatus,
    decimal? ManagerScore,
    string? CoachingNotes,
    string? OverrideReason
);
