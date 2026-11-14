"""Rule-based transcript analyzer for ServicePulse.

Implements the quality score formula from the architecture doc:
  Base Score = 50.0
  + 10.0  greeting detected
  + 20.0  resolution wording detected
  + 15.0  apology + next step present
  - 10.0  refund / cancellation keywords
  - 15.0  frustrated / angry language
  - 20.0  supervisor escalation requested
  Final = clamped 0.0 - 100.0
"""

import re
from collections import Counter

ANALYZER_VERSION = "ServicePulse-AI-v1.2.0"

# --- Keyword dictionaries -------------------------------------------------

GREETING_PHRASES = [
    "hello", "hi ", "good morning", "good afternoon", "good evening",
    "thank you for calling", "how can i help", "my name is",
]

RESOLUTION_PHRASES = [
    "issue is resolved", "problem is solved", "fixed", "all sorted",
    "that resolves", "taken care of", "no further action needed",
]

APOLOGY_PHRASES = [
    "sorry for the inconvenience", "apologize", "my apologies",
    "sorry about that", "regret any inconvenience",
]

NEXT_STEP_PHRASES = [
    "let me help", "i will", "i'll", "let me", "we'll send",
    "i have sent", "i will follow up", "next step",
]

REFUND_KEYWORDS = [
    "refund", "cancellation", "cancel", "money back", "credit note",
    "reimburse", "partial refund", "full refund",
]

FRUSTRATED_KEYWORDS = [
    "angry", "frustrated", "outrageous", "ridiculous", "absurd",
    "this is unacceptable", "i'm not happy", "i am not happy",
    "disappointed", "terrible service", "worst", "useless",
    "waste of time", "never again", "complaint",
]

ESCALATION_KEYWORDS = [
    "speak to a manager", "talk to a supervisor", "speak to your supervisor",
    "escalate", "escalation", "manager please", "put your manager on",
    "complaint to management", "formal complaint",
]

POSITIVE_KEYWORDS = [
    "thank you", "appreciate", "great", "excellent", "perfect",
    "helpful", "thank you so much", "awesome", "fantastic",
]

NEGATIVE_KEYWORDS = [
    "broken", "not working", "error", "bug", "crash", "failed",
    "delay", "late", "waiting", "long wait", "hold on",
    "can't", "cannot", "unable", "issue", "problem",
]

ALL_KEYWORDS = {
    "greeting": GREETING_PHRASES,
    "resolution": RESOLUTION_PHRASES,
    "apology": APOLOGY_PHRASES,
    "next_step": NEXT_STEP_PHRASES,
    "refund": REFUND_KEYWORDS,
    "frustrated": FRUSTRATED_KEYWORDS,
    "escalation": ESCALATION_KEYWORDS,
    "positive": POSITIVE_KEYWORDS,
    "negative": NEGATIVE_KEYWORDS,
}


def _contains(text: str, phrases: list[str]) -> bool:
    return any(p in text for p in phrases)


def _detect_keywords(text: str) -> list[str]:
    """Return a flat list of keyword categories detected in the text."""
    detected = []
    for category, phrases in ALL_KEYWORDS.items():
        if any(p in text for p in phrases):
            detected.append(category)
    return detected


def _sentiment(pos_hits: int, neg_hits: int) -> str:
    if pos_hits > neg_hits:
        return "Positive"
    if neg_hits > pos_hits:
        return "Negative"
    return "Neutral"


def _risk_level(quality_score: float, frustrated: bool, escalated: bool) -> str:
    # Escalation and frustration are hard red flags regardless of score
    if escalated or frustrated:
        return "High"
    if quality_score < 60:
        return "High"
    if quality_score < 80:
        return "Medium"
    return "Low"


def _summarize(text: str, sentiment: str, quality_score: float) -> str:
    """Produce a one-sentence summary from the analysis result."""
    if sentiment == "Positive":
        return f"Positive interaction with a quality score of {quality_score:.1f}."
    if sentiment == "Negative":
        return f"Negative interaction with a quality score of {quality_score:.1f}."
    return f"Neutral interaction with a quality score of {quality_score:.1f}."


def analyze(text: str, language: str = "en") -> dict:
    """Analyze a transcript and return a result dict matching AnalyzeResponse."""
    lowered = text.lower()

    greeting = _contains(lowered, GREETING_PHRASES)
    resolution = _contains(lowered, RESOLUTION_PHRASES)
    apology = _contains(lowered, APOLOGY_PHRASES)
    next_step = _contains(lowered, NEXT_STEP_PHRASES)
    refund = _contains(lowered, REFUND_KEYWORDS)
    frustrated = _contains(lowered, FRUSTRATED_KEYWORDS)
    escalated = _contains(lowered, ESCALATION_KEYWORDS)

    pos_hits = sum(1 for kw in POSITIVE_KEYWORDS if kw in lowered)
    neg_hits = sum(1 for kw in NEGATIVE_KEYWORDS if kw in lowered)

    # --- Quality score formula (from architecture doc) ---
    score = 50.0
    if greeting:
        score += 10.0
    if resolution:
        score += 20.0
    if apology and next_step:
        score += 15.0
    if refund:
        score -= 10.0
    if frustrated:
        score -= 15.0
    if escalated:
        score -= 20.0

    score = max(0.0, min(100.0, score))

    sentiment = _sentiment(pos_hits, neg_hits)
    risk_level = _risk_level(score, frustrated, escalated)
    keywords = _detect_keywords(lowered)
    summary = _summarize(lowered, sentiment, score)

    return {
        "sentiment": sentiment,
        "risk_level": risk_level,
        "quality_score": round(score, 2),
        "summary": summary,
        "keywords": keywords,
        "analyzer_version": ANALYZER_VERSION,
        "analysis_status": "Succeeded",
        "error_message": None,
    }