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
        }


		public IActionResult SetAnnouncementType(AnnouncementTypeViewModel subViewModel) {
			// Next partial view
			ViewData["announcementPart"] = "Subtype";
			// Inform another view about chosen announcement type (because of different properties of announcement form)
			ViewData["announcementType"] = subViewModel.Type;
			// Retrieve stored view model.
			CreateAnnouncementViewModel viewModel = HttpContext.Session.GetObject<CreateAnnouncementViewModel>("viewModel");
			// Update value.
			viewModel.AnnouncementTypeViewModel.Type = subViewModel.Type;
			// Save update model in session.
			HttpContext.Session.SetObject("viewModel", viewModel);
			// Update view (with new partial view) and pass view model to verify type.
			return View("CreateAnnouncement");
		}


		public IActionResult SetAnnouncementSubtype(AnnouncementSubtypeViewModel subViewModel) {
			// Next partial view
			ViewData["announcementPart"] = "TimePlace";
			// Retrieve stored view model.
			CreateAnnouncementViewModel viewModel = HttpContext.Session.GetObject<CreateAnnouncementViewModel>("viewModel");
			// Update value.
			viewModel.AnnouncementSubtypeViewModel.Subtype = subViewModel.Subtype;
			viewModel.AnnouncementSubtypeViewModel.SharedPart = subViewModel.SharedPart;
			// Save update model in session.
			HttpContext.Session.SetObject("viewModel", viewModel);
			return View("CreateAnnouncement");
		}

		public IActionResult SetAnnouncementTimePlace(AnnouncementTimePlaceViewModel subViewModel)
		{
			// Next partial view
			ViewData["announcementPart"] = "Description";
			// Retrieve stored view model.
			CreateAnnouncementViewModel viewModel = HttpContext.Session.GetObject<CreateAnnouncementViewModel>("viewModel");
			// Update value.
			viewModel.AnnouncementTimePlaceViewModel.Country = subViewModel.Country;
			viewModel.AnnouncementTimePlaceViewModel.Region = subViewModel.Region;
			viewModel.AnnouncementTimePlaceViewModel.City = subViewModel.City;
			viewModel.AnnouncementTimePlaceViewModel.Street = subViewModel.Street;
			viewModel.AnnouncementTimePlaceViewModel.StreetNumber = subViewModel.StreetNumber;
			viewModel.AnnouncementTimePlaceViewModel.From = subViewModel.From;
			viewModel.AnnouncementTimePlaceViewModel.To = subViewModel.To;
			// Save update model in session.
			HttpContext.Session.SetObject("viewModel", viewModel);
			return View("CreateAnnouncement");
		}

		public IActionResult SetAnnouncementDescription(AnnouncementDescriptionViewModel subViewModel)
		{
			// Next partial view
			ViewData["announcementPart"] = "Contact";
			// Retrieve stored view model.
			CreateAnnouncementViewModel viewModel = HttpContext.Session.GetObject<CreateAnnouncementViewModel>("viewModel");
			// Update value.
			viewModel.AnnouncementDescriptionViewModel.Description = subViewModel.Description;
			// Save update model in session.
			HttpContext.Session.SetObject("viewModel", viewModel);
			return View("CreateAnnouncement");
		}

		public IActionResult SetAnnouncementContact(AnnouncementContactViewModel subViewModel)
		{
			// Next partial view
			ViewData["announcementPart"] = "Payment";
			// Retrieve stored view model.
			CreateAnnouncementViewModel viewModel = HttpContext.Session.GetObject<CreateAnnouncementViewModel>("viewModel");
			// Reset current view model to avoid appending.
			viewModel.AnnouncementContactViewModel.AdditionalContacts = new Dictionary<string, string>();
			// Update values.
			// There is no need of null check because of model constructor which initializes dictionary.
			foreach (var contact in subViewModel.AdditionalContacts)
			{
				viewModel.AnnouncementContactViewModel.AdditionalContacts.Add(contact.Key, contact.Value);
			}
			// Save update model in session.
			HttpContext.Session.SetObject("viewModel", viewModel);
			return View("CreateAnnouncement");
		}



		public IActionResult SetAnnouncementPayment(AnnouncementPaymentViewModel subViewModel)
		{
			// Next partial view
			ViewData["announcementPart"] = "Summary";
			// Retrieve stored view model.
			CreateAnnouncementViewModel viewModel = HttpContext.Session.GetObject<CreateAnnouncementViewModel>("viewModel");
			// Reset current view model to avoid appending.
			viewModel.AnnouncementPaymentViewModel.PaymentMethods = new Dictionary<string, string>();
			// Update values.
			// There is no need of null check because of model constructor which initializes dictionary.
			foreach (var paymentMethod in subViewModel.PaymentMethods)
			{
				viewModel.AnnouncementPaymentViewModel.PaymentMethods.Add(paymentMethod.Key, paymentMethod.Value);
			}
			// Save update model in session.
			HttpContext.Session.SetObject("viewModel", viewModel);
			return View("CreateAnnouncement", viewModel);
			//return View("../Shared/UnderConstruction");
		}




	}
}