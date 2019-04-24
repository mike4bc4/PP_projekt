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

			return View();
        }

		public IActionResult GetPartialViewWithData(string partialViewName, CreateAnnouncementViewModel data) {
			ViewData["data"] = data;
			return PartialView("PartialViews/" + partialViewName);
		}

		public IActionResult SaveAnnouncement(CreateAnnouncementViewModel data) {
			// Validate received view model.
			bool announcementCorrect = true;



			ViewData["announcementCorrect"] = announcementCorrect;
			return PartialView("PartialViews/SaveAnnouncement");
		}

	}
}