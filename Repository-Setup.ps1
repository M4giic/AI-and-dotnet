# Repository Setup Script
# This script sets up the repository, installs dependencies, and runs migrations
param (
    [switch]$SkipDependencyCheck = $false,
    [switch]$SkipMigrations = $false,
    [switch]$VerboseOutput = $false
)

$ErrorActionPreference = "Stop"
$VerbosePreference = if ($VerboseOutput) { "Continue" } else { "SilentlyContinue" }

# Array of project paths relative to the repository root
$projectPaths = @(
    "WebAPP/EventManagement.API",
    "Refactoring/DataLayerRefactoring",
    "Refactoring/CommunicationPattern"
    # Add more project paths as needed
    # "src/ClientApp"  # Angular app path
)

# Path to SQL scripts for seeding data
$sqlScriptsPath = "scripts/sql"

# Function to check if a command exists
function Test-CommandExists {
    param ($command)
    
    $exists = $null -ne (Get-Command $command -ErrorAction SilentlyContinue)
    return $exists
}

# Function to log messages
function Write-Log {
    param (
        [Parameter(Mandatory = $true)]
        [string]$Message,
        
        [Parameter(Mandatory = $false)]
        [ValidateSet("Info", "Warning", "Error", "Success")]
        [string]$Level = "Info"
    )
    
    $timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
    $color = switch ($Level) {
        "Info" { "White" }
        "Warning" { "Yellow" }
        "Error" { "Red" }
        "Success" { "Green" }
    }
    
    Write-Host "[$timestamp] " -NoNewline
    Write-Host "[$Level] " -NoNewline -ForegroundColor $color
    Write-Host "$Message"
}

# Function to install .NET SDK if not present
function Install-DotNetSDK {
    if (Test-CommandExists "dotnet") {
        $dotnetVersion = & dotnet --version
        Write-Log ".NET SDK version $dotnetVersion is already installed" "Info"
        return $true
    }
    
    # For Windows, redirect to download page as we can't automatically install
    if ($IsWindows -or $env:OS -match "Windows") {
        Write-Log ".NET SDK is not installed. Please install it from https://dotnet.microsoft.com/download" "Warning"
        $installDotNet = Read-Host "Do you want to open the download page? (Y/N)"
        if ($installDotNet -eq "Y" -or $installDotNet -eq "y") {
            Start-Process "https://dotnet.microsoft.com/download"
        }
        return $false
    }
    # For macOS
    elseif ($IsMacOS) {
        Write-Log "Installing .NET SDK using brew..." "Info"
        & brew install --cask dotnet-sdk
    }
    # For Linux
    elseif ($IsLinux) {
        Write-Log ".NET SDK is not installed. Please follow the instructions at https://docs.microsoft.com/en-us/dotnet/core/install/linux" "Warning"
        return $false
    }
    
    # Check if installation succeeded
    if (Test-CommandExists "dotnet") {
        $dotnetVersion = & dotnet --version
        Write-Log ".NET SDK version $dotnetVersion has been installed" "Success"
        return $true
    }
    else {
        Write-Log "Failed to install .NET SDK" "Error"
        return $false
    }
}

# Function to install Node.js and npm if not present
function Install-NodeAndNpm {
    if (Test-CommandExists "node" -and Test-CommandExists "npm") {
        $nodeVersion = & node --version
        $npmVersion = & npm --version
        Write-Log "Node.js version $nodeVersion and npm version $npmVersion are already installed" "Info"
        return $true
    }
    
    # For Windows, redirect to download page
    if ($IsWindows -or $env:OS -match "Windows") {
        Write-Log "Node.js is not installed. Please install it from https://nodejs.org/" "Warning"
        $installNode = Read-Host "Do you want to open the download page? (Y/N)"
        if ($installNode -eq "Y" -or $installNode -eq "y") {
            Start-Process "https://nodejs.org/"
        }
        return $false
    }
    # For macOS
    elseif ($IsMacOS) {
        Write-Log "Installing Node.js using brew..." "Info"
        & brew install node
    }
    # For Linux
    elseif ($IsLinux) {
        Write-Log "Installing Node.js using apt..." "Info"
        & sudo apt update
        & sudo apt install -y nodejs npm
    }
    
    # Check if installation succeeded
    if (Test-CommandExists "node" -and Test-CommandExists "npm") {
        $nodeVersion = & node --version
        $npmVersion = & npm --version
        Write-Log "Node.js version $nodeVersion and npm version $npmVersion have been installed" "Success"
        return $true
    }
    else {
        Write-Log "Failed to install Node.js and npm" "Error"
        return $false
    }
}

