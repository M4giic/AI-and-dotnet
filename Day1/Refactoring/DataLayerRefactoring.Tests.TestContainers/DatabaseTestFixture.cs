using System.Reflection;
using DataLayerRefactoring.Data;
using DataLayerRefactoring.Models;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Testcontainers.PostgreSql;

namespace DataLayerRefactoring.Tests.TestContainers;

public class DatabaseTestFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer;
    protected ProductCatalogContext DbContext { get; private set; } = null!;
    private readonly string _imageName = "postgres:16";
    private readonly string _password = "Strong_Password123!";
    private readonly string _username = "testuser";
    private readonly string _databaseName = "testdb";
    private readonly bool _performCleanup = true;
    private bool isRunning = false;
    
    // Default PostgreSQL port
    private readonly int _postgresPort = 5432;

    
    public DatabaseTestFixture()
    {
        _dbContainer = new PostgreSqlBuilder()
            .WithImage(_imageName)
            .WithPassword(_password)
            .WithUsername(_username)
            .WithDatabase(_databaseName)
            .WithCleanUp(_performCleanup)
            .Build();
    }

    public async Task InitializeAsync()
    {
        // Start container
        await _dbContainer.StartAsync();
        
        // Create DbContext with container connection
        var options = new DbContextOptionsBuilder<ProductCatalogContext>()
            .UseNpgsql(_dbContainer.GetConnectionString())
            .Options;
        
        DbContext = new ProductCatalogContext(options);
        
        // Ensure database is created and migrations are applied
        await DbContext.Database.EnsureCreatedAsync();
        
        // Seed with test data
        await ExecuteSqlScriptAsync("seed.sql");
        isRunning = true;
    }
    
    public string GetContainerConnectionString()
    {
        if (!isRunning)
        {
            throw new InvalidOperationException("Database container is not running. Call InitializeAsync() first.");
        }
        
        return _dbContainer.GetConnectionString();
    }

    private async Task ExecuteSqlScriptAsync(string scriptName)
    {
        // Get the script content from embedded resources
        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = $"DataLayerRefactoring.Tests.SqlScripts.{scriptName}";
        
        using var stream = assembly.GetManifestResourceStream(resourceName);
        if (stream == null)
            throw new Exception($"SQL script {scriptName} not found in embedded resources");
            
        using var reader = new StreamReader(stream);
        var script = await reader.ReadToEndAsync();
        
        // Execute the script
        await using var connection = new NpgsqlConnection(_dbContainer.GetConnectionString());
        await connection.OpenAsync();
        
        await using var command = new NpgsqlCommand(script, connection);
        await command.ExecuteNonQueryAsync();
    }
  

    public async Task DisposeAsync()
    {
        await DbContext.DisposeAsync();
        await _dbContainer.StopAsync();
    }
}