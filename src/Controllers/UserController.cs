using magnus_backend.Interfaces;
using magnus_backend.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace magnus_backend.Controllers;

[Route("api/user")]
[ApiController]
public class UserController : ControllerBase, IUser
{
    private readonly IMongoClient _mongoClient;
    private readonly ILog _logger;

    // inject and instance of the mongoClient and our custom logger 
    // into the controller so that we can use it
    public UserController(IMongoClient mongoClient, ILog logger)
    {
        _mongoClient = mongoClient;
        _logger = logger;
    }

    // Here is where we create all API's that correspond to users
    [HttpGet("{UserId}")]
    public ActionResult<UserModel> GetUser(string UserId)
    {
        var database = _mongoClient.GetDatabase("Magnus");
        var collection = database.GetCollection<UserModel>("users");

        var filter = Builders<UserModel>.Filter.Eq("UserId", UserId);

        var user = collection.Find(filter).ToList();

        if (user.Count == 0)
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
        var database = _mongoClient.GetDatabase("Magnus");
        var collection = database.GetCollection<UserModel>("users");

        var users = collection.Find(_ => true).ToList();
        if (users.Count == 0)
        {
            return NotFound(new { Message = $"No users found" });
        }

        return Ok(users);
    }

    [HttpPost]
    public IActionResult AddUser([FromBody] UserModel user)
    {
        if (user == null)
        {
            _logger.Log($"User cannot be null", Enums.LogLevels.Error, "UserController - AddUser");
            return BadRequest("User cannot be null");
        }

        var database = _mongoClient.GetDatabase("Magnus");
        var collection = database.GetCollection<UserModel>("users");

        var filter = Builders<UserModel>.Filter.Eq("UserId", user.UserId);
        var exists = collection.Find(filter).ToList();

        if (exists.Count > 0)
        {
            return BadRequest("User Already Exists");
        }

        // make sure all of our values are filled out
        if (
            !string.IsNullOrWhiteSpace(user.UserId)
            && !string.IsNullOrWhiteSpace(user.Username)
            && !string.IsNullOrWhiteSpace(user.Email)
        )
        {
            try
            {
                // add a created at value
                user.CreatedAt = DateTime.UtcNow.ToString();

                collection.InsertOne(user);
                _logger.Log(
                    $"Created User Successfully: {user.UserId}",
                    Enums.LogLevels.Info,
                    "UserController - AddUser"
                );
                return Ok("Created User Successfully");
            }
            catch (Exception e)
            {
                _logger.Log(
                    $"Failed to create user: {e}",
                    Enums.LogLevels.Error,
                    "UserController - AddUser"
                );
                return BadRequest("Failed to create User");
            }
        }

        _logger.Log(
            $"User values cannot be null or whitespace!",
            Enums.LogLevels.Error,
            "UserController - AddUser"
        );
        return BadRequest("User values cannot be null or whitespace!");
    }

    [HttpDelete]
    public IActionResult DeleteUser(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            return BadRequest("UserId Cannot be Null or empty");
        }

        var database = _mongoClient.GetDatabase("Magnus");
        var collection = database.GetCollection<UserModel>("users");

        var filter = Builders<UserModel>.Filter.Eq("UserId", userId);
        var users = collection.Find(filter).ToList();

        if (users.Count == 0)
        {
            _logger.Log(
                $"User does not exist: {userId}",
                Enums.LogLevels.Info,
                "UserController - DeleteUser"
            );
            return BadRequest($"User does not exist: {userId}");
        }

        try
        {
            collection.DeleteOne(filter);
            _logger.Log(
                $"Deleted User Successfully. UserId: {userId}",
                Enums.LogLevels.Info,
                "UserController - DeleteUser"
            );
            return Ok($"Deleted User Successfully. UserId: {userId}");
        }
        catch (Exception e)
        {
            _logger.Log(
                $"Failed to delete user: {userId}. Error: {e}",
                Enums.LogLevels.Error,
                "UserController - DeleteUser"
            );
            return BadRequest($"Failed to delete User: {e}");
        }
    }
}
