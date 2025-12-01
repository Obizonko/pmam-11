using Application.Configurations;
using Domain.Idenity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DataContext.Seeds
{
    public static class IdentitySeeder
    {
        public static async Task EnsureDataSeeded(IServiceProvider serviceProvider)
        {
            await EnsureRolesSeeded(serviceProvider);
            await EnsureAdminsSeeded(serviceProvider);
        }

        private static async Task EnsureAdminsSeeded(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();
            if (await userManager.Users.AllAsync(us => us.Email != "superadmin@gmail.com"))
            {
                var admin = new AppUser
                {
                    Email = "superadmin@gmail.com",
                    UserName = "SuperAdmin",
                    EmailConfirmed = true,
                    FirstName = "SuperAdmin",
                    LastName = "SuperAdmin",
                    PhoneNumber = "+380000",
                    PhoneNumberConfirmed = true,
                    Address = "Ukraine"
                };

                await userManager.CreateAsync(admin, "Qwerty!1");
                await userManager.AddToRolesAsync(admin, new List<string> { IdentityRoles.SuperAdmin });
            }
        }

        private static async Task EnsureRolesSeeded(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<AppRole>>();
            var roles = await roleManager.Roles.ToDictionaryAsync(r => r.Name, r => r);

            foreach (var role in IdentityServerConfiguration.GetRoles())
            {
                if (!roles.TryGetValue(role.Name, out var roleEntity))
                {
                    var result = await roleManager.CreateAsync(role);
                    if (!result.Succeeded)
                        throw new Exception("Error on adding default users` role.");
                }
            }
        }
    }
}
