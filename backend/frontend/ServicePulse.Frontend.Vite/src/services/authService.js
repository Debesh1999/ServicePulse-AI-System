import axios from 'axios';

const TOKEN_KEY = 'servicepulse_token';
const USER_KEY = 'servicepulse_user';

export const authService = {
  getToken() {
    return localStorage.getItem(TOKEN_KEY);
  },

  getUser() {
    const userStr = localStorage.getItem(USER_KEY);
    try {
      return userStr ? JSON.parse(userStr) : null;
    } catch {
      return null;
    }
  },

  isAuthenticated() {
    return !!this.getToken();
  },

  setAuth(token, user) {
    localStorage.setItem(TOKEN_KEY, token);
    localStorage.setItem(USER_KEY, JSON.stringify(user));
  },

  clear() {
    localStorage.removeItem(TOKEN_KEY);
    localStorage.removeItem(USER_KEY);
  },

  async login(credentials) {
    const api = axios.create({
      baseURL: '/',
      headers: { 'Content-Type': 'application/json' },
    });
    const response = await api.post('/api/auth/login', credentials);
    const { token, user } = response.data;
    this.setAuth(token, user);
    return response.data;
  },

  async register(userData) {
    const api = axios.create({
      baseURL: '/',
      headers: { 'Content-Type': 'application/json' },
    });
    const response = await api.post('/api/auth/register', userData);
    const { token, user } = response.data;
    this.setAuth(token, user);
    return response.data;
  },

  logout() {
    this.clear();
  },
};