using magnus_backend;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;

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

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Magnus Backend", Version = "v1" });
});

var app = builder.Build();

var isProd = Environment.GetEnvironmentVariable("IS_PROD");
if (isProd == "false")
{
    Console.WriteLine("WE ARE IN DEVELOPMENT!");
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Magnus Backend");
    });
}

app.MapControllers();

app.UseDefaultFiles();
app.UseStaticFiles();

app.Run();
