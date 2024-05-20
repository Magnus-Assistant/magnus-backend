using magnus_backend.Interfaces;
using magnus_backend.Models;
using magnus_backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace magnus_backend.Controllers;

[Authorize]
[Route("api/user")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly UserService _userService;
    private readonly ILog _logger;

    // inject and instance of the mongoClient and our custom logger
    // into the controller so that we can use it
    public UserController(UserService userService, ILog logger)
    {
        _userService = userService;
        _logger = logger;
    }

    // Here is where we create all API's that correspond to users
    [HttpGet("{UserId}")]
    public ActionResult<UserModel> GetUser(string UserId)
    {
        var user = _userService.GetUser(UserId).Data;

        if (user == null || user.Count == 0)
        {
            _logger.Log(
                $"No user found with UserId {UserId}",
                Enums.LogLevels.Info,
                "UserController - GetUser"
            );
            return NotFound(new { Message = $"No user found with UserId {UserId}" });
        }

        return Ok(user);
    }

    [HttpGet]
    public ActionResult<UserModel> GetAllUsers()
    {
        var users = _userService.GetAllUsers().Data;
        return Ok(users);
    }

    [HttpPost]
    public ActionResult AddUser([FromBody] UserModel user)
    {

        var addResult = _userService.AddUser(user);
        if (addResult.Success == false)
        {
            return BadRequest($"Failed to create User. Error: {addResult.Error}");
        }

        return Ok("Successfully created user");
    }

    [HttpDelete]
    public ActionResult DeleteUser(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            return BadRequest("UserId Cannot be Null or empty");
        }

        // if we didn't successfully delete, error
        var user = _userService.DeleteUser(userId);
        if (user.Success == false)
        {
            _logger.Log(
                $"Failed to delete user: {userId}",
                Enums.LogLevels.Error,
                "UserController - DeleteUser"
            );
            return BadRequest($"Failed to delete User. Error: {user.Error}");
        }

        return Ok($"Successfully deleted User. UserId: {userId}");
    }
}
