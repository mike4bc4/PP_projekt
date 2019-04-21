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
    public class HostingController : Controller
    {
        /// <summary>
        /// Service necessary to validate policies. 
        /// </summary>
        private IAuthorizationService authorizationService;

        public HostingController(IAuthorizationService authorizationService) {
            this.authorizationService = authorizationService;
        }

        
        public async Task<IActionResult> PublishPlace() {

            var authorizationResult = await authorizationService.AuthorizeAsync(User, Policy.LoggedInUser);
            // If user is not logged in...
            if (!authorizationResult.Succeeded){
                // Page to redirect after login
                TempData["RedirectPage"] = "../Home/Index"; 
                return RedirectToAction("Login", "Account");
            }
			// If user is logged in...

			//return View();
			




			return View("../Shared/UnderConstruction");
        }


		public IActionResult UpdateAnnouncementSection() {

			return View("../Shared/UnderConstruction");
		}




    }
}