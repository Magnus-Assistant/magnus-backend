using magnus_backend.Interfaces;
using magnus_backend.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

//"Url": "http://192.168.1.29:5000" ADD BACK TO APPSETTINGS

namespace magnus_backend.Controllers;

[Route("api/user")]
[ApiController]
public class UserController(IMongoClient mongoClient) : ControllerBase, IUser
{
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
    public ActionResult AddUser([FromBody] UserModel user)
    {

        if (user == null)
        {
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
        if (!string.IsNullOrWhiteSpace(user.UserId) &&
            !string.IsNullOrWhiteSpace(user.Username) &&
            !string.IsNullOrWhiteSpace(user.Email))
        {
            try
            {
                // add a created at value
                user.CreatedAt = DateTime.UtcNow.ToString();

                collection.InsertOne(user);
                Console.WriteLine($"Created user: {user.Username}");
                return Ok("Created User Successfully");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Failed to create user: {e}");
                return BadRequest("Failed to create User");
            }
        }

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

        try
        {
            var filter = Builders<UserModel>.Filter.Eq("UserId", userId);
            collection.DeleteOne(filter);
            return Ok($"Deleted User Successfully. UserId: {userId}");
        }
        catch (Exception e)
        {
            return BadRequest($"Failed to delete User: {e}");
        }
    }
}