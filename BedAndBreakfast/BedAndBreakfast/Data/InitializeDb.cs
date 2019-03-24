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
    public static class InitializeDb
    {
        public static async Task AddAdminAccount(UserManager<User> userManager, IServiceProvider serviceProvider) {
            var adminUser = new User
            {
                UserName = ConfigContainer.adminAccounts.Admin1.Login
            };

            var result = await userManager.CreateAsync(adminUser, ConfigContainer.adminAccounts.Admin1.Password);
            await userManager.AddToRoleAsync(adminUser, Role.Admin);
        }
    }
}
