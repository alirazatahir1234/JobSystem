#!/bin/bash

echo "🚀 Setting up .NET Job System for UAE..."

# Check if Docker is installed
if ! command -v docker &> /dev/null; then
    echo "❌ Docker is not installed. Please install Docker first."
    exit 1
fi

if ! command -v docker-compose &> /dev/null; then
    echo "❌ Docker Compose is not installed. Please install Docker Compose first."
    exit 1
fi

# Create necessary directories
echo "📁 Creating directories..."
mkdir -p backend/logs
mkdir -p frontend/build

# Build and start the services
echo "🐳 Building and starting Docker containers..."
docker-compose -f docker-compose.dev.yml up --build -d

# Wait for database to be ready
echo "⏳ Waiting for database to be ready..."
sleep 30

# Run database migrations
echo "🗄️ Running database migrations..."
docker-compose -f docker-compose.dev.yml exec backend dotnet ef database update

# Seed initial data (if migration exists)
echo "🌱 Seeding initial data..."
# You can add seed data commands here

echo "✅ Setup complete!"
echo ""
echo "🌐 Services are now running:"
echo "   • Backend API: http://localhost:5000"
echo "   • Frontend: http://localhost:3000"
echo "   • Hangfire Dashboard: http://localhost:5000/hangfire"
echo "   • SQL Server: localhost:1433"
echo "   • Redis: localhost:6379"
echo ""
echo "🔧 To stop services: docker-compose -f docker-compose.dev.yml down"
echo "📊 To view logs: docker-compose -f docker-compose.dev.yml logs -f"