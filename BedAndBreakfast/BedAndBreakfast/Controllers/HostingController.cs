using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BedAndBreakfast.Data;
using BedAndBreakfast.Models;
using BedAndBreakfast.Models.ServicesLogic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;

namespace BedAndBreakfast.Controllers
{
    public class HostingController : Controller
    {
        /// <summary>
        /// Service necessary to validate policies. 
        /// </summary>
        private IAuthorizationService authorizationService;
        private AppDbContext context;
        private UserManager<User> userManager;


        public HostingController(IAuthorizationService authorizationService, AppDbContext context, UserManager<User> userManager)
        {
            this.authorizationService = authorizationService;
            this.context = context;
            this.userManager = userManager;
        }

        /// <summary>
        /// Checks if caller is able to create announcement and redirects to announcement page.
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = Role.User)]
        public IActionResult CreateAnnouncement()
        {
            return View();
        }


        /// <summary>
        /// Returns specified partial view from hosting container and updates data
        /// passed from server to main view with new create announcement view model.
        /// </summary>
        /// <param name="partialViewName"></param>
        /// <param name="viewModel"></param>
        /// <returns></returns>
		public IActionResult GetPartialViewWithData(string partialViewName, CreateAnnouncementViewModel viewModel)
        {
            return PartialView("PartialViews/" + partialViewName, viewModel);
        }


        /// <summary>
        /// Validates announcement view model and puts it into database
        /// if it is correct and user is able to create announcement.
        /// Also changes user role to host if he had user role before.
        /// If view model is incorrect view data is updated with proper flag
        /// that results in rendering message.
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.User)]
        public async Task<IActionResult> SaveAnnouncement(CreateAnnouncementViewModel viewModel)
        {
            User currentUser = await userManager.GetUserAsync(HttpContext.User);
            bool announcementCorrect = HostingServiceLogic.IsAnnouncementViewModelValid(viewModel);
            if (announcementCorrect)
            {
                viewModel.IsCorrect = true;
                await HostingServiceLogic.AddAnnouncementToDatabase(viewModel, context, currentUser);
                // Change user role to host if it's his first announcement.
                if (!currentUser.isHost)
                {
                    await HostingServiceLogic.MakeUserHost(currentUser, context);
                }
            }

            return Json(new { page = ControllerExtensions.ParseViewToStringAsync(this, viewModel, "PartialViews/SaveAnnouncement", true).Result, announcementCorrect });
        }


    }
}