using magnus_backend.Interfaces;
using magnus_backend.Models;

namespace magnus_backend.Services;

public class User : IUser
{
    private readonly List<UserModel> _users = [];

    public void AddUser(UserModel user)
    {
        _users.Add(user);
    }

    public void DeleteUser(string user_id)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<UserModel> GetAllUsers()
    {
        return _users;
    }

    public void UpdateUser(string user_id, string new_username)
    {
        throw new NotImplementedException();
    }
}