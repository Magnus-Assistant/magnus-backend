using MongoDB.Bson;

namespace magnus_backend.Models;

public class UserModel
{
    public ObjectId Id { get; }
    public required string UserId { get; set; }
    public required string Username { get; set; }
}