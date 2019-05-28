using BedAndBreakfast.Models;
using BedAndBreakfast.Models.ServicesLogic;
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
            List<User> addedAdmins = new List<User>
            {
                new User { UserName = IoCContainer.PredefinedAccounts.Value.Admin1.Login },
                new User { UserName = IoCContainer.PredefinedAccounts.Value.Admin2.Login }
            };
            await userManager.CreateAsync(addedAdmins[0], IoCContainer.PredefinedAccounts.Value.Admin1.Password);
            await userManager.CreateAsync(addedAdmins[1], IoCContainer.PredefinedAccounts.Value.Admin2.Password);

            await userManager.AddToRoleAsync(addedAdmins[0], Role.Admin);
            await userManager.AddToRoleAsync(addedAdmins[1], Role.Admin);
        }

        /// <summary>
        /// This method should be used to create all roles necessary in this web application.
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        public static async Task CreateUserRoles(RoleManager<IdentityRole> roleManager)
        {
            IdentityResult identityResult;

            identityResult = await roleManager.CreateAsync(new IdentityRole(Role.Admin));
            identityResult = await roleManager.CreateAsync(new IdentityRole(Role.User));
 
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

        /// <summary>
        /// Populates database with test users and their profiles.
        /// </summary>
        /// <param name="userManager"></param>
        /// <param name="context"></param>
        public static void CreateTestUsers(UserManager<User> userManager, AppDbContext context) {
            List<CreateAccountViewModel> usersData = new List<CreateAccountViewModel>
            {
                new CreateAccountViewModel
                {
                    BirthDate = new DateTime(1990,1,1),
                    EmailAddress = "jan@kowalski.com",
                    FirstName = "Jan",
                    LastName = "Kowalski",
                    Password = "testtest"
                },
                new CreateAccountViewModel
                {
                    BirthDate = new DateTime(1980,5,5),
                    EmailAddress = "preston@dawis.com",
                    FirstName = "Preston",
                    LastName = "Davis",
                    Password = "testtest"
                },
                new CreateAccountViewModel
                {
                    BirthDate = new DateTime(1970,8,15),
                    EmailAddress = "andrew@walles.com",
                    FirstName = "Andrew",
                    LastName = "Walles",
                    Password = "testtest"
                },
                new CreateAccountViewModel
                {
                    BirthDate = new DateTime(1992,12,1),
                    EmailAddress = "brian@may.com",
                    FirstName = "Brian",
                    LastName = "May",
                    Password = "testtest"
                },
                new CreateAccountViewModel
                {
                    BirthDate = new DateTime(1982,4,30),
                    EmailAddress = "jack@tasack.com",
                    FirstName = "Jack",
                    LastName = "Tasack",
                    Password = "testtest"
                },
                new CreateAccountViewModel
                {
                    BirthDate = new DateTime(1962,3,18),
                    EmailAddress = "james@may.com",
                    FirstName = "James",
                    LastName = "May",
                    Password = "testtest"
                },
                new CreateAccountViewModel
                {
                    BirthDate = new DateTime(1978,6,5),
                    EmailAddress = "jim@raynor.com",
                    FirstName = "Jim",
                    LastName = "Raynor",
                    Password = "testtest"
                },
                new CreateAccountViewModel
                {
                    BirthDate = new DateTime(1984,8,12),
                    EmailAddress = "sarah@kerrigan.com",
                    FirstName = "Sarah",
                    LastName = "Kerrigan",
                    Password = "testtest"
                },
                new CreateAccountViewModel
                {
                    BirthDate = new DateTime(1976,12,1),
                    EmailAddress = "varian@vrynn.com",
                    FirstName = "Varian",
                    LastName = "Vrynn",
                    Password = "testtest"
                },
                new CreateAccountViewModel
                {
                    BirthDate = new DateTime(1984,11,5),
                    EmailAddress = "jaina@proudmore.com",
                    FirstName = "Jaina",
                    LastName = "Proudmore",
                    Password = "testtest"
                },
                new CreateAccountViewModel
                {
                    BirthDate = new DateTime(1966,8,30),
                    EmailAddress = "arthas@menethil.com",
                    FirstName = "Arthas",
                    LastName = "Menethil",
                    Password = "testtest"
                },
                new CreateAccountViewModel
                {
                    BirthDate = new DateTime(1901,2,22),
                    EmailAddress = "machete@cortez.com",
                    FirstName = "Machete",
                    LastName = "Cortez",
                    Password = "testtest"
                },
                new CreateAccountViewModel
                {
                    BirthDate = new DateTime(1960,5,23),
                    EmailAddress = "peter@griffin.com",
                    FirstName = "Peter",
                    LastName = "Griffin",
                    Password = "testtest"
                },
                new CreateAccountViewModel
                {
                    BirthDate = new DateTime(1967,8,13),
                    EmailAddress = "derwan@gosciwuj.com",
                    FirstName = "Derwan",
                    LastName = "Gościwuj",
                    Password = "testtest"
                },
            };

            List<Profile> profiles = new List<Profile>();
            foreach (CreateAccountViewModel user in usersData) {
                profiles.Add(UserAccountServiceLogic.CreateProfile(user));
            }

            List<User> users = new List<User>();
            int i = 0;
            foreach (CreateAccountViewModel userData in usersData) {
                users.Add(UserAccountServiceLogic.CreateUser(userData, profiles[i]));
                i++;
            }
            i = 0;

            foreach (User user in users) {
                UserAccountServiceLogic.AddUserAndDependenciesToDB(user, userManager, usersData[i], context).Wait();
                i++;
            }
        }
    }
}
