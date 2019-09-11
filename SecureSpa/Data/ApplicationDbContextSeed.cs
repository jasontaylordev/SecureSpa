using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using SecureSpa.Models;

namespace SecureSpa.Data
{
    public class ApplicationDbContextSeed
    {
        public static async Task SeedAsync(UserManager<ApplicationUser> userManager)
        {
            var defaultUser = new ApplicationUser { UserName = "demouser@northwind", Email = "demouser@northwind" };
            await userManager.CreateAsync(defaultUser, "Pass@word1");
        }
    }
}
