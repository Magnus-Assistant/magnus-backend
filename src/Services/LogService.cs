using magnus_backend.Enums;
using magnus_backend.Interfaces;
using magnus_backend.Models;
using MongoDB.Driver;

namespace magnus_backend.Services;
/*
    Logging service that is used just for the Web App.
    Not accessible via an API
*/
public class LoggingService(IMongoClient mongoClient): ILog
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
        // if it doesn't exit it will create it
        var collection = database.GetCollection<LogModel>("logs");

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