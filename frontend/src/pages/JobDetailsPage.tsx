import React from 'react';
import { useParams } from 'react-router-dom';
import {
  Container,
  Typography,
  Paper,
  Box,
  Button,
  Chip,
  Divider,
  Grid,
} from '@mui/material';
import { LocationOn, Work, AttachMoney, Business, CalendarToday } from '@mui/icons-material';

const JobDetailsPage: React.FC = () => {
  const { id } = useParams<{ id: string }>();

  // Mock job data - in real app, this would be fetched from API
  const job = {
    id: 1,
    title: 'Senior .NET Developer',
    company: 'Emirates NBD',
    location: 'Dubai International Financial Centre',
    emirate: 'Dubai',
    salaryMin: 25000,
    salaryMax: 35000,
    currency: 'AED',
    experienceLevel: 'Senior',
    jobType: 'Full-time',
    description: 'Looking for an experienced .NET developer to join our digital banking team. You will work on mission-critical applications serving millions of customers across the UAE.',
    requirements: 'Minimum 5 years experience with .NET Core, C#, ASP.NET MVC, Entity Framework, SQL Server, and Azure cloud services. Experience with microservices architecture preferred.',
    technologies: ['ASP.NET Core', 'C#', 'Entity Framework', 'SQL Server', 'Azure', 'Microservices'],
    benefits: ['Health Insurance', 'Annual Leave', 'Performance Bonus', 'Training Budget'],
    postedDate: '2025-09-20',
    externalUrl: 'https://careers.emiratesnbd.com',
  };

  return (
    <Container maxWidth="lg">
      <Paper elevation={3} sx={{ p: 4 }}>
        <Box sx={{ mb: 3 }}>
          <Typography variant="h4" component="h1" gutterBottom>
            {job.title}
          </Typography>
          <Typography variant="h5" color="primary" gutterBottom>
            {job.company}
          </Typography>
          
          <Grid container spacing={2} sx={{ mb: 3 }}>
            <Grid item xs={12} sm={6} md={3}>
              <Box sx={{ display: 'flex', alignItems: 'center' }}>
                <LocationOn sx={{ mr: 1, color: 'text.secondary' }} />
                <Typography variant="body1">
                  {job.location}, {job.emirate}
                </Typography>
              </Box>
            </Grid>
            
            <Grid item xs={12} sm={6} md={3}>
              <Box sx={{ display: 'flex', alignItems: 'center' }}>
                <AttachMoney sx={{ mr: 1, color: 'text.secondary' }} />
                <Typography variant="body1">
                  {job.salaryMin.toLocaleString()} - {job.salaryMax.toLocaleString()} {job.currency}
                </Typography>
              </Box>
            </Grid>
            
            <Grid item xs={12} sm={6} md={3}>
              <Box sx={{ display: 'flex', alignItems: 'center' }}>
                <Work sx={{ mr: 1, color: 'text.secondary' }} />
                <Typography variant="body1">
                  {job.experienceLevel} • {job.jobType}
                </Typography>
              </Box>
            </Grid>
            
            <Grid item xs={12} sm={6} md={3}>
              <Box sx={{ display: 'flex', alignItems: 'center' }}>
                <CalendarToday sx={{ mr: 1, color: 'text.secondary' }} />
                <Typography variant="body1">
                  Posted {job.postedDate}
                </Typography>
              </Box>
            </Grid>
          </Grid>
          
          <Box sx={{ mb: 3 }}>
            <Button
              variant="contained"
              size="large"
              sx={{ mr: 2, mb: 2 }}
            >
              Apply Now
            </Button>
            <Button
              variant="outlined"
              size="large"
              sx={{ mb: 2 }}
            >
              Save Job
            </Button>
          </Box>
        </Box>

        <Divider sx={{ mb: 3 }} />

        <Box sx={{ mb: 4 }}>
          <Typography variant="h6" gutterBottom>
            Job Description
          </Typography>
          <Typography variant="body1" paragraph>
            {job.description}
          </Typography>
        </Box>

        <Box sx={{ mb: 4 }}>
          <Typography variant="h6" gutterBottom>
            Requirements
          </Typography>
          <Typography variant="body1" paragraph>
            {job.requirements}
          </Typography>
        </Box>

        <Box sx={{ mb: 4 }}>
          <Typography variant="h6" gutterBottom>
            Required Technologies
          </Typography>
          <Box>
            {job.technologies.map((tech, index) => (
              <Chip
                key={index}
                label={tech}
                sx={{ mr: 1, mb: 1 }}
                color="primary"
                variant="outlined"
              />
            ))}
          </Box>
        </Box>

        <Box sx={{ mb: 4 }}>
          <Typography variant="h6" gutterBottom>
            Benefits
          </Typography>
          <Box>
            {job.benefits.map((benefit, index) => (
              <Chip
                key={index}
                label={benefit}
                sx={{ mr: 1, mb: 1 }}
                color="secondary"
              />
            ))}
          </Box>
        </Box>

        <Divider sx={{ mb: 3 }} />

        <Box sx={{ textAlign: 'center' }}>
          <Button
            variant="contained"
            size="large"
            sx={{ mr: 2 }}
          >
            Apply on Company Website
          </Button>
          <Button
            variant="outlined"
            size="large"
            onClick={() => window.history.back()}
          >
            Back to Search
          </Button>
        </Box>
      </Paper>
    </Container>
  );
};

export default JobDetailsPage;