using magnus_backend.Common;
using magnus_backend.Models;

namespace magnus_backend.Interfaces;

public interface IUser
{
    ServiceResult<UserModel> AddUser(UserModel user);

    ServiceResult<UserModel> DeleteUser(string userId);

    ServiceResult<List<UserModel>> GetUser(string userId);

    ServiceResult<List<UserModel>> GetAllUsers();
}
