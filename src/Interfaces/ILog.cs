using magnus_backend.Enums;

namespace magnus_backend.Interfaces;

public interface ILog
{
    public void Log(string message, LogLevels loglevel, string? source = null);
}
