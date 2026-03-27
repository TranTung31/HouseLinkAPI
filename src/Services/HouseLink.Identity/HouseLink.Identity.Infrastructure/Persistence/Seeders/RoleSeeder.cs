using HouseLink.Identity.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace HouseLink.Identity.Infrastructure.Persistence.Seeders
{
    public static class RoleSeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppIdentityUser>>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<AppDbContext>>();

            // Seed roles
            var roles = Enum.GetNames<UserRole>();
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                    logger.LogInformation("Seeded role: {Role}", role);
                }
            }

            // Seed admin account mặc định (chỉ chạy nếu chưa có admin)
            const string adminEmail = "admin@houselink.vn";
            const string adminPassword = "Admin@123456";

            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser is null)
            {
                adminUser = new AppIdentityUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FullName = "HouseLink Admin",
                    PhoneNumber = "0900000000",
                    Role = UserRole.Admin,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(adminUser, adminPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, UserRole.Admin.ToString());
                    logger.LogInformation("Seeded admin account: {Email}", adminEmail);
                }
                else
                {
                    logger.LogError("Failed to seed admin: {Errors}",
                        string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }
        }
    }
}
