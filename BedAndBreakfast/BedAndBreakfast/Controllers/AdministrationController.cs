using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BedAndBreakfast.Data;
using BedAndBreakfast.Models;
using BedAndBreakfast.Models.ServicesLogic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BedAndBreakfast.Controllers
{


    /// <summary>
    /// This controller is used to provide all methods necessary to 
    /// perform web service administration duties.
    /// Only administrator has access to these.
    /// </summary>
    [Authorize(Roles = Role.Admin)]
    public class AdministrationController : Controller
    {
        private AppDbContext context;

        public AdministrationController(AppDbContext context) {
            this.context = context;
        }

        /// <summary>
        /// Returns view with default set of users.
        /// </summary>
        /// <returns></returns>
        public IActionResult FindUser() {
            ViewData["users"] = AdministrationServiceLogic
                .MapUserToViewModel(SearchEngine.FindTopUsers(context));
            return View();
        }

        /// <summary>
        /// Returns view with set of users based on previous query.
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult FindUser(FindUserViewModel viewModel) {
            if (SharedServiceLogic.IsFindUserViewModelEmpty(viewModel)) {
                return RedirectToAction("FindUser");
            }
            ViewData["users"] = ViewData["users"] = AdministrationServiceLogic
                .MapUserToViewModel(SearchEngine.FindUsersByViewModel(viewModel, context));
            return View();
        }
    }
}