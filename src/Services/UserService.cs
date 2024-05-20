using magnus_backend.Common;
using magnus_backend.Interfaces;
using magnus_backend.Models;
using MongoDB.Driver;

namespace magnus_backend.Services;

/*
    User services that handles database connections and interactions
*/
public class UserService(IMongoCollection<UserModel> userCollection, ILog logger) : IUser
{
    private readonly IMongoCollection<UserModel> _userCollection = userCollection;
    private readonly ILog _logger = logger;

    public ServiceResult<UserModel> AddUser(UserModel user)
    {
        var filter = Builders<UserModel>.Filter.Eq("UserId", user.UserId);

        try
        {
            var exists = _userCollection.Find(filter).ToList();
            if (exists == null || exists.Count > 0)
            {
                return ServiceResult<UserModel>.ErrorResult("User already exists");
            }
        }
        catch (Exception e)
        {
            return ServiceResult<UserModel>.ErrorResult(
                $"DB Error, Failed to query user. Error: {e}"
            );
        }

        // add a created at value
        user.CreatedAt = DateTime.UtcNow.ToString();

        try
        {
            _userCollection.InsertOne(user);
            _logger.Log(
                $"Created User Successfully: {user.UserId}",
                Enums.LogLevels.Info,
                "UserService - AddUser"
            );
            return ServiceResult<UserModel>.SuccessResult(user);
        }
        catch (Exception e)
        {
            _logger.Log(
                $"Database Error, Failed to create user: {e}",
                Enums.LogLevels.Error,
                e.ToString()
            );
            return ServiceResult<UserModel>.ErrorResult(
                $"DB Error, Failed to add user. Error: {e}"
            );
        }
    }

    public ServiceResult<UserModel> DeleteUser(string userId)
    {
        var filter = Builders<UserModel>.Filter.Eq("UserId", userId);
        var user = _userCollection.Find(filter).ToList();

        if (user.Count == 0)
        {
            _logger.Log(
                $"User does not exist: {userId}",
                Enums.LogLevels.Info,
                "UserService - DeleteUser"
            );
            return ServiceResult<UserModel>.ErrorResult("User does not exist");
        }

        _userCollection.DeleteOne(filter);
        _logger.Log(
            $"Deleted User Successfully. UserId: {userId}",
            Enums.LogLevels.Info,
            "UserService - DeleteUser"
        );
        return ServiceResult<UserModel>.SuccessResult(user[0]);
    }

    public ServiceResult<List<UserModel>> GetAllUsers()
    {
        try
        {
            return ServiceResult<List<UserModel>>.SuccessResult(
                _userCollection.Find(_ => true).ToList()
            );
        }
        catch (Exception e)
        {
            return ServiceResult<List<UserModel>>.ErrorResult(
                $"DB Error, Failed to retreive all users. Error {e}"
            );
        }
    }

    public ServiceResult<List<UserModel>> GetUser(string userId)
    {
        var filter = Builders<UserModel>.Filter.Eq("UserId", userId);
        try
        {
            var user = _userCollection.Find(filter).ToList();
            return ServiceResult<List<UserModel>>.SuccessResult(user);
        }
        catch (Exception e)
        {
            return ServiceResult<List<UserModel>>.ErrorResult(
                $"DB Error, Failed to retreive user. Error {e}"
            );
        }
    }
}
