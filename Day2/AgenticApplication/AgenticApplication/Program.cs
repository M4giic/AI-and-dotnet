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

// Register service configurations
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.Configure<GmailSettings>(builder.Configuration.GetSection("GmailSettings"));
builder.Services.Configure<AgentSettings>(builder.Configuration.GetSection("AgentSettings"));
builder.Services.Configure<OpenAiSettings>(builder.Configuration.GetSection("OpenAI"));

var agentSettings = builder.Configuration.GetSection("AgentSettings").Get<AgentSettings>();
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
builder.Services.AddScoped<EmailTool>();
builder.Services.AddScoped<GmailReaderTool>();

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