using System.Text.Json.Serialization;
using Microsoft.Net.Http.Headers;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace magnus_backend.Models;

public class UserModel
{
    [BsonId]
    [JsonIgnore]
    public ObjectId Id { get; set; }
    public required string UserId { get; set; }
    public required string Username { get; set; }
    public required string Email { get; set; }
    public string? CreatedAt {get; set; }
}