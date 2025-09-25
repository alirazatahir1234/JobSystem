import React from 'react';
import { 
  Container, 
  Typography, 
  Box, 
  Button, 
  Grid, 
  Card, 
  CardContent,
  Chip,
  Paper
} from '@mui/material';
import { Link, useNavigate } from 'react-router-dom';
import { Work, LocationOn, TrendingUp, Speed } from '@mui/icons-material';
import { useQuery } from 'react-query';
import { jobsAPI } from '../services/api';
import { useAuth } from '../contexts/AuthContext';

const HomePage: React.FC = () => {
  const navigate = useNavigate();
  const { isAuthenticated } = useAuth();

  const { data: recentJobs } = useQuery(
    'recent-jobs',
    () => jobsAPI.searchJobs({
      keywords: '.net',
      page: 1,
      pageSize: 6,
      sortBy: 'PostedDate',
      sortOrder: 'desc',
      technologies: []
    }),
    {
      select: (data) => data.jobs
    }
  );

  const stats = [
    { icon: <Work />, label: 'Active Jobs', value: '500+', color: '#1976d2' },
    { icon: <LocationOn />, label: 'UAE Cities', value: '7', color: '#dc004e' },
    { icon: <TrendingUp />, label: 'Success Rate', value: '85%', color: '#2e7d2e' },
    { icon: <Speed />, label: 'Avg Response', value: '24h', color: '#ed6c02' },
  ];

  return (
    <Box>
      {/* Hero Section */}
      <Box 
        sx={{ 
          background: 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)',
          color: 'white',
          py: 8,
          mb: 6
        }}
      >
        <Container maxWidth="lg">
          <Box textAlign="center">
            <Typography variant="h2" component="h1" gutterBottom fontWeight="bold">
              Find Your Dream .NET Developer Job in UAE
            </Typography>
            <Typography variant="h5" component="p" sx={{ mb: 4, opacity: 0.9 }}>
              Connect with top employers across Dubai, Abu Dhabi, and beyond
            </Typography>
            <Box sx={{ display: 'flex', gap: 2, justifyContent: 'center', flexWrap: 'wrap' }}>
              <Button 
                variant="contained" 
                size="large" 
                onClick={() => navigate('/jobs')}
                sx={{ bgcolor: 'white', color: 'primary.main', '&:hover': { bgcolor: 'grey.100' } }}
              >
                Search Jobs
              </Button>
              {!isAuthenticated && (
                <Button 
                  variant="outlined" 
                  size="large" 
                  onClick={() => navigate('/register')}
                  sx={{ color: 'white', borderColor: 'white', '&:hover': { borderColor: 'grey.300' } }}
                >
                  Create Account
                </Button>
              )}
            </Box>
          </Box>
        </Container>
      </Box>

      <Container maxWidth="lg">
        {/* Stats Section */}
        <Grid container spacing={4} sx={{ mb: 6 }}>
          {stats.map((stat, index) => (
            <Grid item xs={12} sm={6} md={3} key={index}>
              <Paper 
                elevation={2} 
                sx={{ p: 3, textAlign: 'center', height: '100%' }}
              >
                <Box sx={{ color: stat.color, mb: 2 }}>
                  {React.cloneElement(stat.icon, { fontSize: 'large' })}
                </Box>
                <Typography variant="h4" component="div" fontWeight="bold" gutterBottom>
                  {stat.value}
                </Typography>
                <Typography variant="body1" color="text.secondary">
                  {stat.label}
                </Typography>
              </Paper>
            </Grid>
          ))}
        </Grid>

        {/* Featured Technologies */}
        <Box sx={{ mb: 6, textAlign: 'center' }}>
          <Typography variant="h4" component="h2" gutterBottom>
            Popular .NET Technologies
          </Typography>
          <Typography variant="body1" color="text.secondary" sx={{ mb: 3 }}>
            Find opportunities with the latest .NET stack
          </Typography>
          <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 1, justifyContent: 'center' }}>
            {[
              '.NET 8', 'C#', 'ASP.NET Core', 'Entity Framework', 'Blazor', 'Web API',
              'Azure', 'SQL Server', 'React', 'Angular', 'Docker', 'Microservices'
            ].map((tech) => (
              <Chip 
                key={tech} 
                label={tech} 
                variant="outlined" 
                sx={{ fontSize: '0.9rem' }}
                onClick={() => navigate(`/jobs?technologies=${encodeURIComponent(tech)}`)}
              />
            ))}
          </Box>
        </Box>

        {/* Recent Jobs */}
        <Box sx={{ mb: 6 }}>
          <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 3 }}>
            <Typography variant="h4" component="h2">
              Latest .NET Jobs
            </Typography>
            <Button component={Link} to="/jobs" variant="outlined">
              View All Jobs
            </Button>
          </Box>
          
          <Grid container spacing={3}>
            {recentJobs?.slice(0, 6).map((job) => (
              <Grid item xs={12} md={6} key={job.id}>
                <Card 
                  sx={{ 
                    height: '100%', 
                    cursor: 'pointer',
                    '&:hover': { boxShadow: 4 }
                  }}
                  onClick={() => navigate(`/jobs/${job.id}`)}
                >
                  <CardContent>
                    <Typography variant="h6" component="h3" gutterBottom>
                      {job.title}
                    </Typography>
                    <Typography variant="subtitle1" color="primary" gutterBottom>
                      {job.company}
                    </Typography>
                    <Typography variant="body2" color="text.secondary" gutterBottom>
                      <LocationOn fontSize="small" sx={{ mr: 0.5, verticalAlign: 'middle' }} />
                      {job.location}, {job.emirate}
                    </Typography>
                    {job.technologies.length > 0 && (
                      <Box sx={{ mt: 2 }}>
                        {job.technologies.slice(0, 3).map((tech) => (
                          <Chip 
                            key={tech} 
                            label={tech} 
                            size="small" 
                            sx={{ mr: 0.5, mb: 0.5 }} 
                          />
                        ))}
                        {job.technologies.length > 3 && (
                          <Chip 
                            label={`+${job.technologies.length - 3} more`} 
                            size="small" 
                            variant="outlined"
                          />
                        )}
                      </Box>
                    )}
                    {(job.salaryMin || job.salaryMax) && (
                      <Typography variant="body2" color="text.secondary" sx={{ mt: 1 }}>
                        {job.salaryMin && job.salaryMax 
                          ? `${job.currency} ${job.salaryMin.toLocaleString()} - ${job.salaryMax.toLocaleString()}`
                          : job.salaryMax 
                            ? `Up to ${job.currency} ${job.salaryMax.toLocaleString()}`
                            : `From ${job.currency} ${job.salaryMin?.toLocaleString()}`
                        }
                      </Typography>
                    )}
                  </CardContent>
                </Card>
              </Grid>
            ))}
          </Grid>
        </Box>

        {/* CTA Section */}
        <Paper 
          sx={{ 
            p: 4, 
            textAlign: 'center', 
            background: 'linear-gradient(135deg, #f5f7fa 0%, #c3cfe2 100%)',
            mb: 4
          }}
        >
          <Typography variant="h4" component="h2" gutterBottom>
            Ready to Start Your .NET Career in UAE?
          </Typography>
          <Typography variant="body1" color="text.secondary" sx={{ mb: 3 }}>
            Join thousands of developers who found their perfect job through our platform
          </Typography>
          <Box sx={{ display: 'flex', gap: 2, justifyContent: 'center', flexWrap: 'wrap' }}>
            <Button 
              variant="contained" 
              size="large" 
              onClick={() => navigate('/jobs')}
            >
              Browse Jobs
            </Button>
            {!isAuthenticated && (
              <Button 
                variant="outlined" 
                size="large" 
                onClick={() => navigate('/register')}
              >
                Sign Up Free
              </Button>
            )}
          </Box>
        </Paper>
      </Container>
    </Box>
  );
};

export default HomePage;