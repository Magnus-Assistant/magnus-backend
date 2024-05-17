using magnus_backend.Interfaces;
using magnus_backend.Models;
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
}