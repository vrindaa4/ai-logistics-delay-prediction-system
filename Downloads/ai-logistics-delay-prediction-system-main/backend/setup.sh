#!/bin/zsh

# AI Logistics Delay Prediction System - Backend Setup Script

echo "🚀 Starting backend setup..."

# Navigate to backend directory
cd backend/LogisticsAPI

# Check if dotnet is installed
if ! command -v dotnet &> /dev/null; then
    echo "❌ .NET SDK is not installed. Please install it from https://dotnet.microsoft.com/download"
    exit 1
fi

echo "✅ .NET SDK found: $(dotnet --version)"

# Restore packages
echo "📦 Restoring NuGet packages..."
dotnet restore

if [ $? -ne 0 ]; then
    echo "❌ Failed to restore packages"
    exit 1
fi

echo "✅ Packages restored successfully"

# Build the project
echo "🔨 Building project..."
dotnet build

if [ $? -ne 0 ]; then
    echo "❌ Build failed"
    exit 1
fi

echo "✅ Project built successfully"

# Check if database connection string is configured
if ! grep -q "DefaultConnection" appsettings.json; then
    echo "⚠️  Database connection string not configured in appsettings.json"
    echo "   Please update the connection string manually before running migrations"
fi

# Run migrations
echo "🗄️  Applying database migrations..."
dotnet ef database update

if [ $? -ne 0 ]; then
    echo "⚠️  Migration failed. Make sure your database connection is configured correctly."
    echo "   Update appsettings.json with your database connection string"
else
    echo "✅ Database migrations applied successfully"
fi

echo ""
echo "✅ Setup complete! You can now run the application with:"
echo "   dotnet run"
echo ""
echo "📚 Access Swagger documentation at:"
echo "   https://localhost:7xxx/swagger"
