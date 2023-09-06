using Microsoft.AspNetCore.Identity;

namespace CultureWeb.Models
{
    public static class UserRoleInitailizer
    {
        public static async Task InitailizeAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

            string[] roleNames = { "Admin", "User" };

            IdentityResult roleResult;
            foreach(var role in roleNames)
            {
                var roleExists = await roleManager.RoleExistsAsync(role);

                if (!roleExists)
                {
                    roleResult = await roleManager.CreateAsync(role: new IdentityRole(role));
                }
            }

            var email = "admin@gmail.com";
            var password = "Admin123$";
            if(userManager.FindByEmailAsync(email).Result == null)
            {
                ApplicationUser user = new()
                {
                    Email = email,
                    EmailConfirmed = true,
                    UserName = email,
                    PasswordHash = password,
                    FirstName = "Admin",
                    LastName = "Admin"

                };

                IdentityResult result = userManager.CreateAsync(user, password).Result;
                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user, role: "Admin").Wait();
                }
            }

            
        }
    }
}
