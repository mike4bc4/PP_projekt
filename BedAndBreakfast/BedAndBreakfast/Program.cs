using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BedAndBreakfast.Data;
using BedAndBreakfast.Models;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BedAndBreakfast
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateWebHostBuilder(args).Build();

            // Get db context and make sure db created.
            using (var scope = host.Services.CreateScope()) {
                var services = scope.ServiceProvider;

                // Perform database setup.
                try
                {
                    // Get database context from services.
                    var context = services.GetRequiredService<AppDbContext>();
                    var dbNotExists = context.Database.EnsureCreated();

                    if (dbNotExists)
                    {
                        // Create user roles.
                        // Call this method before creating any account.
                        InitializeDb.CreateUserRoles(services.GetRequiredService<RoleManager<IdentityRole>>()).Wait();

                        // Add administrators accounts if db was successfully created.
                        InitializeDb.CreateAdministratorAccounts(services.GetRequiredService<UserManager<User>>()).Wait();

                        // Create test help pages.
                        InitializeDb.CreateTestHelpPages(context).Wait();

                    }

                }
                catch (Exception e) {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(e, "Error occurred during db creation process.");
                }
            }
            host.Run();

        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
