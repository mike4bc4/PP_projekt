using System;
using System.Collections.Generic;
using System.Globalization;
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
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace BedAndBreakfast
{
    public class Startup
    {

        public IConfiguration Configuration { get; set; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Get handle to json file to configure services.
            var config = Configuration.GetSection("DbSettings");

            // Map json files sections to classes.
            services.Configure<DbSettings>(config);
            services.Configure<PredefinedAccounts>(Configuration.GetSection("PredefinedAccounts"));
            services.Configure<PredefinedAnnouncementTags>(Configuration.GetSection("PredefinedAnnouncementTags"));

            // This lambda determines whether user consent for non-essential cookies is needed for a given request.
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => false;
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
                options.Password.RequiredLength = config.Get<DbSettings>().PasswordMinLength;
                options.Password.RequiredUniqueChars = 0;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
            });

            // Cookie used for authorization setup.
            services.ConfigureApplicationCookie(options =>
            {
                // Redirect to login page if signed out user tries to access private content.
                options.LoginPath = "/Account/Login";

                // Cookie expires after 7 days or after signing out.
                options.ExpireTimeSpan = TimeSpan.FromDays(7);
            });

            // Resource files localization.
            services.AddLocalization(options => options.ResourcesPath = "Resources");

            // Share resources localization based on MVC folder structure setup.
            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddViewLocalization()
                .AddDataAnnotationsLocalization(options =>
                 {
                     options.DataAnnotationLocalizerProvider = (type, factory) =>
                         factory.Create(typeof(SharedResources));
                 });

            // Add session service to allow storing data in session attributes.
            services.AddSession();

            // Here are polices defined for this web application.
            services.AddAuthorization(options =>
            {
                options.AddPolicy(Policy.LoggedInUser, policy => policy.RequireRole(Role.User));
            });

            // Configure IoC references before database setup.
            IServiceProvider serviceProvider = services.BuildServiceProvider();
            IoCContainer.DbSettings = serviceProvider.GetRequiredService<IOptions<DbSettings>>();
            IoCContainer.PredefinedAccounts = serviceProvider.GetRequiredService<IOptions<PredefinedAccounts>>();
            IoCContainer.PredefinedAnnouncementTags = serviceProvider.GetRequiredService<IOptions<PredefinedAnnouncementTags>>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider services)
        {

            // Add json files to configuration.
            var builder = new ConfigurationBuilder()
            .SetBasePath(env.ContentRootPath)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables();
            Configuration = builder.Build();

            // Use session service.
            app.UseSession();

            // Resource cultures setup.
            var supportedCultures = new[]
            {
                new CultureInfo("en-US"),
                //new CultureInfo("pl-PL"), Polish culture is off because it
                // forces to look for polish resources which are not present. 
            };

            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("en-US"),
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures
            });

            app.UseStaticFiles();


            // Use authentication in this web application.
            app.UseAuthentication();

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
