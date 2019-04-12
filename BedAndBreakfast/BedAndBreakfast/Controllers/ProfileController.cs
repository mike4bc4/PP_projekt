using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BedAndBreakfast.Data;
using BedAndBreakfast.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BedAndBreakfast.Controllers
{
    /// <summary>
    /// This class should be used to call all methods responsible for user web profile.
    /// Note that only authorized users may access this set of functions.
    /// </summary>
    [Authorize(Policy = Policy.LoggedIn)]
    public class ProfileController : Controller
    {
        protected AppDbContext context;
        protected UserManager<User> userManager;

        public ProfileController(AppDbContext context, UserManager<User> userManager) {
            this.context = context;
            this.userManager = userManager;
        }

        /// <summary>
        /// Allows to view user data.
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Edit() {
            var userData = await userManager.GetUserAsync(HttpContext.User);
            var profile = await context.Profiles.FindAsync(userData.ProfileFK);
            ViewData["Profile"] = profile;
            return View();
        }

        /// <summary>
        /// Allows to change user data.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditProfileViewModel viewModel)
        {
            if (!ModelState.IsValid) {
                // Form validation error.
                return RedirectToAction("Edit");
            }

            // Get user profile.
            var userData = await userManager.GetUserAsync(HttpContext.User);
            var profile = await context.Profiles.FindAsync(userData.ProfileFK);

            // Update all profile fields.
            profile.FirstName = viewModel.FirstName;
            profile.LastName = viewModel.LastName;
            profile.Gender = viewModel.Gender;
            profile.BirthDate = viewModel.BirthDate;
            profile.PrefLanguage = viewModel.PrefLanguage;
            profile.PrefCurrency = viewModel.PrefCurrency;
            profile.Country = viewModel.Country;
            profile.Region = viewModel.Region;
            profile.City = viewModel.City;
            profile.Street = viewModel.Street;
            profile.StreetNumber = viewModel.StreetNumber;
            profile.PresonalDescription = viewModel.PresonalDescription;
            profile.School = viewModel.School;
            profile.Work = viewModel.Work;
            profile.BackupEmailAddress = viewModel.BackupEmailAddress;

            // Commit changes to database.
            context.SaveChanges();

            return RedirectToAction("Edit");
        }

    }
}