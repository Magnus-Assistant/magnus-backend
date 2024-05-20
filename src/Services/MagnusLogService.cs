using magnus_backend.Common;
using magnus_backend.Enums;
using magnus_backend.Interfaces;
using magnus_backend.Models;
using MongoDB.Driver;

namespace magnus_backend.Services;

public class MagnusLogService(IMongoCollection<MagnusLogModel> logCollection) : IMagnusLog
{
    private readonly IMongoCollection<MagnusLogModel> _logCollection = logCollection;

    public ServiceResult<MagnusLogModel> Log(
        string UserId,
        string Message,
        LogLevels LogLevel,
        string? Source = null
    )
    {
        // create the new log to be added to the database
        var log = new MagnusLogModel
        {
            UserId = UserId,
            Message = Message,
            LogLevel = LogLevel,
            Source = Source,
            CreatedAt = DateTime.UtcNow.ToString()
        };

        try
        {
            _logCollection.InsertOne(log);
            return ServiceResult<MagnusLogModel>.SuccessResult(log);
        }
        catch (Exception e)
        {
            return ServiceResult<MagnusLogModel>.ErrorResult($"Failed to save log. Error: {e}");
        }
    }
}
