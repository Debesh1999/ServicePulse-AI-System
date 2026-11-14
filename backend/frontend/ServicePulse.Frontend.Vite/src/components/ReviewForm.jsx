import React, { useState, useEffect } from 'react';
import { Form, FormGroup, Label, Input, Button, Alert, Badge, Card, CardBody } from 'reactstrap';
import { managerReviewsApi } from '../services/api';

export function ReviewForm({ callId, initialReview, onClose, onSuccess }) {
  const [formData, setFormData] = useState({
    reviewStatus: initialReview?.reviewStatus || 'Pending',
    managerScore: initialReview?.managerScore || '',
    coachingNotes: initialReview?.coachingNotes || '',
    overrideReason: initialReview?.overrideReason || '',
  });
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  const [success, setSuccess] = useState(false);

  const isEditing = !!initialReview?.id;

  const handleChange = (field, value) => {
    setFormData(prev => ({ ...prev, [field]: value }));
    setError(null);
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError(null);
    setLoading(true);

    try {
      const payload = {
        CallId: callId,
        ReviewStatus: formData.reviewStatus,
        ManagerScore: formData.managerScore ? parseFloat(formData.managerScore) : null,
        CoachingNotes: formData.coachingNotes || null,
        OverrideReason: formData.overrideReason || null,
      };

      // Validate: if manager score or override reason is provided, both should be filled
      if ((payload.ManagerScore !== null || payload.OverrideReason) && !payload.OverrideReason) {
        throw new Error('Override reason is required when changing the score or risk level.');
      }

      if (isEditing) {
        await managerReviewsApi.update(initialReview.id, payload);
      } else {
        await managerReviewsApi.create(payload);
      }

      setSuccess(true);
      if (onSuccess) onSuccess();
      if (onClose) onClose();
    } catch (err) {
      setError(err.response?.data?.message || err.message || 'Failed to save review');
    } finally {
      setLoading(false);
    }
  };

  const handleCancel = () => {
    if (onClose) onClose();
  };

  return (
    <Card className="mb-4">
      <CardBody>
        <h4>{isEditing ? 'Edit Manager Review' : 'Create Manager Review'}</h4>
        <p className="text-muted">Add coaching notes and optionally override the AI quality score with a required reason.</p>

        {error && (
          <Alert color="danger" className="mb-3">
            {error}
          </Alert>
        )}

        {success && !loading && (
          <Alert color="success" className="mb-3">
            Review {isEditing ? 'updated' : 'created'} successfully!
          </Alert>
        )}

        <Form onSubmit={handleSubmit}>
          <FormGroup className="mb-3">
            <Label for="reviewStatus">Review Status <span className="text-danger">*</span></Label>
            <Input
              type="select"
              id="reviewStatus"
              name="reviewStatus"
              value={formData.reviewStatus}
              onChange={(e) => handleChange('reviewStatus', e.target.value)}
              required
            >
              <option value="Pending">Pending</option>
              <option value="Reviewed">Reviewed</option>
              <option value="Approved">Approved</option>
              <option value="Rejected">Rejected</option>
            </Input>
          </FormGroup>

          <FormGroup className="mb-3">
            <Label for="managerScore">Manager Score (0-100)</Label>
            <Input
              type="number"
              id="managerScore"
              name="managerScore"
              min="0"
              max="100"
              value={formData.managerScore}
              onChange={(e) => handleChange('managerScore', e.target.value)}
              placeholder="Enter score 0-100"
            />
            <small className="form-text text-muted">
              Override the AI quality score. Requires an override reason.
            </small>
          </FormGroup>

          <FormGroup className="mb-3">
            <Label for="coachingNotes">Coaching Notes</Label>
            <textarea
              id="coachingNotes"
              name="coachingNotes"
              className="form-control"
              rows="4"
              value={formData.coachingNotes}
              onChange={(e) => handleChange('coachingNotes', e.target.value)}
              placeholder="Add coaching feedback for the agent..."
            />
          </FormGroup>

          <FormGroup className="mb-3">
            <Label for="overrideReason">Override Reason <span className="text-danger">*</span></Label>
            <textarea
              id="overrideReason"
              name="overrideReason"
              className="form-control"
              rows="3"
              value={formData.overrideReason}
              onChange={(e) => handleChange('overrideReason', e.target.value)}
              placeholder="Required when overriding AI score or risk level..."
            />
            <small className="form-text text-muted">
              Required when providing a manager score that differs from the AI score.
            </small>
          </FormGroup>

          <div className="d-flex gap-2">
            <Button color="primary" type="submit" disabled={loading}>
              {loading ? 'Saving...' : (isEditing ? 'Update Review' : 'Create Review')}
            </Button>
            <Button color="secondary" type="button" onClick={handleCancel} disabled={loading}>
              Cancel
            </Button>
          </div>
        </Form>
      </CardBody>
    </Card>
  );
}