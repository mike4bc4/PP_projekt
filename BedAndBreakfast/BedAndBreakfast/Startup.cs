using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BedAndBreakfast.Data;
using BedAndBreakfast.Models;
using BedAndBreakfast.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BedAndBreakfast
{
    public class Startup
    {

        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            // Setup web application configuration references.
            ConfigContainer.Configuration = configuration;
            ConfigContainer.adminAccounts = configuration.GetSection("AdminAccounts").Get<AdminAccounts>();

        }

        

        /// <summary>
        /// This method should be used to create all roles necessary in this web application.
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        public async Task CreateUserRoles(IServiceProvider serviceProvider) {

            // Get proper services to create roles.
            var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();  

            IdentityResult identityResult;

            // Check if administrator, user and host exists and create them if not.
            var roleCheck = await RoleManager.RoleExistsAsync(Role.Admin);
            if (!roleCheck) {
                identityResult = await RoleManager.CreateAsync(new IdentityRole(Role.Admin));
            }

            roleCheck = await RoleManager.RoleExistsAsync(Role.Host);
            if (!roleCheck) {
                identityResult = await RoleManager.CreateAsync(new IdentityRole(Role.Host));
            }

            roleCheck = await RoleManager.RoleExistsAsync(Role.User);
            if (!roleCheck) {
                identityResult = await RoleManager.CreateAsync(new IdentityRole(Role.User));
            }

        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            // Connect to SQL server with default name.
            services.AddDbContext<AppDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            // Add classes like sign in manager which allows to handle user log in cookies etc.
            services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>()
                // Add token provider for basic hashing functions.
                .AddDefaultTokenProviders();


            // Password constraints - development setup. 
            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 5;
                options.Password.RequiredUniqueChars = 0;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
            });

            // Cookie used for authorization setup.
            services.ConfigureApplicationCookie(options =>
            {
                // Redirect to login page if signed out user tries to access private content.
                options.LoginPath = "/SignIn";

                // Cookie expires after 7 days or after signing out.
                options.ExpireTimeSpan = TimeSpan.FromDays(7);
            });

            

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider services)
        {

            // Use authentication in this web application.
            app.UseAuthentication();

            // Add web service roles.
            CreateUserRoles(services).Wait();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
