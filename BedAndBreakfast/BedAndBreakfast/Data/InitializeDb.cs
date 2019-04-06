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
        /// Creates predefined administrator accounts.
        /// </summary>
        /// <param name="userManager"></param>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        public static async Task CreateAdministratorAccounts(UserManager<User> userManager ) {

            foreach (AdministartorLoginData admin in PredefinedAccountsContainer.administartorAccounts) {
                var addedUser = new User {
                    UserName = admin.login
                };

                var result = await userManager.CreateAsync(addedUser, admin.password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(addedUser, Role.Admin);
                }
                else
                {
                    throw new Exception("Administrator account creation exception.");
                }
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

        /// <summary>
        /// Adds predefined entities to message type dictionary.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static async Task CreateMsgTypeDisctionary(AppDbContext context) {
            foreach (MsgTypeDictionary type in PredefinedTablesContainer.MsgTypeDictionaries) {
                await context.AddAsync(type);
            }
            await context.SaveChangesAsync();
        }

        /// <summary>
        /// This is development feature. Use this method to populate database with test help pages.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static async Task CreateTestHelpPages(AppDbContext context) {

            List<HelpTag> helpTags = new List<HelpTag>{
                new HelpTag{Value = "help", },
                new HelpTag{Value = "page"},
                new HelpTag{Value = "first"},
                new HelpTag{Value = "second"},
                new HelpTag{Value = "1"},
                new HelpTag{Value = "2"},
                new HelpTag{Value = "test"},
                new HelpTag{Value = "check"},
            };

            List<HelpPage> helpPages = new List<HelpPage> {
                new HelpPage{Title = "Test page 3", Content = @"Lorem ipsum dolor sit amet, consectetur adipiscing elit. 
                    Proin convallis venenatis felis id faucibus. Integer sit amet orci in nibh porttitor accumsan."},
                new HelpPage{Title = "Test page 4", Content = @"Lorem ipsum dolor sit amet, consectetur adipiscing elit. 
                    Proin convallis venenatis felis id faucibus. Integer sit amet orci in nibh porttitor accumsan."},
                new HelpPage{Title = "Test page 5", Content = @"Lorem ipsum dolor sit amet, consectetur adipiscing elit. 
                    Proin convallis venenatis felis id faucibus. Integer sit amet orci in nibh porttitor accumsan."},
                new HelpPage{Title = "Test page 6", Content = @"Lorem ipsum dolor sit amet, consectetur adipiscing elit. 
                    Proin convallis venenatis felis id faucibus. Integer sit amet orci in nibh porttitor accumsan."}

            };

            HelpPage helpPage1 = new HelpPage
            {
                Title = "Test help page 1",
                Content = "Test help page 1 content",

            };

            HelpPage helpPage2 = new HelpPage
            {
                Title = "Test help page 2",
                Content = "Test help page 2 content",
            };

            List<HelpPageHelpTag> hpht = new List<HelpPageHelpTag> {
                new HelpPageHelpTag { HelpPage = helpPage1, HelpTag = helpTags[0] },
                new HelpPageHelpTag { HelpPage = helpPage1, HelpTag = helpTags[1] },
                new HelpPageHelpTag { HelpPage = helpPage1, HelpTag = helpTags[2] },
                new HelpPageHelpTag { HelpPage = helpPage1, HelpTag = helpTags[4] },
                new HelpPageHelpTag { HelpPage = helpPage1, HelpTag = helpTags[6] },
                new HelpPageHelpTag { HelpPage = helpPage1, HelpTag = helpTags[7] },
                new HelpPageHelpTag { HelpPage = helpPage2, HelpTag = helpTags[0] },
                new HelpPageHelpTag { HelpPage = helpPage2, HelpTag = helpTags[1] },
                new HelpPageHelpTag { HelpPage = helpPage2, HelpTag = helpTags[3] },
                new HelpPageHelpTag { HelpPage = helpPage2, HelpTag = helpTags[5] },
                new HelpPageHelpTag { HelpPage = helpPage2, HelpTag = helpTags[6] },
                new HelpPageHelpTag { HelpPage = helpPage2, HelpTag = helpTags[7] }
            };

            foreach (HelpTag helpTag in helpTags) {
                await context.AddAsync(helpTag);
            }
            await context.AddAsync(helpPage1);
            await context.AddAsync(helpPage2);
            foreach (HelpPageHelpTag helpPageHelpTag in hpht) {
                await context.AddAsync(helpPageHelpTag);
            }
            foreach (HelpPage helpPage in helpPages) {
                await context.AddAsync(helpPage);
            }

            await context.SaveChangesAsync();
        }
    }
}
