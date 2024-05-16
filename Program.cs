using magnus_backend;
using magnus_backend.Interfaces;
using magnus_backend.Services;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

// load the .env file
try
{
    var path = Path.Combine(builder.Environment.ContentRootPath, ".env");
    DotEnv.Load(path);
}
catch (Exception ex)
{
    Console.WriteLine($"Error loading .env file: {ex.Message}");
}
var connectionString = Environment.GetEnvironmentVariable("DB_CONN_STRING");

//load config from appsettings and connect to DB
var config = new ConfigurationBuilder()
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddEnvironmentVariables()
    .Build();

builder.Services.AddSingleton<IMongoClient>(sp => new MongoClient(connectionString));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers();
builder.Services.AddHttpClient();
builder.Services.AddScoped<IUser, User>();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.UseDefaultFiles();
app.UseStaticFiles();

app.Run();
