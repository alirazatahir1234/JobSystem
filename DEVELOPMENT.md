# Job System Development Commands

## Backend Development

### Run the API locally
```bash
cd backend/JobSystem.Api
dotnet restore
dotnet run
```

### Create and run migrations
```bash
cd backend/JobSystem.Api
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### Run tests
```bash
cd backend/JobSystem.Api.Tests
dotnet test
```

### Build for production
```bash
cd backend/JobSystem.Api
dotnet publish -c Release -o ./publish
```

## Frontend Development

### Install dependencies and start
```bash
cd frontend
npm install
npm start
```

### Build for production
```bash
cd frontend
npm run build
```

### Run tests
```bash
cd frontend
npm test
```

## Docker Commands

### Development environment
```bash
# Start development services
docker-compose -f docker-compose.dev.yml up -d

# View logs
docker-compose -f docker-compose.dev.yml logs -f

# Stop services
docker-compose -f docker-compose.dev.yml down
```

### Production environment
```bash
# Start production services
docker-compose up -d

# Scale services
docker-compose up -d --scale backend=3

# View logs
docker-compose logs -f backend
```

## Database Management

### Connect to database
```bash
# Using Docker
docker-compose exec database sqlcmd -S localhost -U sa -P "YourStrong@Passw0rd"

# Local SQL Server
sqlcmd -S (localdb)\MSSQLLocalDB -d JobSystemDb
```

### Backup database
```bash
docker-compose exec database /opt/mssql-tools/bin/sqlcmd \
  -S localhost -U sa -P "YourStrong@Passw0rd" \
  -Q "BACKUP DATABASE JobSystemDb TO DISK = '/var/opt/mssql/backup/JobSystem.bak'"
```

## Job Scraping

### Manual trigger job scraping
```bash
# Call the API endpoint
curl -X POST http://localhost:5000/api/admin/scrape-jobs

# Or use Hangfire dashboard
# Navigate to http://localhost:5000/hangfire
```

### Monitor background jobs
- Open Hangfire Dashboard: http://localhost:5000/hangfire
- Check recurring jobs status
- View failed jobs and retry them

## Useful URLs

- **Backend API**: http://localhost:5000
- **Frontend App**: http://localhost:3000
- **API Documentation**: http://localhost:5000/swagger
- **Hangfire Dashboard**: http://localhost:5000/hangfire
- **Health Checks**: http://localhost:5000/health

## Environment Variables

### Backend (.NET)
```bash
export ASPNETCORE_ENVIRONMENT=Development
export ConnectionStrings__DefaultConnection="Server=localhost;Database=JobSystemDb;Trusted_Connection=true;"
export JWT__Secret="YourSuperSecretKeyThatIsAtLeast32CharactersLong!!!"
```

### Frontend (React)
```bash
export REACT_APP_API_URL=http://localhost:5000/api
export REACT_APP_ENVIRONMENT=development
```

## Performance Monitoring

### Check API performance
```bash
# Using curl with timing
curl -w "@curl-format.txt" -o /dev/null -s http://localhost:5000/api/jobs

# Using Apache Bench
ab -n 100 -c 10 http://localhost:5000/api/jobs
```

### Monitor database performance
```sql
-- Check slow queries
SELECT TOP 10 
    query_stats.query_hash,
    SUM(query_stats.total_worker_time) / SUM(query_stats.execution_count) AS "Avg CPU Time",
    MIN(query_stats.statement_text) AS "Statement Text"
FROM 
    (SELECT QS.*, 
        SUBSTRING(ST.text, (QS.statement_start_offset/2) + 1,
        ((CASE statement_end_offset 
            WHEN -1 THEN DATALENGTH(ST.text)
            ELSE QS.statement_end_offset END 
            - QS.statement_start_offset)/2) + 1) AS statement_text
     FROM sys.dm_exec_query_stats AS QS
     CROSS APPLY sys.dm_exec_sql_text(QS.sql_handle) as ST) as query_stats
GROUP BY query_stats.query_hash
ORDER BY 2 DESC;
```

## Troubleshooting

### Common Issues

1. **Database connection failed**
   ```bash
   # Check if SQL Server container is running
   docker-compose ps database
   
   # Restart database container
   docker-compose restart database
   ```

2. **Frontend can't connect to API**
   ```bash
   # Check if API is running
   curl http://localhost:5000/api/jobs
   
   # Check CORS settings in Program.cs
   ```

3. **Job scraping not working**
   ```bash
   # Check Hangfire dashboard for errors
   # Verify external job sites are accessible
   curl -I https://www.bayt.com
   ```

4. **High memory usage**
   ```bash
   # Monitor container resources
   docker stats
   
   # Check for memory leaks in logs
   docker-compose logs backend | grep -i memory
   ```