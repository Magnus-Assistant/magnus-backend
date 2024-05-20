using magnus_backend.Models;
using magnus_backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace magnus_backend.Controllers;

/*
    Houses APIs for creating logs on Magnus
*/
[Authorize]
[Route("api/log")]
[ApiController]
public class MagnusLogController(MagnusLogService logger) : ControllerBase
{
    private readonly MagnusLogService _logger = logger;

    [HttpPost]
    public ActionResult Log([FromBody] MagnusLogModel magnusLog)
    {
        try
        {
            _logger.Log(magnusLog.UserId, magnusLog.Message, magnusLog.LogLevel, magnusLog.Source);
        }
        catch (Exception e)
        {
            return BadRequest($"failed to create log: {e}");
        }
        return Ok();
    }
}
