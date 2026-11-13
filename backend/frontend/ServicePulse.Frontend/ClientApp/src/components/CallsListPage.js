import React, { useState, useEffect } from 'react';
import { Table, Button, Spinner, Alert, Form, FormGroup, Label, Input, Row, Col, Badge, Card, CardBody } from 'reactstrap';
import { Link } from 'react-router-dom';
import { callsApi, lookupApi } from '../services/api';
import { Pager } from './Pager';

const PAGE_SIZE = 10;

export function CallsListPage() {
  const [calls, setCalls] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [searchParams, setSearchParams] = useState({
    agentName: '',
    customerName: '',
    status: '',
    from: '',
    to: '',
  });
  const [page, setPage] = useState(1);
  const [totalCount, setTotalCount] = useState(0);
  const [agents, setAgents] = useState([]);
  const [customers, setCustomers] = useState([]);
  const [statuses] = useState(['Queued', 'Analyzing', 'Completed', 'Failed', 'Draft']);

  // eslint-disable-next-line react-hooks/exhaustive-deps
  useEffect(() => {
    loadOptions();
    loadCalls();
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

  const loadCalls = async () => {
    try {
      setLoading(true);
      setError(null);
      const params = new URLSearchParams();
      Object.entries(searchParams).forEach(([key, value]) => {
        if (value) params.append(key, value);
      });
      params.append('page', page);
      params.append('pageSize', PAGE_SIZE);
      const response = await callsApi.search(params);
      setCalls(response.data.calls || []);
      setTotalCount(response.data.totalCount || 0);
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  const handleFilterChange = (field, value) => {
    setSearchParams(prev => ({ ...prev, [field]: value }));
  };

  const handleSearch = (e) => {
    e.preventDefault();
    setPage(1);
    loadCalls();
  };

  const clearFilters = () => {
    setSearchParams({
      agentName: '',
      customerName: '',
      status: '',
      from: '',
      to: '',
    });
    setPage(1);
    loadCalls();
  };

  const handlePageChange = (newPage) => {
    setPage(newPage);
    loadCalls();
  };

  if (loading) {
    return (
      <div className="text-center py-5">
        <Spinner color="primary" />
        <p className="mt-2">Loading calls...</p>
      </div>
    );
  }

  if (error) {
    return (
      <Alert color="danger">
        Error loading calls: {error}
        <Button color="primary" className="ms-2" onClick={loadCalls}>
          Retry
        </Button>
      </Alert>
    );
  }

  const getStatusColor = (status) => {
    switch (status) {
      case 'Completed': return 'success';
      case 'Analyzing': return 'warning';
      case 'Failed': return 'danger';
      case 'Queued': return 'secondary';
      default: return 'primary';
    }
  };

  return (
    <div>
      <div className="d-flex justify-content-between align-items-center mb-4">
        <h2>Calls</h2>
        <Link to="/calls/add">
          <Button color="primary">Add New Call</Button>
        </Link>
      </div>

      {/* Search/Filter Form */}
      <Card className="mb-4">
        <CardBody>
          <Form onSubmit={handleSearch}>
            <Row className="g-3">
              <Col md={3}>
                <FormGroup>
                  <Label for="filterAgent">Agent</Label>
                  <Input
                    type="select"
                    id="filterAgent"
                    value={searchParams.agentName}
                    onChange={(e) => handleFilterChange('agentName', e.target.value)}
                  >
                    <option value="">All Agents</option>
                    {agents.map(agent => (
                      <option key={agent} value={agent}>{agent}</option>
                    ))}
                  </Input>
                </FormGroup>
              </Col>

              <Col md={3}>
                <FormGroup>
                  <Label for="filterCustomer">Customer</Label>
                  <Input
                    type="select"
                    id="filterCustomer"
                    value={searchParams.customerName}
                    onChange={(e) => handleFilterChange('customerName', e.target.value)}
                  >
                    <option value="">All Customers</option>
                    {customers.map(customer => (
                      <option key={customer} value={customer}>{customer}</option>
                    ))}
                  </Input>
                </FormGroup>
              </Col>

              <Col md={2}>
                <FormGroup>
                  <Label for="filterStatus">Status</Label>
                  <Input
                    type="select"
                    id="filterStatus"
                    value={searchParams.status}
                    onChange={(e) => handleFilterChange('status', e.target.value)}
                  >
                    <option value="">All Status</option>
                    {statuses.map(status => (
                      <option key={status} value={status}>{status}</option>
                    ))}
                  </Input>
                </FormGroup>
              </Col>

              <Col md={2}>
                <FormGroup>
                  <Label for="filterFrom">From Date</Label>
                  <Input
                    type="date"
                    id="filterFrom"
                    value={searchParams.from}
                    onChange={(e) => handleFilterChange('from', e.target.value)}
                  />
                </FormGroup>
              </Col>

              <Col md={2}>
                <FormGroup>
                  <Label for="filterTo">To Date</Label>
                  <Input
                    type="date"
                    id="filterTo"
                    value={searchParams.to}
                    onChange={(e) => handleFilterChange('to', e.target.value)}
                  />
                </FormGroup>
              </Col>
            </Row>
            <Row className="g-3">
              <Col md={12} className="d-flex gap-2">
                <Button color="primary" type="submit">
                  Search
                </Button>
                <Button color="secondary" type="button" onClick={clearFilters}>
                  Clear Filters
                </Button>
              </Col>
            </Row>
          </Form>
        </CardBody>
      </Card>

      {calls.length === 0 ? (
        <Alert color="info">No calls found. Click "Add New Call" to create one.</Alert>
      ) : (
        <>
          <Table striped bordered hover responsive>
            <thead>
              <tr>
                <th>ID</th>
                <th>Agent</th>
                <th>Customer</th>
                <th>Started At</th>
                <th>Duration (sec)</th>
                <th>Status</th>
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
              {calls.map((call) => (
                <tr key={call.id}>
                  <td>{call.id.substring(0, 8)}...</td>
                  <td>{call.agentName || 'N/A'}</td>
                  <td>{call.customerName || 'N/A'}</td>
                  <td>{new Date(call.startedAtUtc).toLocaleString()}</td>
                  <td>{call.durationSeconds}</td>
                  <td>
                    <Badge color={getStatusColor(call.status)}>{call.status}</Badge>
                  </td>
                  <td>
                    <Link to={`/calls/${call.id}`}>
                      <Button color="link" size="sm">View</Button>
                    </Link>
                  </td>
                </tr>
              ))}
            </tbody>
          </Table>
          <Pager
            currentPage={page}
            totalPages={Math.ceil(totalCount / PAGE_SIZE) || 1}
            onPageChange={handlePageChange}
          />
        </>
      )}
    </div>
  );
}