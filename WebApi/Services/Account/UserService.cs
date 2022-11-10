using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApi.Data.Entities;
using WebApi.Helpers;
using WebApi.Models;

namespace WebApi.Services.Account;

public class UserService
{
    private readonly UserManager<User> _userManager;

    public UserService(UserManager<User> userManager)
    {
        _userManager = userManager;
    }
    
    public async Task<User> GetUserByEmail(string email)
    {
        return await _userManager.FindByEmailAsync(email);
    }
    
    //get user list
    public async Task<List<UserDto>> GetUsers()
    {
      
        return await _userManager.Users.Select(user=> new UserDto()
        {
            Id = user.Id,
            Email = user.Email,
            Username = user.UserName
        }).ToListAsync();
    }
    
    //get user by id
    public async Task<User> GetUserById(string id)
    {
        return await _userManager.FindByIdAsync(id);
    }

    public async Task<bool> AssignUserRole(RoleDto role)
    {
        var user = await _userManager.FindByIdAsync(role.UserId);
        if (user == null) return false;
            await _userManager.AddToRoleAsync(user, role.Role);
        return true;
    }
}