# Function to install Angular CLI if not present
function Install-AngularCLI {
    if (Test-CommandExists "ng") {
        $ngVersion = & ng version --version
        Write-Log "Angular CLI version $ngVersion is already installed" "Info"
        return $true
    }
    
    Write-Log "Installing Angular CLI..." "Info"
    & npm install -g @angular/cli
    
    # Check if installation succeeded
    if (Test-CommandExists "ng") {
        $ngVersion = & ng version --version
        Write-Log "Angular CLI version $ngVersion has been installed" "Success"
        return $true
    }
    else {
        Write-Log "Failed to install Angular CLI" "Error"
        return $false
    }
}

# Function to install project dependencies (.NET)
function Install-DotNetDependencies {
    param (
        [Parameter(Mandatory = $true)]
        [string]$ProjectPath
    )
    
    Write-Log "Installing .NET dependencies for $ProjectPath..." "Info"
    
    # Check if directory exists and contains a .csproj file
    if (-not (Test-Path $ProjectPath)) {
        Write-Log "Project directory not found: $ProjectPath" "Error"
        return $false
    }
    
    $csprojFiles = Get-ChildItem -Path $ProjectPath -Filter "*.csproj" -File
    if ($csprojFiles.Count -eq 0) {
        Write-Log "No .csproj file found in $ProjectPath" "Error"
        return $false
    }
    
    # Restore packages
    Push-Location $ProjectPath
    try {
        & dotnet restore
        if ($LASTEXITCODE -ne 0) {
            Write-Log "Failed to restore NuGet packages for $ProjectPath" "Error"
            return $false
        }
        
        Write-Log "Successfully installed .NET dependencies for $ProjectPath" "Success"
        return $true
    }
    finally {
        Pop-Location
    }
}

# Function to install project dependencies (Angular)
function Install-AngularDependencies {
    param (
        [Parameter(Mandatory = $true)]
        [string]$ProjectPath
    )
    
    Write-Log "Installing Angular dependencies for $ProjectPath..." "Info"
    
    # Check if directory exists and contains a package.json file
    if (-not (Test-Path $ProjectPath)) {
        Write-Log "Project directory not found: $ProjectPath" "Error"
        return $false
    }
    
    $packageJsonFile = Join-Path -Path $ProjectPath -ChildPath "package.json"
    if (-not (Test-Path $packageJsonFile)) {
        Write-Log "No package.json file found in $ProjectPath" "Error"
        return $false
    }
    
    # Install npm packages
    Push-Location $ProjectPath
    try {
        & npm install
        if ($LASTEXITCODE -ne 0) {
            Write-Log "Failed to install npm packages for $ProjectPath" "Error"
            return $false
        }
        
        Write-Log "Successfully installed Angular dependencies for $ProjectPath" "Success"
        return $true
    }
    finally {
        Pop-Location
    }
}

# Function to run database migrations
function Invoke-Migrations {
    param (
        [Parameter(Mandatory = $true)]
        [string]$ProjectPath
    )
    
    Write-Log "Running database migrations for $ProjectPath..." "Info"
    
    # Check if directory exists
    if (-not (Test-Path $ProjectPath)) {
        Write-Log "Project directory not found: $ProjectPath" "Error"
        return $false
    }
    
    # Run EF Core migrations
    Push-Location $ProjectPath
    try {
        # Update the database
        & dotnet ef database update
        if ($LASTEXITCODE -ne 0) {
            Write-Log "Failed to run migrations for $ProjectPath" "Error"
            return $false
        }
        
        Write-Log "Successfully applied migrations for $ProjectPath" "Success"
        return $true
    }
    finally {
        Pop-Location
    }
}

