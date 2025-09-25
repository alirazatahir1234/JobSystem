@echo off
echo 🚀 Setting up .NET Job System for UAE...

REM Check if Docker is installed
docker --version >nul 2>&1
if %errorlevel% neq 0 (
    echo ❌ Docker is not installed. Please install Docker Desktop first.
    pause
    exit /b 1
)

docker-compose --version >nul 2>&1
if %errorlevel% neq 0 (
    echo ❌ Docker Compose is not installed. Please install Docker Desktop with Compose.
    pause
    exit /b 1
)

REM Create necessary directories
echo 📁 Creating directories...
if not exist "backend\logs" mkdir backend\logs
if not exist "frontend\build" mkdir frontend\build

REM Build and start the services
echo 🐳 Building and starting Docker containers...
docker-compose -f docker-compose.dev.yml up --build -d

REM Wait for database to be ready
echo ⏳ Waiting for database to be ready...
timeout /t 30 /nobreak >nul

REM Run database migrations
echo 🗄️ Running database migrations...
docker-compose -f docker-compose.dev.yml exec backend dotnet ef database update

echo ✅ Setup complete!
echo.
echo 🌐 Services are now running:
echo    • Backend API: http://localhost:5000
echo    • Frontend: http://localhost:3000
echo    • Hangfire Dashboard: http://localhost:5000/hangfire
echo    • SQL Server: localhost:1433
echo    • Redis: localhost:6379
echo.
echo 🔧 To stop services: docker-compose -f docker-compose.dev.yml down
echo 📊 To view logs: docker-compose -f docker-compose.dev.yml logs -f
pause