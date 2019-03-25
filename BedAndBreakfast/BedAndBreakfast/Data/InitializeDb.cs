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
        public static async Task AddAdminAccount(UserManager<User> userManager, IServiceProvider serviceProvider) {
            var adminUser = new User
            {
                UserName = ConfigContainer.adminAccounts.Admin.Login
            };

            var result = await userManager.CreateAsync(adminUser, ConfigContainer.adminAccounts.Admin.Password);
            await userManager.AddToRoleAsync(adminUser, Role.Admin);
        }
    }
}
