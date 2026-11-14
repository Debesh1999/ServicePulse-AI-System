import axios from 'axios';
import { authService } from './authService';

// Use relative URL - the React dev server proxy will forward /api requests to the backend
const api = axios.create({
  baseURL: '/',
  headers: {
    'Content-Type': 'application/json',
  },
});

// Attach the JWT bearer token to every request and redirect to login on 401
api.interceptors.request.use(
  (config) => {
    const token = authService.getToken();
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => Promise.reject(error)
);

api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response && error.response.status === 401) {
      authService.clear();
      if (window.location.pathname !== '/login') {
        window.location.href = '/login';
      }
    }
    return Promise.reject(error);
  }
);

export const callsApi = {
  getAll: () => api.get('/api/calls'),
  getById: (id) => api.get(`/api/calls/${id}`),
  create: (callData) => api.post('/api/calls', callData),
  search: (params) => api.get('/api/calls/search', { params }),
  getStats: () => api.get('/api/calls/stats'),
};

export const managerReviewsApi = {
  create: (reviewData) => api.post('/api/ManagerReviews', reviewData),
  getById: (id) => api.get(`/api/ManagerReviews/${id}`),
  getForCall: (callId) => api.get(`/api/ManagerReviews/call/${callId}`),
  getLatestForCall: (callId) => api.get(`/api/ManagerReviews/call/${callId}/latest`),
  update: (id, reviewData) => api.put(`/api/ManagerReviews/${id}`, reviewData),
};

export const lookupApi = {
  getAgents: () => api.get('/api/lookup/agents'),
  getCustomers: () => api.get('/api/lookup/customers'),
};

export default api;