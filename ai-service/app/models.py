"""Request and response models for the ServicePulse AI analysis API."""

from pydantic import BaseModel, Field


class AnalyzeRequest(BaseModel):
    """A single transcript to analyze."""
    text: str = Field(..., min_length=1, max_length=50000, description="Transcript text to analyze")
    language: str = Field(default="en", description="ISO 639-1 language code")


class AnalyzeResponse(BaseModel):
    """Analysis result returned by the rule-based engine or Gemini fallback."""
    sentiment: str = Field(..., description="Positive | Negative | Neutral")
    risk_level: str = Field(..., description="Low | Medium | High")
    quality_score: float = Field(..., ge=0.0, le=100.0, description="Quality score 0-100")
    summary: str = Field(..., description="One-sentence summary of the transcript")
    keywords: list[str] = Field(default_factory=list, description="Detected keywords")
    analyzer_version: str = Field(..., description="Version of the analyzer that produced this result")
    analysis_status: str = Field(default="Succeeded", description="Succeeded | Failed")
    error_message: str | None = Field(default=None)
    # Optional fields from Gemini Quality Coach
    provider: str | None = Field(default=None, description="RuleBased | Gemini | RuleBased (fallback)")
    coaching_suggestion: str | None = Field(default=None)
    evidence: list[str] | None = Field(default=None)
    confidence: float | None = Field(default=None, ge=0.0, le=1.0)
    fallback_reason: str | None = Field(default=None)


class HealthResponse(BaseModel):
    """Health check response."""
    status: str
    analyzer_version: str
