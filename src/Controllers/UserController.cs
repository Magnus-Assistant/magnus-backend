using magnus_backend.Interfaces;
using magnus_backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Microsoft.AspNetCore.Authorization;

//"Url": "http://192.168.1.29:5000" ADD BACK TO APPSETTINGS

namespace magnus_backend.Controllers;

[Authorize]
[Route("api/user")]
[ApiController]
public class UserController(IUser user, IMongoClient mongoClient) : ControllerBase
{
    private readonly IUser _user = user;
    private readonly IMongoClient _mongoClient = mongoClient;

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
            return NotFound(new { Message = $"No user found with UserId {UserId}"});
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
            return NotFound(new { Message = $"No users found"});
        }

        return Ok(users);
    }

    [HttpPost]
    public ActionResult AddUser([FromBody] UserModel user)
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
            return Conflict("User Already Exists"); // returns a 409
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
    public ActionResult DeleteUser(string userId)
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
