using System;
using System.Collections.Generic;
using System.Dynamic;
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
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace BedAndBreakfast.Controllers
{
    public class AnnouncementController : Controller
    {
        /// <summary>
        /// Service necessary to validate policies. 
        /// </summary>
        private IAuthorizationService authorizationService;
        private AppDbContext context;
        private UserManager<User> userManager;


        public AnnouncementController(IAuthorizationService authorizationService, AppDbContext context, UserManager<User> userManager)
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
        public IActionResult EditAnnouncement(bool newModel = true)
        {
            dynamic model = new ExpandoObject();
            model.newModel = newModel;
            TempData["newModel"] = newModel;
            //return View("UnderConstruction");

            return View(model);
        }


        /// <summary>
        /// Returns specified partial view from hosting container.
        /// </summary>
        /// <param name="partialViewName"></param>
        /// <param name="viewModel"></param>
        /// <returns></returns>
		public IActionResult GetPartialViewWithData(string partialViewName, EditAnnouncementViewModel viewModel)
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
        public async Task<IActionResult> SaveAnnouncement(EditAnnouncementViewModel viewModel)
        {
            User currentUser = await userManager.GetUserAsync(HttpContext.User);
            bool announcementCorrect = AnnouncementServiceLogic.IsAnnouncementViewModelValid(viewModel);
            if (announcementCorrect)
            {
                viewModel.IsCorrect = true;
                bool newModel = (bool)TempData["newModel"];
                await AnnouncementServiceLogic.SaveAnnouncementToDatabase(viewModel, context, currentUser, newModel);
                // Change user role to host if it's his first announcement.
                if (!currentUser.isHost)
                {
                    await AnnouncementServiceLogic.MakeUserHost(currentUser, context);
                }
            }

            return Json(new { page = ControllerExtensions.ParseViewToStringAsync(this, viewModel, "PartialViews/SaveAnnouncement", true).Result, announcementCorrect });
        }


        [Authorize(Roles = Role.User)]
        public async Task<IActionResult> ListUserAnnouncements(string sortingMethod, string queryString)
        {

            User currentUser = await userManager.GetUserAsync(HttpContext.User);
            // Get only these announcements which were not removed by user.
            List<Announcement> usersAnnouncements = context.Announcements
                .Include(a => a.Address)
                .Where(a => a.User == currentUser)
                .Where(a => a.Removed == false)
                .ToList();

            List<EditAnnouncementViewModel> viewModel = AnnouncementServiceLogic.ParseAnnouncementsToViewModelList(usersAnnouncements, context);

            dynamic model = new ExpandoObject();
            model.announcements = viewModel;

            return View(model);
        }


        [Authorize(Roles = Role.User)]
        public async Task<IActionResult> ChangeAnnouncementsStatus(List<int> announcementsIDs, bool? areActive) {
            List<Announcement> announcements = (from a in context.Announcements
                                                where announcementsIDs.Contains(a.ID)
                                                select a).ToList();
            foreach (Announcement announcement in announcements) {
                if (areActive != null)
                    announcement.IsActive = (bool)areActive;
                else
                    announcement.Removed = true;
            }
            await context.SaveChangesAsync();
            return Json(true);
        }

        public IActionResult Browse(string annBrowserQuery)
        {
            dynamic model = new ExpandoObject();
            List<EditAnnouncementViewModel> viewModel = AnnouncementServiceLogic
                .ParseAnnouncementsToViewModelList(SearchEngine.FindAnnoucements(annBrowserQuery, context),
                context);
            model.announcements = viewModel;

            return View(model);
        }


    }
}