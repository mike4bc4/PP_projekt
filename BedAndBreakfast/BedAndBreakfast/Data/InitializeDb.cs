using BedAndBreakfast.Models;
using BedAndBreakfast.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BedAndBreakfast.Data
{
    /// <summary>
    /// Use this class to call methods just after basic database setup.
    /// </summary>
    public static class InitializeDb
    {
        /// <summary>
        /// Creates administrator account based on login and password stored in appsettings.json file.
        /// </summary>
        /// <param name="userManager"></param>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        public static async Task CreateAdminAccount(UserManager<User> userManager ) {
            var adminUser = new User
            {
                UserName = ConfigContainer.adminAccounts.Admin.Login
            };

            var result = await userManager.CreateAsync(adminUser, ConfigContainer.adminAccounts.Admin.Password);

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, Role.Admin);
            }
            else {
                throw new Exception("Administrator account creation exception.");
            }
        }

        /// <summary>
        /// This method should be used to create all roles necessary in this web application.
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        public static async Task CreateUserRoles(RoleManager<IdentityRole> roleManager)
        {
            IdentityResult identityResult;

            // Check if administrator, user and host exists and create them if not.
            var roleCheck = await roleManager.RoleExistsAsync(Role.Admin);
            if (!roleCheck)
            {
                identityResult = await roleManager.CreateAsync(new IdentityRole(Role.Admin));
            }

            roleCheck = await roleManager.RoleExistsAsync(Role.Host);
            if (!roleCheck)
            {
                identityResult = await roleManager.CreateAsync(new IdentityRole(Role.Host));
            }

            roleCheck = await roleManager.RoleExistsAsync(Role.User);
            if (!roleCheck)
            {
                identityResult = await roleManager.CreateAsync(new IdentityRole(Role.User));
            }

        }
    }
}
