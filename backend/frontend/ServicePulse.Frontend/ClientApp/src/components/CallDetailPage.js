import React, { useState, useEffect } from 'react';
import { Card, CardHeader, CardBody, Button, Spinner, Alert, Badge, Nav, NavItem, NavLink } from 'reactstrap';
import { Link, useParams } from 'react-router-dom';
import { callsApi } from '../services/api';

export function CallDetailPage() {
  const { id } = useParams();
  const [call, setCall] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [activeTab, setActiveTab] = useState('overview');

  // eslint-disable-next-line react-hooks/exhaustive-deps
  useEffect(() => {
    loadCall();
  }, [id]);

  const loadCall = async () => {
    try {
      setLoading(true);
      setError(null);
      const response = await callsApi.getById(id);
      setCall(response.data);
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  const formatDate = (dateString) => {
    if (!dateString) return '-';
    return new Date(dateString).toLocaleString();
  };

  const getStatusBadge = (status) => {
    const colors = {
      'Completed': 'success',
      'Analyzing': 'warning',
      'Failed': 'danger',
      'Queued': 'secondary',
      'Draft': 'info',
    };
    return <Badge color={colors[status] || 'primary'}>{status}</Badge>;
  };

  if (loading) {
    return (
      <div className="text-center py-5">
        <Spinner color="primary" />
        <p className="mt-2">Loading call details...</p>
      </div>
    );
  }

  if (error) {
    return (
      <Alert color="danger">
        Error loading call: {error}
        <Button color="primary" className="ms-2" onClick={loadCall}>
          Retry
        </Button>
      </Alert>
    );
  }

  if (!call) {
    return <Alert color="warning">Call not found.</Alert>;
  }

  return (
    <div>
      <div className="d-flex justify-content-between align-items-center mb-4">
        <div>
          <h2>Call Details</h2>
          <p className="text-muted mb-0">Call ID: {call.id.substring(0, 8)}...</p>
        </div>
        <Link to="/calls">
          <Button color="secondary">Back to List</Button>
        </Link>
      </div>

      {/* Tabs */}
      <Card className="mb-4">
        <CardHeader>
          <Nav tabs>
            <NavItem>
              <NavLink
                className={activeTab === 'overview' ? 'active' : ''}
                onClick={() => setActiveTab('overview')}
              >
                Overview
              </NavLink>
            </NavItem>
            <NavItem>
              <NavLink
                className={activeTab === 'transcript' ? 'active' : ''}
                onClick={() => setActiveTab('transcript')}
              >
                Transcript
              </NavLink>
            </NavItem>
            {call.analysis && (
              <NavItem>
                <NavLink
                  className={activeTab === 'analysis' ? 'active' : ''}
                  onClick={() => setActiveTab('analysis')}
                >
                  Analysis
                </NavLink>
              </NavItem>
            )}
            {call.review && (
              <NavItem>
                <NavLink
                  className={activeTab === 'review' ? 'active' : ''}
                  onClick={() => setActiveTab('review')}
                >
                  Review
                </NavLink>
              </NavItem>
            )}
          </Nav>
        </CardHeader>
        <CardBody>
          {activeTab === 'overview' && (
            <>
              <div className="row mb-3">
                <div className="col-md-6">
                  <strong>Agent:</strong>
                  <p>{call.agentName || 'N/A'}</p>
                </div>
                <div className="col-md-6">
                  <strong>Customer:</strong>
                  <p>{call.customerName || 'N/A'}</p>
                </div>
              </div>
              <div className="row mb-3">
                <div className="col-md-4">
                  <strong>External Call ID:</strong>
                  <p>{call.externalCallId || 'N/A'}</p>
                </div>
                <div className="col-md-4">
                  <strong>Started At:</strong>
                  <p>{formatDate(call.startedAtUtc)}</p>
                </div>
                <div className="col-md-4">
                  <strong>Duration:</strong>
                  <p>{call.durationSeconds} seconds</p>
                </div>
              </div>
              <div className="row mb-3">
                <div className="col-md-4">
                  <strong>Status:</strong>
                  <p>{getStatusBadge(call.status)}</p>
                </div>
              </div>
            </>
          )}

          {activeTab === 'transcript' && (
            <div>
              {call.transcriptText ? (
                <pre style={{ whiteSpace: 'pre-wrap', fontFamily: 'inherit', backgroundColor: '#f8f9fa', padding: '1rem', borderRadius: '4px' }}>
                  {call.transcriptText}
                </pre>
              ) : (
                <p className="text-muted">No transcript available for this call.</p>
              )}
            </div>
          )}

          {activeTab === 'analysis' && call.analysis && (
            <AnalysisPanel analysis={call.analysis} />
          )}

          {activeTab === 'review' && call.review && (
            <ReviewPanel review={call.review} />
          )}

          {activeTab === 'analysis' && !call.analysis && (
            <p className="text-muted">No analysis available for this call.</p>
          )}

          {activeTab === 'review' && !call.review && (
            <p className="text-muted">No manager review available for this call.</p>
          )}
        </CardBody>
      </Card>
    </div>
  );
}

function AnalysisPanel({ analysis }) {
  const sentimentColors = {
    'Positive': 'success',
    'Negative': 'danger',
    'Neutral': 'warning',
  };
  const riskColors = {
    'Low': 'success',
    'Medium': 'warning',
    'High': 'danger',
    'Critical': 'danger',
  };

  return (
    <div>
      <div className="row mb-4">
        <div className="col-md-3">
          <strong>Sentiment:</strong>
          <div className="mt-1">
            <Badge color={sentimentColors[analysis.sentiment] || 'secondary'} pill>
              {analysis.sentiment}
            </Badge>
          </div>
        </div>
        <div className="col-md-3">
          <strong>Risk Level:</strong>
          <div className="mt-1">
            <Badge color={riskColors[analysis.riskLevel] || 'secondary'} pill>
              {analysis.riskLevel}
            </Badge>
          </div>
        </div>
        <div className="col-md-3">
          <strong>Quality Score:</strong>
          <div className="mt-1">
            <span className="badge bg-primary fs-6">{analysis.qualityScore}/100</span>
          </div>
        </div>
        <div className="col-md-3">
          <strong>Analyzer:</strong>
          <div className="mt-1 text-muted small">
            {analysis.analyzerVersion}
          </div>
        </div>
      </div>

      {analysis.summary && (
        <div className="mb-4">
          <strong>Summary:</strong>
          <p className="mt-1">{analysis.summary}</p>
        </div>
      )}

      {analysis.detectedKeywords && analysis.detectedKeywords.length > 0 && (
        <div className="mb-4">
          <strong>Detected Keywords:</strong>
          <div className="mt-2">
            {analysis.detectedKeywords.map((keyword, idx) => (
              <Badge key={idx} color="secondary" className="me-1 mb-1" pill>
                {keyword}
              </Badge>
            ))}
          </div>
        </div>
      )}

      {analysis.analysisStatus !== 'Succeeded' && analysis.errorMessage && (
        <Alert color="warning">
          <strong>Analysis Status:</strong> {analysis.analysisStatus}
          <br />
          <strong>Error:</strong> {analysis.errorMessage}
        </Alert>
      )}
    </div>
  );
}

const formatDate = (dateString) => {
  if (!dateString) return '-';
  return new Date(dateString).toLocaleString();
};

function ReviewPanel({ review }) {
  const statusColors = {
    'Reviewed': 'success',
    'Pending': 'warning',
    'Rejected': 'danger',
    'Approved': 'info',
  };

  return (
    <div>
      <div className="row mb-4">
        <div className="col-md-3">
          <strong>Review Status:</strong>
          <div className="mt-1">
            <Badge color={statusColors[review.reviewStatus] || 'secondary'} pill>
              {review.reviewStatus}
            </Badge>
          </div>
        </div>
        <div className="col-md-3">
          <strong>Manager Score:</strong>
          <div className="mt-1">
            {review.managerScore !== null && review.managerScore !== undefined ? (
              <span className="badge bg-primary fs-6">{review.managerScore}/100</span>
            ) : (
              <span className="text-muted">Not scored</span>
            )}
          </div>
        </div>
        <div className="col-md-3">
          <strong>Reviewer:</strong>
          <div className="mt-1 text-muted">
            {review.reviewerDisplayName}
          </div>
        </div>
        <div className="col-md-3">
          <strong>Reviewed At:</strong>
          <div className="mt-1 text-muted small">
            {formatDate(review.reviewedAtUtc)}
          </div>
        </div>
      </div>

      {review.coachingNotes && (
        <div className="mb-4">
          <strong>Coaching Notes:</strong>
          <p className="mt-1">{review.coachingNotes}</p>
        </div>
      )}

      {review.overrideReason && (
        <Alert color="info">
          <strong>Override Reason:</strong> {review.overrideReason}
        </Alert>
      )}
    </div>
  );
}