using Identity.Api.Data;
using Identity.Api.Models;
using Identity.Api.Services;
using Microsoft.AspNetCore.Identity;

namespace Identity.Api;

public class UsersSeed(ILogger<UsersSeed> logger, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager) : IDbSeeder<ApplicationDbContext>
{
    public async Task SeedAsync(ApplicationDbContext context)
    {
        await EnsureRolesAsync(roleManager, logger);

        var alice = await userManager.FindByNameAsync("alice");

        if (alice == null)
        {
            alice = new ApplicationUser
            {
                UserName = "alice",
                Email = "AliceSmith@email.com",
                EmailConfirmed = true,
                Id = Guid.NewGuid().ToString(),
                LastName = "Smith",
                Name = "Alice",
                PhoneNumber = "1234567890",
            };

            var result = await userManager.CreateAsync(alice, "Pass123$");

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(alice, "admin");
                if (logger.IsEnabled(LogLevel.Debug))
                {
                    logger.LogDebug("alice created and assigned admin role");
                }
            }
            else
            {
                throw new Exception(result.Errors.First().Description);
            }
        }
        else
        {
            if (logger.IsEnabled(LogLevel.Debug))
            {
                logger.LogDebug("alice already exists");
            }

            var roles = await userManager.GetRolesAsync(alice);
            if (!roles.Contains("admin"))
            {
                await userManager.AddToRoleAsync(alice, "admin");
            }
        }

        var bob = await userManager.FindByNameAsync("bob");

        if (bob == null)
        {
            bob = new ApplicationUser
            {
                UserName = "bob",
                Email = "BobSmith@email.com",
                EmailConfirmed = true,
                Id = Guid.NewGuid().ToString(),
                LastName = "Smith",
                Name = "Bob",
                PhoneNumber = "1234567890",
            };

            var result = await userManager.CreateAsync(bob, "Pass123$");

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(bob, "user");
                if (logger.IsEnabled(LogLevel.Debug))
                {
                    logger.LogDebug("bob created and assigned user role");
                }
            }
            else
            {
                throw new Exception(result.Errors.First().Description);
            }
        }
        else
        {
            if (logger.IsEnabled(LogLevel.Debug))
            {
                logger.LogDebug("bob already exists");
            }

            var roles = await userManager.GetRolesAsync(bob);
            if (!roles.Contains("user"))
            {
                await userManager.AddToRoleAsync(bob, "user");
            }
        }
    }

    private static async Task EnsureRolesAsync(RoleManager<IdentityRole> roleManager, ILogger logger)
    {
        if (!await roleManager.RoleExistsAsync("admin"))
        {
            var result = await roleManager.CreateAsync(new IdentityRole("admin"));
            if (logger.IsEnabled(LogLevel.Debug))
            {
                logger.LogDebug("admin role created");
            }

            if (!result.Succeeded)
            {
                throw new Exception(result.Errors.First().Description);
            }
        }

        if (!await roleManager.RoleExistsAsync("user"))
        {
            var result = await roleManager.CreateAsync(new IdentityRole("user"));
            if (logger.IsEnabled(LogLevel.Debug))
            {
                logger.LogDebug("user role created");
            }

            if (!result.Succeeded)
            {
                throw new Exception(result.Errors.First().Description);
            }
        }
    }
}