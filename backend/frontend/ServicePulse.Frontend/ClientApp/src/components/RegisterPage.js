import React, { useState } from 'react';
import { Form, FormGroup, Label, Input, Button, Alert, Card, CardBody, CardHeader, Container } from 'reactstrap';
import { Link, useNavigate } from 'react-router-dom';
import { authService } from '../services/authService';

export function RegisterPage() {
  const navigate = useNavigate();
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [displayName, setDisplayName] = useState('');
  const [role, setRole] = useState('Agent');
  const [error, setError] = useState(null);
  const [loading, setLoading] = useState(false);

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError(null);
    setLoading(true);
    try {
      const result = await authService.register(email, password, displayName, role);
      authService.save(result.accessToken, result.user);
      navigate('/dashboard', { replace: true });
    } catch (err) {
      setError(err.response?.data?.message || 'Registration failed.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <Container className="mt-5" style={{ maxWidth: '480px' }}>
      <Card>
        <CardHeader className="text-center"><h4 className="mb-0">Create Account</h4></CardHeader>
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
              <Label for="displayName">Display Name</Label>
              <Input
                type="text"
                id="displayName"
                value={displayName}
                onChange={(e) => setDisplayName(e.target.value)}
                placeholder="Jane Doe"
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
                minLength="6"
              />
            </FormGroup>
            <FormGroup>
              <Label for="role">Role</Label>
              <Input
                type="select"
                id="role"
                value={role}
                onChange={(e) => setRole(e.target.value)}
              >
                <option value="Agent">Agent</option>
                <option value="Manager">Manager</option>
                <option value="Admin">Admin</option>
              </Input>
            </FormGroup>
            <Button color="primary" block disabled={loading}>
              {loading ? 'Creating...' : 'Register'}
            </Button>
          </Form>
          <p className="mt-3 mb-0 text-center">
            Already have an account? <Link to="/login">Sign in</Link>
          </p>
        </CardBody>
      </Card>
    </Container>
  );
}