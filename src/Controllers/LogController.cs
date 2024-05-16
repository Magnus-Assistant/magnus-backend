using magnus_backend.Enums;
using magnus_backend.Interfaces;
using magnus_backend.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace magnus_backend.Controllers;

[Controller]
public class LogController(IMongoClient mongoClient) : ControllerBase, ILog
{
    private readonly IMongoClient _mongoClient = mongoClient;

    public void Log(string message, LogLevels loglevel, string? source = null)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            Console.WriteLine("Message cannot be empty!");
            return;
        }

        var database = _mongoClient.GetDatabase("Magnus");
        var collection = database.GetCollection<LogModel>("backend_logs");

        var log = new LogModel
        {
            Message = message,
            LogLevel = loglevel,
            Source = source,
            CreatedAt = DateTime.UtcNow.ToString()
        };

        try
        {
            collection.InsertOne(log);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Failed to save log: {e}");
        }
    }
}
