using System.Text.Json.Serialization;
using AgenticApplication.Data.Settings;
using AgenticApplication.Services;
using AgenticApplication.Tools;

var builder = WebApplication.CreateBuilder(args);

// Add controllers with JSON options
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

// Add Swagger for development
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
}

IConfigurationRoot config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddUserSecrets<Program>()
    .AddEnvironmentVariables()
    .Build();

// Register service configurations
builder.Services.Configure<EmailSettings>(config.GetSection("EmailSettings"));
builder.Services.Configure<GmailSettings>(config.GetSection("GmailSettings"));
builder.Services.Configure<AgentSettings>(config.GetSection("AgentSettings"));
builder.Services.Configure<OpenAiSettings>(config.GetSection("OpenAI"));

var agentSettings = config.GetSection("AgentSettings").Get<AgentSettings>();
if (agentSettings.PromptSource == "LangFuse")
{
    //we will add lang fuse here later
}
else
{
    builder.Services.AddSingleton<IPromptProvider, FileSystemPromptProvider>();
}

// Register OpenAI client service
builder.Services.AddScoped<IOpenAiClientService, OpenAiClientService>();

// Register tools
builder.Services.AddScoped<EmailSenderTool>();
builder.Services.AddScoped<GmailReaderTool>();
builder.Services.AddScoped<FinalResonseTool>();

// Register core services
builder.Services.AddScoped<IToolFactory, ToolFactory>();
builder.Services.AddScoped<IToolingService, ToolingService>();
builder.Services.AddScoped<IAiAgentService, AiAgentService>();

var app = builder.Build();

// Configure middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();