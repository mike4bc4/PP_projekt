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
    [Authorize(Roles = Role.User + "," + Role.Host)]
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
            // TODO: Change record in database.

            return RedirectToAction("Edit");
        }

    }
}