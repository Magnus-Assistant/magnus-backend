using magnus_backend;
using magnus_backend.Controllers;
using magnus_backend.Interfaces;
using magnus_backend.Models;
using magnus_backend.Services;
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
    .AddJsonFile("appsettings.json")
    .AddEnvironmentVariables()
    .Build();

// we only want one instance of these to live at a time
builder.Services.AddSingleton<IMongoClient, MongoClient>(sp => new MongoClient(connectionString));

// add one instance of the Magnus database that we can use in our services
builder.Services.AddScoped(sp => {
    var client = sp.GetRequiredService<IMongoClient>();
    return client.GetDatabase("Magnus");
});

// add one instance of the users collection that we can use in our services
builder.Services.AddScoped(sp => {
    var database = sp.GetRequiredService<IMongoDatabase>();
    return database.GetCollection<UserModel>("users");
});

// add one instance of the users collection that we can use in our services
builder.Services.AddScoped(sp => {
    var database = sp.GetRequiredService<IMongoDatabase>();
    return database.GetCollection<MagnusLogModel>("logs");
});

builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<LoggingService>();
builder.Services.AddScoped<MagnusLogService>();

// builder.Services.AddSingleton<IMagnusLog, LoggingController>();
builder.Services.AddSingleton<ILog, LoggingService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers();
builder.Services.AddHttpClient();

builder.Services.AddAuthentication().AddJwtBearer();

builder.Services.AddAuthorization();

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

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseDefaultFiles();
app.UseStaticFiles();

app.Run();
