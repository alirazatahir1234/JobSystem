# .NET Full Stack Developer Job System - UAE

A comprehensive job search system specifically designed to find .NET Full Stack Developer positions in the UAE.

## Features

- **Job Search Engine**: Search for .NET Full Stack Developer jobs across multiple job boards
- **UAE Focus**: Specifically targets job opportunities in the United Arab Emirates
- **Skill Matching**: Matches jobs based on .NET technologies (ASP.NET Core, C#, Entity Framework, etc.)
- **Web Scraping**: Automated job collection from popular UAE job portals
- **Job Filtering**: Filter by salary, experience level, location within UAE
- **Application Tracking**: Track job applications and their status
- **Resume Builder**: Generate tailored resumes for .NET positions
- **Interview Preparation**: Resources and tips specific to .NET developer interviews

## Tech Stack

- **Backend**: ASP.NET Core Web API
- **Frontend**: React with TypeScript
- **Database**: SQL Server with Entity Framework Core
- **Authentication**: JWT tokens
- **Job Scraping**: HTML Agility Pack, Selenium WebDriver
- **Deployment**: Docker containers

## UAE Job Portals Integrated

- Bayt.com
- Dubizzle Jobs
- GulfTalent
- Naukrigulf
- Indeed UAE
- LinkedIn UAE
- Monster Gulf

## Getting Started

1. Clone the repository
2. Run `dotnet restore` in the backend folder
3. Run `npm install` in the frontend folder
4. Configure database connection in appsettings.json
5. Run database migrations
6. Start the application

## Project Structure

```
JobSystem/
├── backend/              # ASP.NET Core Web API
├── frontend/            # React TypeScript app
├── database/           # SQL scripts and migrations
├── docker/            # Docker configuration
├── docs/              # Documentation
└── tests/             # Unit and integration tests
```

## API Endpoints

- `GET /api/jobs` - Search jobs
- `GET /api/jobs/{id}` - Get job details
- `POST /api/applications` - Submit job application
- `GET /api/applications` - Get user applications
- `POST /api/resume/generate` - Generate tailored resume

## Environment Variables

```
DATABASE_CONNECTION_STRING=your_sql_server_connection
JWT_SECRET=your_jwt_secret
BAYT_API_KEY=your_bayt_api_key
```

## License

MIT License