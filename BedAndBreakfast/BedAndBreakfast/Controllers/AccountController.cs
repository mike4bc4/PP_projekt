using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BedAndBreakfast.Data;
using BedAndBreakfast.Models;
using BedAndBreakfast.Models.ServicesLogic;
using BedAndBreakfast.Settings;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
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
        protected AppDbContext context;

        /// <summary>
        /// Constructor is used to handle all code injections.
        /// </summary>
        /// <param name="userManager"></param>
        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, IStringLocalizer<AccountController> localizer, AppDbContext context) {
            this.userManager = userManager;
            this.localizer = localizer;
            this.signInManager = signInManager;
            this.context = context;
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
        public IActionResult Create(CreateAccountViewModel viewModel) {
            // If basic syntax check returns false return to view with proper messages
            // defined in view model annotations.
            if (!ModelState.IsValid) {
                return View();
            }

            Profile addedProfile = UserAccountServiceLogic.CreateProfile(viewModel);
            User addedUser = addedUser = UserAccountServiceLogic.CreateUser(viewModel, addedProfile);
            addedProfile.User = addedUser;

            if (UserAccountServiceLogic.AddUserAndDependiencesToDB(addedUser, userManager, viewModel, context).Result)
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

            // Check if account is locked.
            if (UserAccountServiceLogic.IsAccountLocked(context, viewModel)) {
                return View("Locked");
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
        /// This method provides user settings to view elements and allows
        /// navigation in account settings by option parameter.
        /// </summary>
        /// <param name="option"></param>
        /// <returns></returns>
        [Authorize(Policy = Policy.LoggedIn)]
        public async Task<IActionResult> Edit(string option) {
            ViewData["option"] = option;

            // Work with database only if necessary.
            if (option == "Notifications" || option == null)
            {
                User currentUser = await userManager.GetUserAsync(HttpContext.User);
                NotificationsSetting notificationsSettings = context.NotificationSettings.Where(s => s.User == currentUser).Single();
                ViewData["notificationSettings"] = notificationsSettings;
            }
            else if (option == "Privacy")
            {
                User currentUser = await userManager.GetUserAsync(HttpContext.User);
                PrivacySetting privacySettings = context.PrivacySettings.Where(s => s.User == currentUser).Single();
                ViewData["privacySettings"] = privacySettings;
            }
            ViewBag.Message = TempData["message"];
            return View();
        }

        /// <summary>
        /// Takes returned view model and changes user settings in database
        /// then redirects to viewing action to reload account settings.
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Policy = Policy.LoggedIn)]
        public async Task<IActionResult> EditNotifications(EditNotificationsViewModel viewModel) {
            User currentUser = await userManager.GetUserAsync(HttpContext.User);
            NotificationsSetting notificationsSettings = context.NotificationSettings.Where(s => s.User == currentUser).Single();

            UserAccountServiceLogic.CopyNotificationSettings(notificationsSettings, viewModel);

            await context.SaveChangesAsync();

            return RedirectToAction("Edit");
        }

        /// <summary>
        /// Takes returned view model and changes user settings in database
        /// then redirects to viewing action to reload account settings. 
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Policy = Policy.LoggedIn)]
        public async Task<IActionResult> EditPrivacy(EditPrivacyViewModel viewModel) {
            User currentUser = await userManager.GetUserAsync(HttpContext.User);
            PrivacySetting privacySettings = context.PrivacySettings.Where(s => s.User == currentUser).Single();

            privacySettings.ShowProfileToFriends = viewModel.ShowProfileToFriends;
            privacySettings.ShowProfileToWorld = viewModel.ShowProfileToWorld;
            await context.SaveChangesAsync();

            return RedirectToAction("Edit", new { option = "Privacy"});
        }

        /// <summary>
        /// Validates and changes user password.
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = Policy.LoggedIn)]
        public async Task<IActionResult> EditSecurity(EditSecurityViewModel viewModel) {
            // If inserted data does not match validation rules redirect back.
            if (!ModelState.IsValid) {
                return RedirectToAction("Edit", new { option = "Security" });
            }

            User currentUser = await userManager.GetUserAsync(HttpContext.User);

            // Verify old password.
            PasswordVerificationResult verificationResult = userManager
                .PasswordHasher
                .VerifyHashedPassword(currentUser, currentUser.PasswordHash, viewModel.CurrentPassword);

            if (verificationResult != PasswordVerificationResult.Success) {
                TempData["message"] = "Current password does not match";
                return RedirectToAction("Edit", new { option = "Security" });
            }

            // Change password.
            string resetToken = await userManager.GeneratePasswordResetTokenAsync(currentUser);
            IdentityResult passwordChangeResult = await userManager.ResetPasswordAsync(currentUser, resetToken, viewModel.NewPassword);

            TempData["message"] = "Password changed.";

            return RedirectToAction("Edit", new { option = "Security" });
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