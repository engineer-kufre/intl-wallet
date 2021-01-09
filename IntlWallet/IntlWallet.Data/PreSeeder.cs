using IntlWallet.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntlWallet.Data
{
    public static class PreSeeder
    {
        public static async Task Seeder(AppDbContext ctx, RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {

            ctx.Database.EnsureCreated();
            if (!roleManager.Roles.Any())
            {
                var listOfRoles = new List<IdentityRole>
                {
                    new IdentityRole("Admin"),
                    new IdentityRole("Elite"),
                    new IdentityRole("Noob")
                };
                foreach (var role in listOfRoles)
                {
                    await roleManager.CreateAsync(role);
                }
            }

            ApplicationUser seededAdmin;
            if (!userManager.Users.Any())
            {
                seededAdmin = new ApplicationUser
                {
                    Id = Guid.NewGuid().ToString(),
                    UserName = "admin@admin.com",
                    Email = "admin@admin.com",
                    LastName = "Admin",
                    FirstName = "Administrator",
                    PhoneNumber = "+2348033456789"
                };

                var result = await userManager.CreateAsync(seededAdmin, "01234Admin");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(seededAdmin, "Admin");
                }
            }
        }
    }
}
