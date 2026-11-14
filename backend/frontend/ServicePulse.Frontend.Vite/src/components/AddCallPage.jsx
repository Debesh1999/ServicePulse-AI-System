import React, { useState, useEffect } from 'react';
import { Form, FormGroup, Label, Input, Button, Alert, Row, Col } from 'reactstrap';
import { Link, useNavigate } from 'react-router-dom';
import { callsApi, lookupApi } from '../services/api';

export function AddCallPage() {
  const navigate = useNavigate();
  const [agentName, setAgentName] = useState('');
  const [customerName, setCustomerName] = useState('');
  const [externalCallId, setExternalCallId] = useState('');
  const [startedAtUtc, setStartedAtUtc] = useState(new Date().toISOString().slice(0, 16));
  const [durationSeconds, setDurationSeconds] = useState(0);
  const [transcriptText, setTranscriptText] = useState('');
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  const [success, setSuccess] = useState(false);
  const [agents, setAgents] = useState([]);
  const [customers, setCustomers] = useState([]);

  useEffect(() => {
    loadOptions();
  }, []);

  const loadOptions = async () => {
    try {
      const [agentsRes, customersRes] = await Promise.all([
        lookupApi.getAgents(),
        lookupApi.getCustomers(),
      ]);
      setAgents(agentsRes.data.map(a => a.name));
      setCustomers(customersRes.data.map(c => c.name));
    } catch (err) {
      console.error('Failed to load options:', err);
    }
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    if (!agentName || !customerName) {
      setError('Please select both an agent and a customer.');
      return;
    }
    try {
      setLoading(true);
      setError(null);
      setSuccess(false);

      const callData = {
        agentName,
        customerName,
        externalCallId: externalCallId || null,
        startedAtUtc: new Date(startedAtUtc).toISOString(),
        durationSeconds: parseInt(durationSeconds) || 0,
        transcriptText,
      };

      await callsApi.create(callData);
      setSuccess(true);
      setTimeout(() => navigate('/calls'), 2000);
    } catch (err) {
      setError(err.response?.data?.message || err.message);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div>
      <div className="d-flex justify-content-between align-items-center mb-4">
        <h2>Add New Call</h2>
        <Link to="/calls">
          <Button color="secondary">Back to List</Button>
        </Link>
      </div>

      {success && (
        <Alert color="success">
          Call created successfully! Redirecting...
        </Alert>
      )}

      {error && (
        <Alert color="danger">
          {error}
        </Alert>
      )}

      <Form onSubmit={handleSubmit}>
        <Row>
          <Col md={6}>
            <FormGroup>
              <Label for="agentName">Agent Name</Label>
              <Input
                type="select"
                id="agentName"
                value={agentName}
                onChange={(e) => setAgentName(e.target.value)}
              >
                <option value="">Select Agent</option>
                {agents.map(agent => (
                  <option key={agent} value={agent}>{agent}</option>
                ))}
              </Input>
            </FormGroup>
          </Col>

          <Col md={6}>
            <FormGroup>
              <Label for="customerName">Customer Name</Label>
              <Input
                type="select"
                id="customerName"
                value={customerName}
                onChange={(e) => setCustomerName(e.target.value)}
              >
                <option value="">Select Customer</option>
                {customers.map(customer => (
                  <option key={customer} value={customer}>{customer}</option>
                ))}
              </Input>
            </FormGroup>
          </Col>
        </Row>

        <Row>
          <Col md={6}>
            <FormGroup>
              <Label for="externalCallId">External Call ID</Label>
              <Input
                type="text"
                id="externalCallId"
                value={externalCallId}
                onChange={(e) => setExternalCallId(e.target.value)}
                placeholder="Optional external reference"
              />
            </FormGroup>
          </Col>

          <Col md={6}>
            <FormGroup>
              <Label for="durationSeconds">Duration (seconds)</Label>
              <Input
                type="number"
                id="durationSeconds"
                value={durationSeconds}
                onChange={(e) => setDurationSeconds(e.target.value)}
                min="0"
              />
            </FormGroup>
          </Col>
        </Row>

        <Row>
          <Col md={6}>
            <FormGroup>
              <Label for="startedAtUtc">Started At (UTC)</Label>
              <Input
                type="datetime-local"
                id="startedAtUtc"
                value={startedAtUtc}
                onChange={(e) => setStartedAtUtc(e.target.value)}
              />
            </FormGroup>
          </Col>
        </Row>

        <FormGroup>
          <Label for="transcriptText">Transcript Text</Label>
          <Input
            type="textarea"
            id="transcriptText"
            rows="6"
            value={transcriptText}
            onChange={(e) => setTranscriptText(e.target.value)}
            placeholder="Enter the call transcript..."
          />
        </FormGroup>

        <Button color="primary" type="submit" disabled={loading}>
          {loading ? 'Creating...' : 'Create Call'}
        </Button>
      </Form>
    </div>
  );
}