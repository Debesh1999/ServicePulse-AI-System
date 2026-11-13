import React, { useState, useEffect } from 'react';
import { Card, CardHeader, CardBody, Row, Col, Spinner, Badge, Table } from 'reactstrap';
import { callsApi } from '../services/api';

export function DashboardPage() {
  const [stats, setStats] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    loadStats();
  }, []);

  const loadStats = async () => {
    try {
      setLoading(true);
      setError(null);
      const response = await callsApi.getStats();
      setStats(response.data);
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  const formatNumber = (num) => {
    if (num === undefined || num === null) return '0';
    return num.toLocaleString();
  };

  if (loading) {
    return (
      <div className="text-center py-5">
        <Spinner color="primary" />
        <p className="mt-2">Loading dashboard...</p>
      </div>
    );
  }

  if (error) {
    return (
      <div className="alert alert-danger">
        Error loading dashboard: {error}
        <button className="btn btn-primary ms-2" onClick={loadStats}>Retry</button>
      </div>
    );
  }

  if (!stats) {
    return <div className="alert alert-warning">No statistics available.</div>;
  }

  const statusColors = {
    'Completed': 'success',
    'Analyzing': 'warning',
    'Failed': 'danger',
    'Queued': 'secondary',
    'Draft': 'info',
  };

  return (
    <div>
      <h2 className="mb-4">Dashboard</h2>

      {/* Stats Cards Row */}
      <Row className="mb-4">
        <Col md={4} sm={6} className="mb-3">
          <Card>
            <CardBody className="text-center">
              <h3 className="text-primary mb-0">{formatNumber(stats.totalCalls)}</h3>
              <p className="text-muted mb-0">Total Calls</p>
            </CardBody>
          </Card>
        </Col>
        <Col md={4} sm={6} className="mb-3">
          <Card>
            <CardBody className="text-center">
              <h3 className="text-success mb-0">{formatNumber(stats.completedCalls)}</h3>
              <p className="text-muted mb-0">Completed</p>
            </CardBody>
          </Card>
        </Col>
        <Col md={4} sm={6} className="mb-3">
          <Card>
            <CardBody className="text-center">
              <h3 className="text-warning mb-0">{formatNumber(stats.analyzingCalls)}</h3>
              <p className="text-muted mb-0">Analyzing</p>
            </CardBody>
          </Card>
        </Col>
        <Col md={4} sm={6} className="mb-3">
          <Card>
            <CardBody className="text-center">
              <h3 className="text-danger mb-0">{formatNumber(stats.failedCalls)}</h3>
              <p className="text-muted mb-0">Failed</p>
            </CardBody>
          </Card>
        </Col>
        <Col md={4} sm={6} className="mb-3">
          <Card>
            <CardBody className="text-center">
              <h3 className="text-secondary mb-0">{formatNumber(stats.queuedCalls)}</h3>
              <p className="text-muted mb-0">Queued</p>
            </CardBody>
          </Card>
        </Col>
        <Col md={4} sm={6} className="mb-3">
          <Card>
            <CardBody className="text-center">
              <h3 className="text-info mb-0">{formatNumber(stats.averageDurationSeconds)}s</h3>
              <p className="text-muted mb-0">Avg Duration</p>
            </CardBody>
          </Card>
        </Col>
      </Row>

      {/* Status Distribution Table */}
      <Card className="mb-4">
        <CardHeader>
          <strong>Calls by Status</strong>
        </CardHeader>
        <CardBody>
          <Table striped hover size="sm">
            <thead>
              <tr>
                <th>Status</th>
                <th>Count</th>
                <th>Percentage</th>
              </tr>
            </thead>
            <tbody>
              {Object.entries(stats.statusCounts || {}).map(([status, count]) => (
                <tr key={status}>
                  <td>
                    <Badge color={statusColors[status] || 'primary'}>{status}</Badge>
                  </td>
                  <td>{formatNumber(count)}</td>
                  <td>
                    {stats.totalCalls > 0
                      ? `${((count / stats.totalCalls) * 100).toFixed(1)}%`
                      : '0%'}
                  </td>
                </tr>
              ))}
            </tbody>
          </Table>
        </CardBody>
      </Card>

      {/* Agent Performance Table */}
      <Card className="mb-4">
        <CardHeader>
          <strong>Calls by Agent</strong>
        </CardHeader>
        <CardBody>
          <Table striped hover size="sm">
            <thead>
              <tr>
                <th>Agent</th>
                <th>Call Count</th>
                <th>Percentage</th>
              </tr>
            </thead>
            <tbody>
              {Object.entries(stats.agentCounts || {})
                .sort(([, a], [, b]) => b - a)
                .map(([agent, count]) => (
                  <tr key={agent}>
                    <td>{agent}</td>
                    <td>{formatNumber(count)}</td>
                    <td>
                      {stats.totalCalls > 0
                        ? `${((count / stats.totalCalls) * 100).toFixed(1)}%`
                        : '0%'}
                    </td>
                  </tr>
                ))}
            </tbody>
          </Table>
        </CardBody>
      </Card>

      {/* Customer Volume Table */}
      <Card>
        <CardHeader>
          <strong>Top Customers by Call Volume</strong>
        </CardHeader>
        <CardBody>
          <Table striped hover size="sm">
            <thead>
              <tr>
                <th>Customer</th>
                <th>Call Count</th>
                <th>Percentage</th>
              </tr>
            </thead>
            <tbody>
              {Object.entries(stats.customerCounts || {})
                .sort(([, a], [, b]) => b - a)
                .slice(0, 10)
                .map(([customer, count]) => (
                  <tr key={customer}>
                    <td>{customer}</td>
                    <td>{formatNumber(count)}</td>
                    <td>
                      {stats.totalCalls > 0
                        ? `${((count / stats.totalCalls) * 100).toFixed(1)}%`
                        : '0%'}
                    </td>
                  </tr>
                ))}
            </tbody>
          </Table>
        </CardBody>
      </Card>
    </div>
  );
}