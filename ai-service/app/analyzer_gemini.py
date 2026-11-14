"""Gemini-based transcript analyzer with rule-based fallback."""

import json
import re
from typing import Optional

try:
    import google.generativeai as genai  # type: ignore
    _GENAI_AVAILABLE = True
except ImportError:
    _GENAI_AVAILABLE = False

from analyzer import analyze as rule_based_analyze

ANALYZER_VERSION = "ServicePulse-AI-v1.3.0"

_PROMPT_TEMPLATE = """\
You are a quality coach for customer support calls. Analyze the following call transcript and return JSON only, no other text.

JSON format required:
{{
  "sentiment": "Positive|Negative|Neutral",
  "risk_level": "Low|Medium|High",
  "quality_score": <number 0-100>,
  "summary": "<one sentence summary>",
  "keywords": ["keyword1", "keyword2"],
  "coaching_suggestion": "<brief coaching recommendation>",
  "evidence": ["quote1", "quote2"]
}}

Transcript:
{transcript}
"""


def _parse_gemini_output(text: str) -> dict:
    """Parse JSON from Gemini response, extracting from code blocks if present."""
    text = text.strip()
    match = re.search(r'```(?:json)?\s*([\s\S]*?)```', text)
    if match:
        text = match.group(1).strip()
    return json.loads(text)


class GeminiAnalyzer:
    """Calls Gemini to produce a Quality Coach draft."""

    def __init__(self, api_key: str):
        self._api_key = api_key
        self._model_name = "gemini-pro"

    def _initialize_client(self) -> None:
        if not _GENAI_AVAILABLE:
            raise ImportError("google-generativeai package not installed")
        genai.configure(api_key=self._api_key)

    def analyze(self, text: str) -> dict:
        """Run analysis via Gemini. Returns a draft result."""
        if not _GENAI_AVAILABLE:
            raise ImportError("google-generativeai package not installed")

        self._initialize_client()
        model = genai.GenerativeModel(self._model_name)
        prompt = _PROMPT_TEMPLATE.format(transcript=text)

        response = model.generate_content(prompt)
        parsed = _parse_gemini_output(response.text)

        return {
            "sentiment": parsed.get("sentiment", "Neutral"),
            "risk_level": parsed.get("risk_level", "Medium"),
            "quality_score": parsed.get("quality_score", 50.0),
            "summary": parsed.get("summary", ""),
            "keywords": parsed.get("keywords", []),
            "analyzer_version": ANALYZER_VERSION,
            "provider": "Gemini",
            "coaching_suggestion": parsed.get("coaching_suggestion", ""),
            "evidence": parsed.get("evidence", []),
            "confidence": 0.85,
            "analysis_status": "Succeeded",
            "error_message": None,
        }


def analyze_with_gemini_fallback(
    text: str,
    language: str = "en",
    gemini_api_key: Optional[str] = None,
) -> dict:
    """Try Gemini first; fall back to rule-based on failure."""
    if not gemini_api_key or gemini_api_key.strip() == "":
        result = rule_based_analyze(text, language)
        result["provider"] = "RuleBased"
        return result

    try:
        analyzer = GeminiAnalyzer(gemini_api_key)
        result = analyzer.analyze(text)
        result["fallback_reason"] = None
        return result
    except Exception:
        result = rule_based_analyze(text, language)
        result["provider"] = "RuleBased (fallback from Gemini)"
        return result
