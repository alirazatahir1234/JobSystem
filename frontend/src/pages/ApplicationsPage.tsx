import React from 'react';
import { Container, Typography, Paper, Box } from '@mui/material';

const ApplicationsPage: React.FC = () => {
  return (
    <Container maxWidth="lg">
      <Paper elevation={3} sx={{ p: 4 }}>
        <Typography variant="h4" component="h1" gutterBottom>
          My Applications
        </Typography>
        <Box sx={{ textAlign: 'center', mt: 4 }}>
          <Typography variant="body1" color="text.secondary">
            Job applications tracking functionality will be implemented in the next iteration.
          </Typography>
        </Box>
      </Paper>
    </Container>
  );
};

export default ApplicationsPage;