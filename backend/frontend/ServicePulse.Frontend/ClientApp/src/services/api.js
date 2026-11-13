import axios from 'axios';

// Use relative URL - the React dev server proxy will forward /api requests to the backend
const api = axios.create({
  baseURL: '/',
  headers: {
    'Content-Type': 'application/json',
  },
});

export const callsApi = {
  getAll: () => api.get('/api/calls'),
  getById: (id) => api.get(`/api/calls/${id}`),
  create: (callData) => api.post('/api/calls', callData),
  search: (params) => api.get('/api/calls/search', { params }),
  getStats: () => api.get('/api/calls/stats'),
};

export const lookupApi = {
  getAgents: () => api.get('/api/lookup/agents'),
  getCustomers: () => api.get('/api/lookup/customers'),
};

export default api;