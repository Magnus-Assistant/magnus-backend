using magnus_backend.Models;
using Microsoft.AspNetCore.Mvc;

namespace magnus_backend.Interfaces;

public interface IUser {
    IActionResult AddUser(UserModel user);

    IActionResult DeleteUser(string userId);

    ActionResult<UserModel> GetUser(string userId);

    ActionResult<UserModel> GetAllUsers();
}