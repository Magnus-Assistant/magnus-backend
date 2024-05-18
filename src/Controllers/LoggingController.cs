using magnus_backend.Interfaces;
using magnus_backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace magnus_backend.Controllers;

/*
    Houses APIs for creating logs on Magnus
*/
[Route("api/log")]
[ApiController]
public class LoggingController(IMongoClient mongoClient) : ControllerBase, IMagnusLog
{
    private readonly IMongoClient _mongoClient = mongoClient;

    [Authorize]
    [HttpPost]
    public ActionResult Log(MagnusLogModel magnusLog)
    {
        if (magnusLog == null)
        {
            return BadRequest("Log cannot be null!");
        }

        var database = _mongoClient.GetDatabase("Magnus");
        // save all logs to the same place
        var collection = database.GetCollection<MagnusLogModel>("logs");

        // create the new log to be added to the database
        var log = new MagnusLogModel
        {
            UserId = magnusLog.UserId,
            Message = magnusLog.Message,
            LogLevel = magnusLog.LogLevel,
            Source = magnusLog.Source,
            CreatedAt = DateTime.UtcNow.ToString()
        };

        try
        {
            collection.InsertOne(log);
        }
        catch (Exception e)
        {
            return BadRequest($"failed to create log: {e}");
        }
        return Ok();
    }
}

