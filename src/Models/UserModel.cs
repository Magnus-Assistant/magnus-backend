using MongoDB.Bson;

namespace magnus_backend.Models;

public class UserModel
{
    public ObjectId Id { get; }
    public string UserId { get; set; }
    public string Username { get; set; }
}