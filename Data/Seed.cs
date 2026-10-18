using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using ChatApp.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Data
{
    public class Seed
    {
        public static async Task ClearConnections(DataContext context)
        {
            context.Connections.RemoveRange(context.Connections);
            await context.SaveChangesAsync();
        }

       public static async Task SeedUsers(
    UserManager<AppUser> userManager,
    RoleManager<AppRoles> roleManager)
{
    // Create roles first
    var roles = new List<AppRoles>
    {
        new AppRoles { Name = "MEMBER" },
        new AppRoles { Name = "Admin" },
        new AppRoles { Name = "Moderator" }
    };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role.Name!))
        {
            var result = await roleManager.CreateAsync(role);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    Console.WriteLine(
                        $"Role creation error: {error.Code} - {error.Description}");
                }
            }
        }
    }

    // Don't create seed users if users already exist
    if (await userManager.Users.AnyAsync())
        return;

    var userData = await File.ReadAllTextAsync("Data/UserSeedData.json");

    var options = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true
    };

    var users = JsonSerializer.Deserialize<List<AppUser>>(userData, options);

    if (users == null)
        return;

    foreach (var user in users)
    {
        user.UserName = user.UserName?.ToLower();

        user.Created = DateTime.SpecifyKind(
            user.Created,
            DateTimeKind.Utc);

        user.LastActive = DateTime.SpecifyKind(
            user.LastActive,
            DateTimeKind.Utc);

        var result = await userManager.CreateAsync(user, "Pa$$w0rd");

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                Console.WriteLine(
                    $"User creation error: {error.Code} - {error.Description}");
            }

            continue;
        }

        await userManager.AddToRoleAsync(user, "MEMBER");
    }

    var admin = new AppUser
    {
        UserName = "admin"
    };

    var adminResult = await userManager.CreateAsync(
        admin,
        "Pa$$w0rd");

    if (adminResult.Succeeded)
    {
        await userManager.AddToRolesAsync(
            admin,
            new[] { "Admin", "Moderator" });
    }
}
    }
}