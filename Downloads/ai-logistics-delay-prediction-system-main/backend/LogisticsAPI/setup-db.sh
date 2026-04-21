#!/bin/bash

# 🗄️ Database Setup Script for Logistics API
# This script initializes MySQL, creates the database, and sets up migrations

set -e

echo " Starting Logistics API Database Setup..."
echo ""

# Step 1: Check if MySQL is installed
echo "📋 Step 1: Checking MySQL installation..."
if ! command -v mysql &> /dev/null; then
    echo "❌ MySQL is not installed. Installing via Homebrew..."
    brew install mysql
else
    echo "✅ MySQL is installed"
fi
echo ""

# Step 2: Start MySQL service
echo "📋 Step 2: Starting MySQL service..."
brew services start mysql
echo "✅ MySQL service started"
echo ""

# Step 3: Wait for MySQL to be ready
echo "📋 Step 3: Waiting for MySQL to be ready..."
sleep 3
echo "✅ MySQL is ready"
echo ""

# Step 4: Create database
echo "📋 Step 4: Creating LogisticsDB database..."
mysql -u root -e "CREATE DATABASE IF NOT EXISTS LogisticsDB DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;"
echo "✅ Database created"
echo ""

# Step 5: Navigate to backend
echo "📋 Step 5: Setting up .NET project..."
cd /Users/vrinda/ai-logistics-delay-prediction-system/backend/LogisticsAPI

# Step 6: Clean and restore
echo "📋 Step 6: Cleaning and restoring NuGet packages..."
dotnet clean
dotnet restore
echo "✅ Packages restored"
echo ""

# Step 7: Build
echo "📋 Step 7: Building project..."
dotnet build
echo "✅ Project built successfully"
echo ""

# Step 8: Apply migrations
echo "📋 Step 8: Applying database migrations..."
dotnet ef database update
echo "✅ Migrations applied"
echo ""

# Step 9: Verify database
echo "📋 Step 9: Verifying database setup..."
mysql -u root LogisticsDB -e "SHOW TABLES;"
echo "✅ Database verification complete"
echo ""

echo "🎉 Database setup complete!"
echo ""
echo "📊 Database Info:"
echo "   • Database: LogisticsDB"
echo "   • Server: localhost"
echo "   • User: root"
echo ""
echo "🚀 To run the API:"
echo "   cd /Users/vrinda/ai-logistics-delay-prediction-system/backend/LogisticsAPI"
echo "   dotnet run"
echo ""
echo "📚 To access Swagger UI:"
echo "   https://localhost:7219/swagger/ui"
