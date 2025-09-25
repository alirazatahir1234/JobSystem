import React, { useState } from 'react';
import {
  Container,
  Typography,
  TextField,
  Button,
  Box,
  Grid,
  Card,
  CardContent,
  CardActions,
  Chip,
  FormControl,
  InputLabel,
  Select,
  MenuItem,
  Pagination,
} from '@mui/material';
import { Search, LocationOn, Work, AttachMoney } from '@mui/icons-material';

interface Job {
  id: number;
  title: string;
  company: string;
  location: string;
  emirate: string;
  salaryMin?: number;
  salaryMax?: number;
  currency: string;
  experienceLevel: string;
  jobType: string;
  technologies: string[];
  description: string;
  postedDate: string;
}

const JobSearchPage: React.FC = () => {
  const [searchTerm, setSearchTerm] = useState('');
  const [emirateFilter, setEmirateFilter] = useState('');
  const [experienceFilter, setExperienceFilter] = useState('');
  const [jobs, setJobs] = useState<Job[]>([]);
  const [loading, setLoading] = useState(false);
  const [currentPage, setCurrentPage] = useState(1);

  const handleSearch = async () => {
    setLoading(true);
    try {
      const params = new URLSearchParams();
      if (searchTerm) params.append('search', searchTerm);
      if (emirateFilter) params.append('emirate', emirateFilter);
      if (experienceFilter) params.append('experience', experienceFilter);
      params.append('page', currentPage.toString());
      
      const response = await fetch(`http://localhost:5001/api/jobs?${params}`);
      const data = await response.json();
      setJobs(data.jobs || []);
    } catch (error) {
      console.error('Error fetching jobs:', error);
    } finally {
      setLoading(false);
    }
  };

  React.useEffect(() => {
    handleSearch();
  }, [currentPage]);

  return (
    <Container maxWidth="lg">
      <Typography variant="h4" component="h1" gutterBottom>
        Find .NET Developer Jobs in UAE
      </Typography>

      {/* Search Form */}
      <Box sx={{ mb: 4 }}>
        <Grid container spacing={2} alignItems="center">
          <Grid item xs={12} md={4}>
            <TextField
              fullWidth
              label="Search Jobs"
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              placeholder="e.g. .NET, C#, React"
              InputProps={{
                startAdornment: <Search sx={{ mr: 1, color: 'action.active' }} />,
              }}
            />
          </Grid>
          <Grid item xs={12} md={3}>
            <FormControl fullWidth>
              <InputLabel>Emirate</InputLabel>
              <Select
                value={emirateFilter}
                label="Emirate"
                onChange={(e) => setEmirateFilter(e.target.value)}
              >
                <MenuItem value="">All Emirates</MenuItem>
                <MenuItem value="Dubai">Dubai</MenuItem>
                <MenuItem value="Abu Dhabi">Abu Dhabi</MenuItem>
                <MenuItem value="Sharjah">Sharjah</MenuItem>
                <MenuItem value="Ajman">Ajman</MenuItem>
                <MenuItem value="Ras Al Khaimah">Ras Al Khaimah</MenuItem>
                <MenuItem value="Fujairah">Fujairah</MenuItem>
                <MenuItem value="Umm Al Quwain">Umm Al Quwain</MenuItem>
              </Select>
            </FormControl>
          </Grid>
          <Grid item xs={12} md={3}>
            <FormControl fullWidth>
              <InputLabel>Experience Level</InputLabel>
              <Select
                value={experienceFilter}
                label="Experience Level"
                onChange={(e) => setExperienceFilter(e.target.value)}
              >
                <MenuItem value="">All Levels</MenuItem>
                <MenuItem value="Junior">Junior</MenuItem>
                <MenuItem value="Mid-level">Mid-level</MenuItem>
                <MenuItem value="Senior">Senior</MenuItem>
              </Select>
            </FormControl>
          </Grid>
          <Grid item xs={12} md={2}>
            <Button
              fullWidth
              variant="contained"
              onClick={handleSearch}
              disabled={loading}
              sx={{ height: 56 }}
            >
              {loading ? 'Searching...' : 'Search'}
            </Button>
          </Grid>
        </Grid>
      </Box>

      {/* Job Results */}
      <Grid container spacing={3}>
        {jobs.map((job) => (
          <Grid item xs={12} md={6} key={job.id}>
            <Card>
              <CardContent>
                <Typography variant="h6" component="h2" gutterBottom>
                  {job.title}
                </Typography>
                <Typography variant="subtitle1" color="primary" gutterBottom>
                  {job.company}
                </Typography>
                <Box sx={{ display: 'flex', alignItems: 'center', mb: 1 }}>
                  <LocationOn sx={{ mr: 1, fontSize: 16 }} />
                  <Typography variant="body2">
                    {job.location}, {job.emirate}
                  </Typography>
                </Box>
                {job.salaryMin && job.salaryMax && (
                  <Box sx={{ display: 'flex', alignItems: 'center', mb: 1 }}>
                    <AttachMoney sx={{ mr: 1, fontSize: 16 }} />
                    <Typography variant="body2">
                      {job.salaryMin.toLocaleString()} - {job.salaryMax.toLocaleString()} {job.currency}
                    </Typography>
                  </Box>
                )}
                <Box sx={{ display: 'flex', alignItems: 'center', mb: 2 }}>
                  <Work sx={{ mr: 1, fontSize: 16 }} />
                  <Typography variant="body2">
                    {job.experienceLevel} • {job.jobType}
                  </Typography>
                </Box>
                <Typography variant="body2" color="text.secondary" sx={{ mb: 2 }}>
                  {job.description.substring(0, 150)}...
                </Typography>
                <Box sx={{ mb: 2 }}>
                  {job.technologies.slice(0, 3).map((tech, index) => (
                    <Chip
                      key={index}
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
                      sx={{ mr: 0.5, mb: 0.5 }}
                    />
                  )}
                </Box>
              </CardContent>
              <CardActions>
                <Button size="small" color="primary">
                  View Details
                </Button>
                <Button size="small" variant="contained">
                  Apply Now
                </Button>
              </CardActions>
            </Card>
          </Grid>
        ))}
      </Grid>

      {jobs.length === 0 && !loading && (
        <Box sx={{ textAlign: 'center', mt: 4 }}>
          <Typography variant="h6" color="text.secondary">
            No jobs found. Try adjusting your search criteria.
          </Typography>
        </Box>
      )}

      {jobs.length > 0 && (
        <Box sx={{ display: 'flex', justifyContent: 'center', mt: 4 }}>
          <Pagination
            count={10}
            page={currentPage}
            onChange={(_, page) => setCurrentPage(page)}
            color="primary"
          />
        </Box>
      )}
    </Container>
  );
};

export default JobSearchPage;