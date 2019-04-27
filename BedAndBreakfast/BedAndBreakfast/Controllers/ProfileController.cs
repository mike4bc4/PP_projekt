using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BedAndBreakfast.Data;
using BedAndBreakfast.Models;
using BedAndBreakfast.Models.ServicesLogic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BedAndBreakfast.Controllers
{
    /// <summary>
    /// This class should be used to call all methods responsible for user web profile.
    /// Note that only authorized users may access this set of functions.
    /// </summary>
    [Authorize(Policy = Policy.LoggedInUser)]
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
            User userData = await userManager.GetUserAsync(HttpContext.User);
			Profile profile = context.Profiles.Include(p => p.Address).Where(p => p.User == userData).First();
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

			// Get user.
			User userData = await userManager.GetUserAsync(HttpContext.User);

            // Get user profile or null if does not exists.
			Profile profile = context.Profiles.Include(p => p.Address).Where(p => p.User == userData).First();


            Address viewModelAddress = new Address
            {
                Country = viewModel.Country,
                Region = viewModel.Region,
                City = viewModel.City,
                Street = viewModel.Street,
                StreetNumber = viewModel.StreetNumber
            };

            bool addressesSimilar = UserAccountServiceLogic.AddressesSimilarCheck(profile.Address, viewModelAddress);

            // Update address only if it has been changed by user.
            if (!addressesSimilar)
            {
                Address addressInDatabase = SearchEngine.FindAddressByContent(viewModelAddress, context);
                if (addressInDatabase != null)
                {
                    profile.Address = addressInDatabase;
                    profile.AddressFK = addressInDatabase.ID;
                }
                else {
                    // Address not found in database
                    profile.Address = viewModelAddress;
                }
            }

			// Update all profile fields.
			profile.FirstName = viewModel.FirstName;
            profile.LastName = viewModel.LastName;
            profile.Gender = viewModel.Gender;
            profile.BirthDate = viewModel.BirthDate;
            profile.PrefLanguage = viewModel.PrefLanguage;
            profile.PrefCurrency = viewModel.PrefCurrency;
            profile.PresonalDescription = viewModel.PresonalDescription;
            profile.School = viewModel.School;
            profile.Work = viewModel.Work;
            profile.BackupEmailAddress = viewModel.BackupEmailAddress;

            // Commit changes to database.
            await context.SaveChangesAsync();

            return RedirectToAction("Edit");
        }

    }
}