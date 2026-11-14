"""ServicePulse AI analysis microservice.

FastAPI + Pydantic transcript analyzer with Gemini Quality Coach and rule-based fallback.
Runs standalone on :8000. The .NET backend calls it via HTTP.
"""

import os
from fastapi import FastAPI, HTTPException

from analyzer import analyze as rule_based_analyze
from analyzer_gemini import analyze_with_gemini_fallback
from models import AnalyzeRequest, AnalyzeResponse, HealthResponse

GEMINI_API_KEY = os.environ.get("GEMINI_API_KEY", "").strip()
ANALYZER_VERSION = "ServicePulse-AI-v1.3.0"

app = FastAPI(
    title="ServicePulse AI Analyzer",
    description="Transcript analysis for the ServicePulse quality inspection system with Gemini fallback.",
    version="1.3.0",
)


@app.get("/health", response_model=HealthResponse)
async def health() -> HealthResponse:
    return HealthResponse(status="Healthy", analyzer_version=ANALYZER_VERSION)


@app.post("/analyze", response_model=AnalyzeResponse)
async def analyze_transcript(request: AnalyzeRequest) -> AnalyzeResponse:
    try:
        result = analyze_with_gemini_fallback(
            text=request.text,
            language=request.language,
            gemini_api_key=GEMINI_API_KEY,
        )
        return AnalyzeResponse(**result)
    except Exception as exc:
        raise HTTPException(
            status_code=500,
            detail=f"Analysis failed: {exc}",
        )


@app.post("/analyze/rules-only")
async def analyze_rules_only(request: AnalyzeRequest) -> dict:
    """Rules-only endpoint for deterministic testing without Gemini."""
    result = rule_based_analyze(request.text, request.language)
    result["provider"] = "RuleBased"
    return result


if __name__ == "__main__":
    import uvicorn
    uvicorn.run(app, host="0.0.0.0", port=8000)
