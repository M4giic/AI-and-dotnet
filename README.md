# Repository Setup Guide

This repository contains multiple .NET and Angular projects that need to be properly set up before use. This guide explains how to use the automated setup script to install dependencies, run database migrations, and seed data.

## Prerequisites

The setup script will attempt to install these prerequisites if they are missing, but it's recommended to have them installed beforehand:

- **.NET SDK 8.0** - [Download .NET 8.0](https://dotnet.microsoft.com/download/dotnet/8.0)
- **Node.js and npm** - [Download Node.js](https://nodejs.org/)
- **Angular CLI** - Install using `npm install -g @angular/cli`
- **SQLite** - Required for database operations

## Repository Structure

This repository contains the following projects:

- `src/OrderProcessingAPI` - A REST API project that can be transformed to various communication patterns
- `src/ProductCatalog` - A project using the repository pattern that can be transformed to different data access layers
- `src/ClientApp` - An Angular frontend application

## Setup Instructions

### Using the Automated Setup Script

1. Clone this repository to your local machine:
   ```
   git clone <repository-url>
   cd <repository-directory>
   ```

2. Run the setup script:
   ```powershell
   # Windows - Run in PowerShell
   .\Setup-Repository.ps1
   
   # macOS/Linux - Run in PowerShell Core
   pwsh Setup-Repository.ps1
   ```

### Script Options

The setup script supports several optional parameters:

- `-SkipDependencyCheck`: Skip checking and installing dependencies
- `-SkipMigrations`: Skip running database migrations and data seeding
- `-VerboseOutput`: Display more detailed output during setup

Example with options:
```powershell
.\Setup-Repository.ps1 -VerboseOutput -SkipMigrations
```

### Manual Setup

If you prefer to set up the repository manually, follow these steps for each project:

1. **For .NET projects (OrderProcessingAPI, ProductCatalog):**
   ```
   cd src/OrderProcessingAPI
   dotnet restore
   dotnet build
   dotnet ef database update
   
   cd ../ProductCatalog
   dotnet restore
   dotnet build
   dotnet ef database update
   ```

2. **For the Angular project:**
   ```
   cd src/ClientApp
   npm install
   ng serve
   ```

3. **To seed data manually:**
   ```
   # For SQLite databases
   sqlite3 path/to/database.db < scripts/sql/ProjectName-seed.sql
   ```

## Database Migrations

The setup script automatically applies any pending migrations to create the database schema. If you need to manually run migrations:

```
cd src/ProjectName
dotnet ef database update
```

To create a new migration:
```
dotnet ef migrations add MigrationName
```

## Seeding Data

Sample data is loaded from SQL scripts in the `scripts/sql` directory. The setup script executes these scripts automatically. If you need to manually seed data:

```
# For SQLite
sqlite3 path/to/database.db < scripts/sql/ProjectName-seed.sql
```

## Troubleshooting

### Common Issues

1. **Missing .NET tools:**
   ```
   dotnet tool install --global dotnet-ef
   ```

2. **Database file permission issues:**
   Ensure you have write permissions to the directory where the database files are created.

3. **Node.js or npm errors:**
   Try updating to the latest version:
   ```
   npm install -g npm@latest
   ```

4. **Database already exists:**
   To reset the database:
   ```
   dotnet ef database drop --force
   dotnet ef database update
   ```

### Getting Help

If you encounter issues not covered here, please:
1. Check the logs for detailed error messages
2. Search for the error message in our issue tracker
3. Create a new issue with detailed information about the problem

## License

This project is licensed under the MIT License - see the LICENSE file for details.