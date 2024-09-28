namespace Identity.Api
{
    public class UsersSeed(ILogger<UsersSeed> logger, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager) : IDbSeeder<ApplicationDbContext>
    {
        public async Task SeedAsync(ApplicationDbContext context)
        {
            // Ensure roles exist
            await EnsureRoleExists("Admin");
            await EnsureRoleExists("User");

            // Create users
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

                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }

                if (logger.IsEnabled(LogLevel.Debug))
                {
                    logger.LogDebug("alice created");
                }
            }
            else
            {
                if (logger.IsEnabled(LogLevel.Debug))
                {
                    logger.LogDebug("alice already exists");
                }
            }
            
            await AssignRoleToUser(alice, "Admin");

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

                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }

                if (logger.IsEnabled(LogLevel.Debug))
                {
                    logger.LogDebug("bob created");
                }
            }
            else
            {
                if (logger.IsEnabled(LogLevel.Debug))
                {
                    logger.LogDebug("bob already exists");
                }
            }
            
            await AssignRoleToUser(bob, "User");
        }

        private async Task EnsureRoleExists(string roleName)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                var result = await roleManager.CreateAsync(new IdentityRole(roleName));
                if (!result.Succeeded)
                {
                    throw new Exception($"Failed to create {roleName} role.");
                }
            }
        }

        private async Task AssignRoleToUser(ApplicationUser user, string roleName)
        {
            if (!await userManager.IsInRoleAsync(user, roleName))
            {
                var result = await userManager.AddToRoleAsync(user, roleName);
                if (!result.Succeeded)
                {
                    throw new Exception($"Failed to assign {roleName} role to user {user.UserName}.");
                }
            }
        }
    }
}