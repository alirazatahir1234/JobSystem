// API Types
export interface Job {
  id: number;
  title: string;
  company: string;
  description: string;
  requirements: string;
  location: string;
  emirate: string;
  salaryMin?: number;
  salaryMax?: number;
  currency: string;
  experienceLevel: string;
  jobType: string;
  source: string;
  externalUrl: string;
  technologies: string[];
  benefits: string[];
  isActive: boolean;
  postedDate: string;
  createdAt: string;
  updatedAt: string;
}

export interface JobSearchCriteria {
  keywords?: string;
  location?: string;
  emirate?: string;
  minSalary?: number;
  maxSalary?: number;
  experienceLevel?: string;
  jobType?: string;
  technologies: string[];
  page: number;
  pageSize: number;
  sortBy: string;
  sortOrder: string;
}

export interface JobSearchResponse {
  jobs: Job[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

export interface User {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  createdAt: string;
  updatedAt: string;
}

export interface UserProfile {
  id: number;
  userId: string;
  summary: string;
  skills: string[];
  experience: Experience[];
  education: Education[];
  certifications: string[];
  linkedInUrl: string;
  gitHubUrl: string;
  portfolioUrl: string;
  createdAt: string;
  updatedAt: string;
}

export interface Experience {
  title: string;
  company: string;
  description: string;
  startDate: string;
  endDate?: string;
  isCurrent: boolean;
  technologies: string[];
}

export interface Education {
  degree: string;
  institution: string;
  fieldOfStudy: string;
  startDate: string;
  endDate?: string;
  grade: string;
}

export interface JobApplication {
  id: number;
  userId: string;
  jobId: number;
  status: string;
  coverLetter: string;
  resumeFileName: string;
  appliedDate: string;
  interviewDate?: string;
  notes: string;
  job: Job;
}

export interface CreateApplicationRequest {
  jobId: number;
  coverLetter: string;
  resumeFileName: string;
  notes: string;
}

export interface AuthResult {
  success: boolean;
  token: string;
  user: User;
  message: string;
}

export interface RegisterRequest {
  firstName: string;
  lastName: string;
  email: string;
  password: string;
  confirmPassword: string;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface ChangePasswordRequest {
  currentPassword: string;
  newPassword: string;
  confirmNewPassword: string;
}