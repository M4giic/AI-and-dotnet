using EventManagement.API.Data;
using EventManagement.API.Repositories;
using EventManagement.API.Services;
using EventManagement.Core.Services;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;


Batteries_V2.Init();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddScoped<IEventRepository, EventRepository>();
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<EventManagementDbContext>(options =>
{
    if (String.IsNullOrEmpty(connectionString))
    {
        options.UseInMemoryDatabase("EventManagementDb");
    }
    else
    {
        options.UseSqlite(connectionString);
    }
});
    
builder.Services.AddControllers();
// In Program.cs of the API project
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularOrigin",
        builder => builder
            .WithOrigins("http://localhost:4200") // Angular app
            .AllowAnyMethod()
            .AllowAnyHeader());
});

// And later in the request pipeline
var app = builder.Build();
app.MapControllers();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAngularOrigin");

app.Run();
