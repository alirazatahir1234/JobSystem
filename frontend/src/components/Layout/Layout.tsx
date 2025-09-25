import React from 'react';
import {
  AppBar,
  Toolbar,
  Typography,
  Container,
  Box,
  Button,
  IconButton,
} from '@mui/material';
import { Home, Work, Person, Dashboard } from '@mui/icons-material';
import { Link, useLocation } from 'react-router-dom';

interface LayoutProps {
  children: React.ReactNode;
}

const Layout: React.FC<LayoutProps> = ({ children }) => {
  const location = useLocation();

  return (
    <Box sx={{ flexGrow: 1 }}>
      <AppBar position="static" sx={{ backgroundColor: '#1976d2' }}>
        <Toolbar>
          <IconButton
            edge="start"
            color="inherit"
            aria-label="home"
            component={Link}
            to="/"
            sx={{ mr: 2 }}
          >
            <Work />
          </IconButton>
          <Typography variant="h6" component="div" sx={{ flexGrow: 1 }}>
            UAE Job System - .NET Developer Jobs
          </Typography>
          
          <Button 
            color="inherit" 
            component={Link} 
            to="/"
            startIcon={<Home />}
            sx={{ mx: 1 }}
          >
            Home
          </Button>
          
          <Button 
            color="inherit" 
            component={Link} 
            to="/jobs"
            startIcon={<Work />}
            sx={{ mx: 1 }}
          >
            Jobs
          </Button>
          
          <Button 
            color="inherit" 
            component={Link} 
            to="/dashboard"
            startIcon={<Dashboard />}
            sx={{ mx: 1 }}
          >
            Dashboard
          </Button>
          
          <Button 
            color="inherit" 
            component={Link} 
            to="/profile"
            startIcon={<Person />}
            sx={{ mx: 1 }}
          >
            Profile
          </Button>
        </Toolbar>
      </AppBar>
      
      <Container maxWidth="lg" sx={{ mt: 4, mb: 4 }}>
        {children}
      </Container>
    </Box>
  );
};

export default Layout;