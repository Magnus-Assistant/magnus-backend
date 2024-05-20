using magnus_backend.Common;
using magnus_backend.Enums;
using magnus_backend.Models;
using Microsoft.AspNetCore.Mvc;

namespace magnus_backend.Interfaces;

public interface IMagnusLog
{
    public ServiceResult<MagnusLogModel> Log(string UserId, string Message, LogLevels LogLevel, string? Source = null);
}