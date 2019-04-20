using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BedAndBreakfast.Data;
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

        /// <summary>
        /// This action is visible for everyone except host. Authorization is solved internally
        /// so only logged in users may access this action. Note that host is also considered as logged
        /// in user so if he will force call this action it will work similar to hosting a place or event.
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> BecomeHost() {

            var authorizationResult = await authorizationService.AuthorizeAsync(User, Policy.LoggedInUser);
            // If user is not logged in...
            if (!authorizationResult.Succeeded){
                // Page to redirect after login
                TempData["RedirectPage"] = "../Home/Index"; 
                return RedirectToAction("Login", "Account");
            }
            // If user is logged in...
            


            return View("../Shared/UnderConstruction");
        }


        [Authorize(Roles = Role.Host)]
        public async Task<IActionResult> Host() {


            return View("../Shared/UnderConstruction");
        }




    }
}