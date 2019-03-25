using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BedAndBreakfast.Models;
using BedAndBreakfast.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using BedAndBreakfast.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Localization;

namespace BedAndBreakfast.Controllers
{
    public class HomeController : Controller
    {

        protected AppDbContext context;
        protected UserManager<User> userManager;
        protected SignInManager<User> signInManager;
        protected IStringLocalizer<StringResources> localizer;

        public HomeController(AppDbContext context, UserManager<User> userManager, SignInManager<User> signInManager, IStringLocalizer<StringResources> localizer) {
            this.context = context;
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.localizer = localizer;
        }


        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        // Test authorization methods.
        [Route("usr")]
        [Authorize(Roles = Role.User + "," + Role.Admin)]
        public IActionResult TestUserContenet() {
            return Content("User private area.", "text/HTML");
        }


        [Route("adm")]
        [Authorize(Roles = Role.Admin)]
        public IActionResult TestAdminContenet() {
            return Content("Administrator private area.", "text/HTML");
        }

        // Sign out method.
        // Removes signed in cookie.
        public async Task<IActionResult> SignOut() {
            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
            return RedirectToAction("Index", "Home");
        }

        // Default sing in method - called while user is redirected to login page.
        public IActionResult SignIn() {
            return View();
        }

        // This method is called while user presses sign in button with filled form.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignIn(SignInUserModel model) {
            // Remove any signed in user.
            // Just to make sure if any cookie was left.
            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);

            // Form validation.
            if (!ModelState.IsValid)
            {
                // Return simple view with form definied messages.
                return View();
            }

            var a = User.Identity;

            var result = await signInManager.PasswordSignInAsync(model.Login, model.Password, true, false);

            // Verify if user exists in db.
            // Handle user sign in cookie etc.
            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.Message = "Sign in failed.";
                return View();
            }
            
        }


        // This method is called while user is redirected to sign up page.
        public IActionResult SignUp() {
            return View();
        }

        // Method which allows to create user and store it in db.
        // This method starts up while sign up button is pressed.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignUp(SignUpUserModel model) {
            // Check if form data is correct.
            if (!ModelState.IsValid) {
                // Default form messages are displayed on fail.
                return View();
            }

            // Double check - user is only created if it fits db restrictions defined in startup options.

            var addedUser = new User
            {
                UserName = model.Login,
                Email = model.EmailAddress,
                FirstName = model.FirstName,
                LastName = model.LastName
            };

            // Create user with password specified in form.
            var result = await userManager.CreateAsync(addedUser, model.Password);

            // Add user default User role.
            await userManager.AddToRoleAsync(addedUser, Role.User);

            if (result.Succeeded)
            {
                ViewBag.Message = "User created!";
            }
            else {
                ViewBag.Message = "User creation failed!";
            }

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
