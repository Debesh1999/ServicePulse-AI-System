import axios from 'axios';

const TOKEN_KEY = 'servicepulse_token';
const USER_KEY = 'servicepulse_user';

export const authService = {
  getToken() {
    return localStorage.getItem(TOKEN_KEY);
  },

  getUser() {
    const raw = localStorage.getItem(USER_KEY);
    return raw ? JSON.parse(raw) : null;
  },

  save(token, user) {
    localStorage.setItem(TOKEN_KEY, token);
    localStorage.setItem(USER_KEY, JSON.stringify(user));
  },

  clear() {
    localStorage.removeItem(TOKEN_KEY);
    localStorage.removeItem(USER_KEY);
  },

  async register(email, password, displayName, role = 'Agent') {
    const response = await axios.post('/api/auth/register', {
      email,
      password,
      displayName,
      role,
    });
    return response.data;
  },

  async login(email, password) {
    const response = await axios.post('/api/auth/login', {
      email,
      password,
    });
    return response.data;
  },

  async me() {
    const response = await axios.get('/api/auth/me', {
      headers: { Authorization: `Bearer ${this.getToken()}` },
    });
    return response.data;
  },
};