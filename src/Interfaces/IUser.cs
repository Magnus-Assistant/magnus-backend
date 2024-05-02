using magnus_backend.Models;

namespace magnus_backend.Interfaces;

public interface IUser {
    void AddUser(UserModel user);

    void DeleteUser(string user_id);

    void UpdateUser(string user_id, string new_username);

    IEnumerable<UserModel> GetAllUsers();
}