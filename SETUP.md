# .NET Job System - Development Setup Guide

## Quick Start

### Option 1: Using Setup Scripts (Recommended)

**For macOS/Linux:**
```bash
./setup.sh
```

**For Windows:**
```cmd
setup.bat
```

### Option 2: Manual Setup

1. **Prerequisites:**
   - Docker Desktop
   - .NET 8.0 SDK
   - Node.js 18+
   - SQL Server (or use Docker container)

2. **Backend Setup:**
   ```bash
   cd backend/JobSystem.Api
   dotnet restore
   dotnet ef database update
   dotnet run
   ```

3. **Frontend Setup:**
   ```bash
   cd frontend
   npm install
   npm start
   ```

## Configuration

### Database Connection
Update `appsettings.json` with your SQL Server connection string:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=JobSystemDb;Trusted_Connection=true;"
  }
}
```

### Environment Variables
Create `.env` file in frontend directory:
```
REACT_APP_API_URL=https://localhost:7001/api
```

## Features

### ✅ Job Search & Filtering
- Search jobs by keywords, location, salary
- Filter by technology stack (.NET, C#, etc.)
- Filter by Emirates (Dubai, Abu Dhabi, etc.)
- Sort by date, relevance, salary

### ✅ Job Sources Integration
- **Bayt.com** - UAE's leading job portal
- **Dubizzle Jobs** - Popular classifieds platform
- **GulfTalent** - Regional job portal
- **NaukriGulf** - Gulf-focused job site
- **Indeed UAE** & **LinkedIn UAE** (planned)

### ✅ Smart Job Recommendations
- AI-powered job matching based on user profile
- Skills-based recommendations
- Experience level matching
- Location preferences

### ✅ Application Tracking
- Track application status
- Interview scheduling
- Application statistics
- Notes and follow-ups

### ✅ Resume Generation
- Auto-generate tailored resumes
- Multiple professional templates
- Job-specific customization
- PDF export (planned)

### ✅ User Profile Management
- Skills and experience tracking
- Education and certifications
- Portfolio and social links
- Professional summary

## API Endpoints

### Authentication
- `POST /api/auth/register` - User registration
- `POST /api/auth/login` - User login
- `POST /api/auth/change-password` - Change password

### Jobs
- `GET /api/jobs` - Search jobs
- `GET /api/jobs/{id}` - Get job details
- `GET /api/jobs/recommended` - Get recommended jobs
- `GET /api/jobs/technologies` - Get available technologies
- `GET /api/jobs/locations` - Get UAE locations

### Applications
- `GET /api/applications` - Get user applications
- `POST /api/applications` - Apply for job
- `PUT /api/applications/{id}/status` - Update application status
- `DELETE /api/applications/{id}` - Withdraw application

## UAE Job Market Focus

### Targeted Locations
- **Dubai**: Marina, Downtown, DIFC, Internet City
- **Abu Dhabi**: City center, Corniche, Yas Island
- **Sharjah**: City center, Al Majaz
- **Other Emirates**: Ajman, RAK, Fujairah

### .NET Technology Stack
- ASP.NET Core / .NET 8
- C# Programming
- Entity Framework Core
- Web API Development
- Blazor Applications
- Microservices Architecture
- Azure Cloud Services
- SQL Server / PostgreSQL

### Salary Ranges (AED)
- **Junior**: 8,000 - 15,000 AED/month
- **Mid-Level**: 15,000 - 25,000 AED/month  
- **Senior**: 25,000 - 40,000 AED/month
- **Lead/Architect**: 40,000+ AED/month

## Background Services

### Job Scraping
- Runs hourly via Hangfire
- Scrapes latest .NET developer jobs
- Deduplicates existing entries
- Updates job status and availability

### Monitoring
- Hangfire Dashboard: `http://localhost:5000/hangfire`
- Application logs in `backend/logs/`
- Health checks and performance monitoring

## Security Features

- JWT token authentication
- Password hashing with BCrypt
- Input validation and sanitization
- SQL injection prevention
- CORS configuration
- Rate limiting (planned)

## Deployment

### Docker Production
```bash
docker-compose up -d
```

### Azure Deployment
- Azure App Service for backend
- Azure Static Web Apps for frontend
- Azure SQL Database
- Azure Redis Cache
- Azure Container Registry

## Development Tools

- **Backend**: Visual Studio / VS Code, Postman
- **Frontend**: VS Code, React DevTools
- **Database**: SQL Server Management Studio, Azure Data Studio
- **Monitoring**: Hangfire Dashboard, Application Insights

## Contributing

1. Fork the repository
2. Create feature branch: `git checkout -b feature/amazing-feature`
3. Commit changes: `git commit -m 'Add amazing feature'`
4. Push to branch: `git push origin feature/amazing-feature`
5. Open Pull Request

## Support

For issues and questions:
- Create GitHub Issues
- Check documentation in `/docs` folder
- Review API documentation at `/swagger`

---

**Built specifically for .NET developers seeking opportunities in the UAE job market! 🇦🇪**