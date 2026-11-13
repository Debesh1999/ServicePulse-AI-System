namespace ServicePulse.Domain.Enums;

public enum CallStatus
{
    Draft = 0,
    Queued = 1,
    Analyzing = 2,
    Completed = 3,
    Failed = 4
}

public enum ResolutionStatus
{
    Open = 0,
    Resolved = 1,
    Escalated = 2,
    Unknown = 3
}

public enum AnalysisStatus
{
    Succeeded = 0,
    Failed = 1
}

public enum Sentiment
{
    Positive = 0,
    Neutral = 1,
    Negative = 2
}

public enum RiskLevel
{
    Low = 0,
    Medium = 1,
    High = 2
}

public enum TranscriptSource
{
    ManualUpload = 0,
    Whisper = 1,
    Import = 2
}

public enum ReviewStatus
{
    Pending = 0,
    Reviewed = 1,
    Disputed = 2
}

public enum AuditAction
{
    CallCreated = 0,
    CallQueued = 1,
    AnalysisStarted = 2,
    AnalysisCompleted = 3,
    AnalysisFailed = 4,
    ReviewCreated = 5,
    ReviewUpdated = 6,
    ScoreOverridden = 7,
    RiskOverridden = 8,
    RetryRequested = 9
}