# Function to seed data from SQL scripts
function Invoke-DataSeeding {
    param (
        [Parameter(Mandatory = $true)]
        [string]$ProjectPath,
        
        [Parameter(Mandatory = $true)]
        [string]$ScriptsPath
    )
    
    $fullScriptsPath = Join-Path -Path $ProjectPath -ChildPath $ScriptsPath
    Write-Log "Seeding data for $ProjectPath from $fullScriptsPath..." "Info"
    
    # Check if scripts directory exists
    if (-not (Test-Path $fullScriptsPath)) {
        Write-Log "SQL scripts directory not found: $fullScriptsPath" "Error"
        return $false
    }
    
    # Get project name from path to find matching SQL script
    $projectName = Split-Path -Path $ProjectPath -Leaf
    $sqlScriptPath = Join-Path -Path $fullScriptsPath -ChildPath "$projectName-seed.sql"
    
    if (-not (Test-Path $sqlScriptPath)) {
        Write-Log "SQL seed script not found: $sqlScriptPath" "Warning"
        return $false
    }
    
    # Execute the SQL script against the database
    Push-Location $ProjectPath
    try {
        $connectionString = Get-ConnectionString -ProjectPath $ProjectPath
        if (-not $connectionString) {
            Write-Log "Could not determine connection string for $ProjectPath" "Error"
            return $false
        }
        
        if (-not (Is-DatabaseEmpty -ConnectionString $connectionString)) {
            Write-Log "Database is not empty. Skipping data seeding for $ProjectPath." "Info"
            return $true
        }

        # For SQLite
        if ($connectionString -match "Data Source=(.+)") {
            $dbPath = $Matches[1]
            $absoluteDbPath = Join-Path -Path (Get-Location) -ChildPath $dbPath
            $sqlScript = Get-Content -Path $sqlScriptPath -Raw
            
            # Use the SQLite CLI to execute the script
            if (Test-CommandExists "sqlite3") {
                & sqlite3 $absoluteDbPath $sqlScript
                if ($LASTEXITCODE -ne 0) {
                    Write-Log "Failed to seed data for $ProjectPath" "Error"
                    return $false
                }
            }
            else {
                Write-Log "SQLite CLI not found. Please install it to seed data." "Warning"
                return $false
            }
        }
        # For SQL Server
        elseif ($connectionString -match "Server=(.+);Database=(.+);") {
            if (Test-CommandExists "sqlcmd") {
                & sqlcmd -S $Matches[1] -d $Matches[2] -i $sqlScriptPath
                if ($LASTEXITCODE -ne 0) {
                    Write-Log "Failed to seed data for $ProjectPath" "Error"
                    return $false
                }
            }
            else {
                Write-Log "SQL Server CLI (sqlcmd) not found. Please install it to seed data." "Warning"
                return $false
            }
        }
        else {
            Write-Log "Unsupported database type in connection string: $connectionString" "Error"
            return $false
        }
        
        Write-Log "Successfully seeded data for $ProjectPath" "Success"
        return $true
    }
    finally {
        Pop-Location
    }
}

function Is-DatabaseEmpty {
    param (
        [Parameter(Mandatory = $true)]
        [string]$ConnectionString
    )
    
    # For SQLite
    if ($ConnectionString -match "Data Source=(.+)") {
        $dbPath = $Matches[1]
        $absoluteDbPath = Join-Path -Path (Get-Location) -ChildPath $dbPath
        
        if (Test-CommandExists "sqlite3") {
            $query = "SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name NOT LIKE 'sqlite_%';"
            $tableCount = & sqlite3 $absoluteDbPath $query
            return [int]$tableCount -eq 0
        }
        else {
            Write-Log "SQLite CLI not found. Cannot check if database is empty." "Warning"
            return $false
        }
    }
    # For SQL Server
    elseif ($ConnectionString -match "Server=(.+);Database=(.+);") {
        if (Test-CommandExists "sqlcmd") {
            $server = $Matches[1]
            $database = $Matches[2]
            $query = "IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE') SELECT 0 ELSE SELECT 1;"
            $result = & sqlcmd -S $server -d $database -Q $query -h -1
            return [int]$result -eq 1
        }
        else {
            Write-Log "SQL Server CLI (sqlcmd) not found. Cannot check if database is empty." "Warning"
            return $false
        }
    }
    else {
        Write-Log "Unsupported database type in connection string: $ConnectionString" "Error"
        return $false
    }
}

