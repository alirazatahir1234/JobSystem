import axios from 'axios';
import { 
  Job, 
  JobSearchCriteria, 
  JobSearchResponse, 
  JobApplication, 
  CreateApplicationRequest,
  AuthResult,
  RegisterRequest,
  LoginRequest,
  ChangePasswordRequest,
  UserProfile
} from '../types';

const API_BASE_URL = process.env.REACT_APP_API_URL || 'https://localhost:7001/api';

// Create axios instance
const api = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Request interceptor to add auth token
api.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('token');
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

// Response interceptor to handle auth errors
api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      localStorage.removeItem('token');
      window.location.href = '/login';
    }
    return Promise.reject(error);
  }
);

// Auth API
export const authAPI = {
  register: async (data: RegisterRequest): Promise<AuthResult> => {
    const response = await api.post('/auth/register', data);
    return response.data;
  },

  login: async (data: LoginRequest): Promise<AuthResult> => {
    const response = await api.post('/auth/login', data);
    return response.data;
  },

  changePassword: async (data: ChangePasswordRequest): Promise<void> => {
    await api.post('/auth/change-password', data);
  },
};

// Jobs API
export const jobsAPI = {
  searchJobs: async (criteria: JobSearchCriteria): Promise<JobSearchResponse> => {
    const params = new URLSearchParams();
    
    Object.entries(criteria).forEach(([key, value]) => {
      if (value !== undefined && value !== null && value !== '') {
        if (Array.isArray(value)) {
          value.forEach((item) => params.append(key, item.toString()));
        } else {
          params.append(key, value.toString());
        }
      }
    });

    const response = await api.get(`/jobs?${params.toString()}`);
    return response.data;
  },

  getJob: async (id: number): Promise<Job> => {
    const response = await api.get(`/jobs/${id}`);
    return response.data;
  },

  getRecommendedJobs: async (): Promise<Job[]> => {
    const response = await api.get('/jobs/recommended');
    return response.data;
  },

  getTechnologies: async (): Promise<string[]> => {
    const response = await api.get('/jobs/technologies');
    return response.data;
  },

  getLocations: async (): Promise<string[]> => {
    const response = await api.get('/jobs/locations');
    return response.data;
  },

  getExperienceLevels: async (): Promise<string[]> => {
    const response = await api.get('/jobs/experience-levels');
    return response.data;
  },

  getJobTypes: async (): Promise<string[]> => {
    const response = await api.get('/jobs/job-types');
    return response.data;
  },
};

// Applications API
export const applicationsAPI = {
  getUserApplications: async (): Promise<JobApplication[]> => {
    const response = await api.get('/applications');
    return response.data;
  },

  getApplication: async (id: number): Promise<JobApplication> => {
    const response = await api.get(`/applications/${id}`);
    return response.data;
  },

  applyForJob: async (data: CreateApplicationRequest): Promise<void> => {
    await api.post('/applications', data);
  },

  updateApplicationStatus: async (id: number, status: string): Promise<void> => {
    await api.put(`/applications/${id}/status`, { status });
  },

  withdrawApplication: async (id: number): Promise<void> => {
    await api.delete(`/applications/${id}`);
  },

  getApplicationStats: async (): Promise<Record<string, number>> => {
    const response = await api.get('/applications/stats');
    return response.data;
  },
};

// Profile API
export const profileAPI = {
  getProfile: async (): Promise<UserProfile> => {
    const response = await api.get('/profile');
    return response.data;
  },

  updateProfile: async (data: Partial<UserProfile>): Promise<UserProfile> => {
    const response = await api.put('/profile', data);
    return response.data;
  },
};

// Resume API
export const resumeAPI = {
  generateResume: async (jobId?: number): Promise<string> => {
    const url = jobId ? `/resume/generate?jobId=${jobId}` : '/resume/generate';
    const response = await api.get(url);
    return response.data;
  },

  getAvailableTemplates: async (): Promise<string[]> => {
    const response = await api.get('/resume/templates');
    return response.data;
  },
};

export default api;