using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using WebApi.Controllers;
using WebApi.Data;
using WebApi.Data.Entities;
using WebApi.Services.Account;

namespace WebApi.Helpers;

public static class SeedData
{
    public static async Task Initialize(this IServiceProvider serviceProvider)
    {
        var appServices = serviceProvider.CreateScope();
        var userManager = appServices.ServiceProvider.GetRequiredService<UserManager<User>>();
        var roleManager = appServices.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        
        string[] roles = new string[] { "Owner", "Administrator", "Manager", "Editor", "Buyer", "Business", "Seller", "Subscriber" };

        //create default roles
        foreach (string role in roles)
        {
            var existingRole = await roleManager.FindByNameAsync(role);
            if (existingRole == null)
            {
                var identityRole = new IdentityRole(role);
                await   roleManager.CreateAsync(identityRole);
            }
        }
        
        
        //create admin user
        var user = new User()
        {
            FirstName = "Nurullo",
            LastName = "Sulaymonov",
            Email = "sulaimonovnn@gmail.com",
            UserName = "nurullo",
            PhoneNumber = "+992900737704",
            EmailConfirmed = true,
            PhoneNumberConfirmed = true
        };
        //
        var existingUser = await userManager.FindByNameAsync(user.UserName);
        if (existingUser != null) return;
        //add user
        var res = await userManager!.CreateAsync(user,"secret");
        if(res.Succeeded)
            //addrole
           await userManager.AddToRolesAsync(user, roles);
    }

    public static int CountLetter(this string origin, char let)
    {
        int counter = 0;
        foreach (var letter in origin)
        {
            if (letter == let) counter++;
        }

        return counter;
    }

}