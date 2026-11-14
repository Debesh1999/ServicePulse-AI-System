import React, { useState } from 'react';
import { Form, FormGroup, Label, Input, Button, Alert, Card, CardBody, CardHeader, Container } from 'reactstrap';
import { Link, useNavigate, useLocation } from 'react-router-dom';
import { authService } from '../services/authService';

export function LoginPage() {
  const navigate = useNavigate();
  const location = useLocation();
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState(null);
  const [loading, setLoading] = useState(false);

  const from = location.state?.from?.pathname || '/dashboard';

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError(null);
    setLoading(true);
    try {
      const result = await authService.login(email, password);
      navigate(from, { replace: true });
    } catch (err) {
      setError(err.response?.data?.message || 'Invalid email or password.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <Container className="mt-5" style={{ maxWidth: '460px' }}>
      <Card>
        <CardHeader className="text-center"><h4 className="mb-0">ServicePulse Login</h4></CardHeader>
        <CardBody>
          {error && <Alert color="danger">{error}</Alert>}
          <Form onSubmit={handleSubmit}>
            <FormGroup>
              <Label for="email">Email</Label>
              <Input
                type="email"
                id="email"
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                placeholder="you@example.com"
                required
              />
            </FormGroup>
            <FormGroup>
              <Label for="password">Password</Label>
              <Input
                type="password"
                id="password"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
                required
              />
            </FormGroup>
            <Button color="primary" block disabled={loading}>
              {loading ? 'Signing in...' : 'Sign In'}
            </Button>
          </Form>
          <p className="text-muted small mt-3 mb-0">
            Demo accounts seeded on first run:
            <br />admin@servicepulse.local / Admin@123 (Admin)
            <br />priya.sharma@servicepulse.local / Priya@123 (Manager)
            <br />alice.johnson@servicepulse.local / Alice@123 (Agent)
          </p>
          <p className="mt-3 mb-0">
            Don't have an account? <Link to="/register">Register</Link>
          </p>
        </CardBody>
      </Card>
    </Container>
  );
}