# Function to extract connection string from appsettings.json
function Get-ConnectionString {
    param (
        [Parameter(Mandatory = $true)]
        [string]$ProjectPath
    )
    
    $appSettingsPath = Join-Path -Path $ProjectPath -ChildPath "appsettings.json"
    if (-not (Test-Path $appSettingsPath)) {
        $appSettingsPath = Join-Path -Path $ProjectPath -ChildPath "appsettings.Development.json"
        if (-not (Test-Path $appSettingsPath)) {
            Write-Log "No appsettings.json or appsettings.Development.json found in $ProjectPath" "Error"
            return $null
        }
    }
    
    try {
        $appSettings = Get-Content -Path $appSettingsPath | ConvertFrom-Json
        
        # Try to find connection string in different formats
        if ($appSettings.ConnectionStrings.DefaultConnection) {
            return $appSettings.ConnectionStrings.DefaultConnection
        }
        elseif ($appSettings.ConnectionStrings.ProductCatalogContext) {
            return $appSettings.ConnectionStrings.ProductCatalogContext
        }
        elseif ($appSettings.ConnectionStrings.OrderProcessingContext) {
            return $appSettings.ConnectionStrings.OrderProcessingContext
        }
        # Add more connection string names as needed
        
        Write-Log "No connection string found in appsettings.json" "Error"
        return $null
    }
    catch {
        Write-Log "Error parsing appsettings.json: $_" "Error"
        return $null
    }
}

# Main script execution

# Display a welcome message
Write-Host "`n=========================================" -ForegroundColor Cyan
Write-Host "      Repository Setup Script" -ForegroundColor Cyan
Write-Host "=========================================`n" -ForegroundColor Cyan

# Check and install dependencies if needed
if (-not $SkipDependencyCheck) {
    Write-Host "Checking dependencies..." -ForegroundColor Cyan
    
    $dotnetInstalled = Install-DotNetSDK
    if (-not $dotnetInstalled) {
        Write-Log "Setup cannot continue without .NET SDK installed" "Error"
        exit 1
    }
    
    $nodeInstalled = Install-NodeAndNpm
    if (-not $nodeInstalled) {
        Write-Log "Node.js and npm are required for Angular applications" "Warning"
    }
    else {
        $angularInstalled = Install-AngularCLI
        if (-not $angularInstalled) {
            Write-Log "Angular CLI is required for Angular applications" "Warning"
        }
    }
}

# Get repository root path
$repoRoot = Get-Location
Write-Log "Repository root: $repoRoot" "Info"

# Process each project
foreach ($relativePath in $projectPaths) {
    $projectPath = Join-Path -Path $repoRoot -ChildPath $relativePath
    Write-Host "`nProcessing project: $relativePath" -ForegroundColor Cyan
    
    # Check if project exists
    if (-not (Test-Path $projectPath)) {
        Write-Log "Project path does not exist: $projectPath" "Error"
        continue
    }
    
    # Install dependencies
    if ($relativePath -match "ClientApp") {
        # Angular project
        if (Test-CommandExists "npm") {
            Install-AngularDependencies -ProjectPath $projectPath
        }
        else {
            Write-Log "Skipping Angular dependencies for $relativePath (npm not installed)" "Warning"
        }
    }
    else {
        # .NET project
        Install-DotNetDependencies -ProjectPath $projectPath
        
        # Run migrations if not skipped
        if (-not $SkipMigrations) {
            Invoke-Migrations -ProjectPath $projectPath
            Invoke-DataSeeding -ProjectPath $projectPath -ScriptsPath $sqlScriptsPath
        }
    }
}

Write-Host "`n=========================================" -ForegroundColor Cyan
Write-Host "      Setup Completed" -ForegroundColor Cyan
Write-Host "=========================================`n" -ForegroundColor Cyan

# Return success
exit 0