using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BedAndBreakfast.Models.ServicesLogic
{
	/// <summary>
	/// Container for methods used in actions of hosting controller.
	/// </summary>
	public static class HostingServiceLogic
	{

		/// <summary>
		/// Compares values stored in old an new view model and replaces
		/// old ones if changed. If old model is null new object is created.
		/// </summary>
		/// <param name="oldViewModel"></param>
		/// <param name="newViewModel"></param>
		/// <returns></returns>
		public static CreateAnnouncementViewModel UpdateCreateAnnouncementViewModel(CreateAnnouncementViewModel oldViewModel, CreateAnnouncementViewModel newViewModel) {
			CreateAnnouncementViewModel resultViewModel;
			if (oldViewModel == null) {
				resultViewModel = new CreateAnnouncementViewModel();
			}
			else {
				resultViewModel = oldViewModel;
			}

			resultViewModel.Type = (resultViewModel.Type != newViewModel.Type) ? newViewModel.Type : resultViewModel.Type;
			resultViewModel.Subtype = (resultViewModel.Subtype != newViewModel.Subtype) ? newViewModel.Subtype : resultViewModel.Subtype;
			resultViewModel.SharedPart = (resultViewModel.SharedPart != newViewModel.SharedPart) ? newViewModel.SharedPart : resultViewModel.SharedPart;
			resultViewModel.Country = (resultViewModel.Country != newViewModel.Country) ? newViewModel.Country : resultViewModel.Country;
			resultViewModel.Region = (resultViewModel.Region != newViewModel.Region) ? newViewModel.Region : resultViewModel.Region;
			resultViewModel.City = (resultViewModel.City != newViewModel.City) ? newViewModel.City : resultViewModel.City;
			resultViewModel.Street = (resultViewModel.Street != newViewModel.Street) ? newViewModel.Street : resultViewModel.Street;
			resultViewModel.StreetNumber = (resultViewModel.StreetNumber != newViewModel.StreetNumber) ? newViewModel.StreetNumber : resultViewModel.StreetNumber;
			resultViewModel.From = (resultViewModel.From != newViewModel.From) ? newViewModel.From : resultViewModel.From;
			resultViewModel.To = (resultViewModel.To != newViewModel.To) ? newViewModel.To : resultViewModel.To;
			resultViewModel.Description = (resultViewModel.Description != newViewModel.Description) ? newViewModel.Description : resultViewModel.Description;
			resultViewModel.AdditionalContact = (resultViewModel.AdditionalContact != newViewModel.AdditionalContact) ? newViewModel.AdditionalContact : resultViewModel.AdditionalContact;
			resultViewModel.PaymentMethod = (resultViewModel.PaymentMethod != newViewModel.PaymentMethod) ? newViewModel.PaymentMethod : resultViewModel.PaymentMethod;

			return resultViewModel;
		}
	}
}
