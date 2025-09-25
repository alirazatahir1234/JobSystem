import React from 'react';
import { Container, Typography, Paper, Box } from '@mui/material';

const ProfilePage: React.FC = () => {
  return (
    <Container maxWidth="md">
      <Paper elevation={3} sx={{ p: 4 }}>
        <Typography variant="h4" component="h1" gutterBottom>
          User Profile
        </Typography>
        <Box sx={{ textAlign: 'center', mt: 4 }}>
          <Typography variant="body1" color="text.secondary">
            Profile management functionality will be implemented in the next iteration.
          </Typography>
        </Box>
      </Paper>
    </Container>
  );
};

export default ProfilePage;