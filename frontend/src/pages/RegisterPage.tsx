import React from 'react';
import { Container, Typography, Paper, Box } from '@mui/material';

const RegisterPage: React.FC = () => {
  return (
    <Container maxWidth="sm">
      <Paper elevation={3} sx={{ p: 4, mt: 4 }}>
        <Typography variant="h4" component="h1" gutterBottom align="center">
          Register
        </Typography>
        <Box sx={{ textAlign: 'center', mt: 4 }}>
          <Typography variant="body1" color="text.secondary">
            Registration functionality will be implemented in the next iteration.
          </Typography>
        </Box>
      </Paper>
    </Container>
  );
};

export default RegisterPage;