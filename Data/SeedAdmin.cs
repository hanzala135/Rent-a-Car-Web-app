using Microsoft.AspNetCore.Identity;

namespace SmartCarRentACar.Data
{
    public static class SeedAdmin
    {
        public static async Task EnsureAdmin(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            string adminEmail = "admin@example.com"; 
            string adminPassword = "12345";

           
            if (!await roleManager.RoleExistsAsync("Admin"))
                await roleManager.CreateAsync(new IdentityRole("Admin"));

           
            var admin = await userManager.FindByEmailAsync(adminEmail);
            if (admin == null)
            {
                var user = new IdentityUser { UserName = adminEmail, Email = adminEmail };
                await userManager.CreateAsync(user, adminPassword);
                await userManager.AddToRoleAsync(user, "Admin");
            }
        }
    }
}
