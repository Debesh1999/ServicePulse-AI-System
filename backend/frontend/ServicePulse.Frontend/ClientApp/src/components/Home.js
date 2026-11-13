import React, { Component } from 'react';
import { Button, Card, CardBody, Row, Col } from 'reactstrap';
import { Link } from 'react-router-dom';

export class Home extends Component {
  static displayName = Home.name;

  render() {
    return (
      <div>
        <div className="text-center mb-5">
          <h1 className="display-4">ServicePulse AI System</h1>
          <p className="lead text-muted">Call Center Analytics & Quality Management Platform</p>
        </div>

        <Row className="mb-4">
          <Col md={4}>
            <Card>
              <CardBody>
                <h5>📞 View Calls</h5>
                <p className="text-muted">Browse all call records with agent, customer, and status information.</p>
                <Link to="/calls">
                  <Button color="primary" block>View Calls</Button>
                </Link>
              </CardBody>
            </Card>
          </Col>
          <Col md={4}>
            <Card>
              <CardBody>
                <h5>➕ Add New Call</h5>
                <p className="text-muted">Create a new call record with agent, customer, and call details.</p>
                <Link to="/calls/add">
                  <Button color="success" block>Add Call</Button>
                </Link>
              </CardBody>
            </Card>
          </Col>
          <Col md={4}>
            <Card>
              <CardBody>
                <h5>📊 Features</h5>
                <ul className="text-muted small">
                  <li>Call transcripts with multi-turn conversations</li>
                  <li>AI-powered sentiment & risk analysis</li>
                  <li>Manager review & coaching workflow</li>
                  <li>Audit logging for compliance</li>
                </ul>
              </CardBody>
            </Card>
          </Col>
        </Row>

        <div className="text-center">
          <h5>Technology Stack</h5>
          <div className="d-flex justify-content-center flex-wrap gap-2">
            <span className="badge bg-primary fs-6 px-3 py-2">ASP.NET Core 8</span>
            <span className="badge bg-secondary fs-6 px-3 py-2">Entity Framework Core</span>
            <span className="badge bg-info fs-6 px-3 py-2">SQLite</span>
            <span className="badge bg-warning text-dark fs-6 px-3 py-2">React 18</span>
            <span className="badge bg-danger fs-6 px-3 py-2">Bootstrap 5</span>
            <span className="badge bg-dark fs-6 px-3 py-2">Vite</span>
          </div>
        </div>
      </div>
    );
  }
}
