import React from 'react';
import { Container, Typography, Paper, Box, Grid, Card, CardContent } from '@mui/material';
import { Work, Assignment, Person, TrendingUp } from '@mui/icons-material';

const DashboardPage: React.FC = () => {
  return (
    <Container maxWidth="lg">
      <Typography variant="h4" component="h1" gutterBottom>
        Dashboard
      </Typography>
      
      <Grid container spacing={3} sx={{ mt: 2 }}>
        <Grid item xs={12} sm={6} md={3}>
          <Card>
            <CardContent sx={{ textAlign: 'center' }}>
              <Work sx={{ fontSize: 40, color: 'primary.main', mb: 2 }} />
              <Typography variant="h6">Applied Jobs</Typography>
              <Typography variant="h4" color="primary">
                12
              </Typography>
            </CardContent>
          </Card>
        </Grid>
        
        <Grid item xs={12} sm={6} md={3}>
          <Card>
            <CardContent sx={{ textAlign: 'center' }}>
              <Assignment sx={{ fontSize: 40, color: 'secondary.main', mb: 2 }} />
              <Typography variant="h6">Interviews</Typography>
              <Typography variant="h4" color="secondary">
                3
              </Typography>
            </CardContent>
          </Card>
        </Grid>
        
        <Grid item xs={12} sm={6} md={3}>
          <Card>
            <CardContent sx={{ textAlign: 'center' }}>
              <Person sx={{ fontSize: 40, color: 'success.main', mb: 2 }} />
              <Typography variant="h6">Profile Views</Typography>
              <Typography variant="h4" color="success.main">
                28
              </Typography>
            </CardContent>
          </Card>
        </Grid>
        
        <Grid item xs={12} sm={6} md={3}>
          <Card>
            <CardContent sx={{ textAlign: 'center' }}>
              <TrendingUp sx={{ fontSize: 40, color: 'warning.main', mb: 2 }} />
              <Typography variant="h6">Success Rate</Typography>
              <Typography variant="h4" color="warning.main">
                25%
              </Typography>
            </CardContent>
          </Card>
        </Grid>
      </Grid>
      
      <Box sx={{ mt: 4 }}>
        <Typography variant="body1" color="text.secondary" align="center">
          Detailed dashboard functionality will be implemented in the next iteration.
        </Typography>
      </Box>
    </Container>
  );
};

export default DashboardPage;