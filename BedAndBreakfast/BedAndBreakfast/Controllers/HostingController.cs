using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BedAndBreakfast.Data;
using BedAndBreakfast.Models;
using BedAndBreakfast.Models.ServicesLogic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace BedAndBreakfast.Controllers
{
    public class HostingController : Controller
    {
        /// <summary>
        /// Service necessary to validate policies. 
        /// </summary>
        private IAuthorizationService authorizationService;

        public HostingController(IAuthorizationService authorizationService) {
            this.authorizationService = authorizationService;
        }

        
        public async Task<IActionResult> CreateAnnouncement() {

            var authorizationResult = await authorizationService.AuthorizeAsync(User, Policy.LoggedInUser);
            // If user is not logged in...
            if (!authorizationResult.Succeeded){
                // Page to redirect after login
                TempData["RedirectPage"] = "../Home/Index";
                return RedirectToAction("Login", "Account");
            }
			// If user is logged in...

			// Store new view model in session.
			HttpContext.Session.SetObject("viewModel", new CreateAnnouncementViewModel());
			return View();

			//return View("../Shared/UnderConstruction");
        }


		public IActionResult SetAnnouncementType(string value) {
			// Next partial view
			ViewData["announcementPart"] = "Subtype";

			// Retrieve stored view model.
			CreateAnnouncementViewModel viewModel = HttpContext.Session.GetObject<CreateAnnouncementViewModel>("viewModel");
			// Update value.
			viewModel.Type = value;
			// Save update model in session.
			HttpContext.Session.SetObject("viewModel", viewModel);
			// Update view (with new partial view) and pass view model to verify type.
			return View("CreateAnnouncement", viewModel);

			//return View("../Shared/UnderConstruction");
		}


		public IActionResult SetAnnouncementSubtype(string subtype, string sharedPart) {
			// Next partial view
			ViewData["announcementPart"] = "TimePlace";
			// Retrieve stored view model.
			CreateAnnouncementViewModel viewModel = HttpContext.Session.GetObject<CreateAnnouncementViewModel>("viewModel");
			// Update value.
			viewModel.Subtype = subtype;
			viewModel.SharedPart = sharedPart;
			// Save update model in session.
			HttpContext.Session.SetObject("viewModel", viewModel);
			return View("CreateAnnouncement", viewModel);
			//return View("../Shared/UnderConstruction");
		}




	}
}