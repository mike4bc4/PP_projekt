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
        public IActionResult EditAnnouncement()
        {
            return View();
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

        [Authorize(Roles = Role.User)]
        public async Task<IActionResult> ListUserAnnouncements(string sortingMethod, string queryString)
        {

            User currentUser = await userManager.GetUserAsync(HttpContext.User);
            List<Announcement> usersAnnouncements = context.Announcements
                .Include(a => a.Address)
                .Where(a => a.User == currentUser).ToList();

            var contactData = (from ua in usersAnnouncements
                               join ac in context.AnnouncementToContacts
                               on ua.ID equals ac.AnnouncementID
                               select new { announcementID = ua.ID, contactID = ac.AdditionalContactID });

            var paymentData = (from ua in usersAnnouncements
                               join ap in context.AnnouncementToPayments
                               on ua.ID equals ap.AnnouncementID
                               select new { announcementID = ua.ID, paymentID = ap.PaymentMethodID });

            List<Dictionary<string, string>> contacts = new List<Dictionary<string, string>>();
            foreach (Announcement announcement in usersAnnouncements)
            {
                List<int> announcementsContacsIDs = (from d in contactData
                                                     where d.announcementID == announcement.ID
                                                     select d.contactID).ToList();
                List<AdditionalContact> additionalContacts = (from ac in context.AdditionalContacts
                                                              where announcementsContacsIDs.Contains(ac.ID)
                                                              select ac).ToList();
                Dictionary<string, string> announcementContacts = new Dictionary<string, string>();
                foreach (AdditionalContact additionalContact in additionalContacts)
                {
                    announcementContacts.Add(key: additionalContact.Data, value: additionalContact.Type);
                }
                contacts.Add(announcementContacts);
            }

            List<Dictionary<string, string>> payments = new List<Dictionary<string, string>>();
            foreach (Announcement announcement in usersAnnouncements)
            {
                List<int> announcementsPaymentsIDs = (from d in paymentData
                                                      where d.announcementID == announcement.ID
                                                      select d.paymentID).ToList();
                List<PaymentMethod> paymentMethods = (from pm in context.PaymentMethods
                                                      where announcementsPaymentsIDs.Contains(pm.ID)
                                                      select pm).ToList();
                Dictionary<string, string> announcementPayments = new Dictionary<string, string>();
                foreach (PaymentMethod paymentMethod in paymentMethods)
                {
                    announcementPayments.Add(key: paymentMethod.Data, value: paymentMethod.Type);
                }
                payments.Add(announcementPayments);
            }

            List<EditAnnouncementViewModel> viewModel = HostingServiceLogic.ParseAnnouncementsToViewModelList(usersAnnouncements,
                contacts, payments);

            dynamic model = new ExpandoObject();
            model.announcements = viewModel;


            //return View("UnderConstruction");

            return View(model);
        }


    }
}