using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BedAndBreakfast.Data;
using BedAndBreakfast.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace BedAndBreakfast.Controllers
{
    /// <summary>
    /// This class should be used to handle all actions related to account management -
    /// which does not refer to profile management (except basic user sign up data like name and last name).
    /// 
    /// </summary>
    public class AccountController : Controller
    {
        protected UserManager<User> userManager;
        protected IStringLocalizer<AccountController> localizer;
        protected SignInManager<User> signInManager;
        /// <summary>
        /// Constructor is used to handle all code injections.
        /// </summary>
        /// <param name="userManager"></param>
        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, IStringLocalizer<AccountController> localizer) {
            this.userManager = userManager;
            this.localizer = localizer;
            this.signInManager = signInManager;
        }


        /// <summary>
        /// This method is called if user is redirected to create account page
        /// but has not send any request so data validation has not occurred.
        /// </summary>
        /// <returns></returns>
        public IActionResult Create() {
            return View();
        }

        /// <summary>
        /// This method is called if user sends request of account creation.
        /// Handles basic syntax verification but also db restrictions etc.
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateAccountViewModel viewModel) {
            // If basic syntax check returns false return to view with proper messages
            // defined in view model annotations.
            if (!ModelState.IsValid) {
                return View();
            }

            // New profile entity related to user
            var addedProfile = new Profile
            {
                FirstName = viewModel.FirstName,
                LastName = viewModel.LastName,
                BirthDate = viewModel.BirthDate
            };

            // New user entity
            var addedUser = new User {
                UserName = viewModel.EmailAddress,
                Email = viewModel.EmailAddress,
                Profile = addedProfile
            };

            addedProfile.User = addedUser;
       
            // Try to add user to database.
            var result = await userManager.CreateAsync(addedUser, viewModel.Password);

            // Try to add to user role.
            await userManager.AddToRoleAsync(addedUser, Role.User);

            if (result.Succeeded)
            {
                ViewBag.Message = localizer["CreateSuccess"];
                return RedirectToAction("Login");
            }
            else {
                ViewBag.Message = localizer["CreateFail"];
                return View();
            }
        }

        /// <summary>
        /// This method is called when user is redirected to login page without 
        /// sending login request with login and password.
        /// </summary>
        /// <returns></returns>
        public IActionResult Login() {
            return View();
        }

        /// <summary>
        /// This method is called if user sends request to log in via log in form.
        /// It verifies form syntax and generates cookie token for user.
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LogInViewModel viewModel) {
            // If basic syntax check returns false return to view with proper messages
            // defined in view model annotations.
            if (!ModelState.IsValid)
            {
                return View();
            }
            var result = await signInManager.PasswordSignInAsync(viewModel.Login, viewModel.Password, true, false);

            // Redirect to home page if everything is fine.
            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }
            else {
                ViewBag.Message = localizer["LoginFail"];
                return View();
            }
        }

        /// <summary>
        /// Logs user out then redirects to home page.
        /// Signing out is based on cookie authorization scheme.
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Logout() {
            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
            return RedirectToAction("Index", "Home");
        }

    }
}