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

namespace BedAndBreakfast.Controllers
{
    public class HomeController : Controller
    {

        protected AppDbContext _context;
        protected UserManager<User> _userManager;
        protected SignInManager<User> _signInManager;

        public HomeController(AppDbContext context, UserManager<User> userManager, SignInManager<User> signInManager) {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        // Sign out method.
        // Removes signed in cookie.
        public async Task<IActionResult> SignOut() {
            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
            return View();
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

            var result = await _signInManager.PasswordSignInAsync(model.Login, model.Password, true, false);

            // Verify if user exists in db.
            // Handle user sign in cookie etc.
            if (result.Succeeded)
                ViewBag.Message = $"Welcome {model.Login}";
            else
                ViewBag.Message = "Sign in failed.";


            return View();
        }


        // This method is called while user is redirected to sign up page.
        public IActionResult SignUp() {
            return View();
        }

        // Method which allows to create user and store it in db.
        // This mehod starts up while sign up button is pressed.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignUp(SignUpUserModel model) {
            // Check if form data is correct.
            if (!ModelState.IsValid) {
                // Default form messages are displayed on fail.
                return View();
            }

            // Double check - user is only creted if it fits db restrictions definied in startup options.
            var result = await _userManager.CreateAsync(new User
            {
                Email = model.EmailAddress,
                UserName = model.Login
            });


            if (result.Succeeded)
            {
                ViewBag.Message = "User creted!";
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
