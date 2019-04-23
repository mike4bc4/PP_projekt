using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BedAndBreakfast.Models
{
	public class AnnouncementTypeViewModel {
		public string Type { get; set; }
	}

	public class AnnouncementSubtypeViewModel {
		[Required(ErrorMessage = "Required")]
		public string Subtype { get; set; }

		[Required(ErrorMessage = "Required")]
		public string SharedPart { get; set; }
	}

	public class AnnouncementTimePlaceViewModel {
		[Display(Name = "Announcement starts")]
		[Required(ErrorMessage = "Required")]
		[DataType(DataType.Date)]
		public DateTime From { get; set; }

		[Display(Name = "Announcement ends")]
		[Required(ErrorMessage = "Required")]
		[Remote(controller: "Validation", action: "ValidAnnouncementDate", AdditionalFields = "From,To", ErrorMessage = "InvalidAnnouncementDate")]
		[DataType(DataType.Date)]
		public DateTime To { get; set; }

		[Display(Name = "Country")]
		[Required(ErrorMessage = "Required")]
		[MaxLength(50, ErrorMessage = "TooLong")]
		public string Country { get; set; }

		[Display(Name = "Region")]
		[Required(ErrorMessage = "Required")]
		[MaxLength(50, ErrorMessage = "TooLong")]
		public string Region { get; set; }

		[Display(Name = "City")]
		[Required(ErrorMessage = "Required")]
		[MaxLength(50, ErrorMessage = "TooLong")]
		public string City { get; set; }

		[Display(Name = "Street")]
		[Required(ErrorMessage = "Required")]
		[MaxLength(50, ErrorMessage = "TooLong")]
		public string Street { get; set; }

		[Display(Name = "Street number")]
		[Required(ErrorMessage = "Required")]
		[MaxLength(10, ErrorMessage = "TooLong")]
		public string StreetNumber { get; set; }
	}

	public class AnnouncementDescriptionViewModel {
		[Display(Name = "Description")]
		[Required(ErrorMessage = "Required")]
		public string Description { get; set; }
	}

	public class AnnouncementContactViewModel {
		public AnnouncementContactViewModel() {
			AdditionalContacts = new Dictionary<string, string>();
		}
		public Dictionary<string,string> AdditionalContacts { get; set; }
	}

	public class AnnouncementPaymentViewModel {
		public AnnouncementPaymentViewModel() {
			PaymentMethods = new Dictionary<string, string>();
		}
		public Dictionary<string, string> PaymentMethods { get; set; }
	}

	public class CreateAnnouncementViewModel
	{
		public CreateAnnouncementViewModel() {
			AnnouncementTypeViewModel = new AnnouncementTypeViewModel();
			AnnouncementSubtypeViewModel = new AnnouncementSubtypeViewModel();
			AnnouncementTimePlaceViewModel = new AnnouncementTimePlaceViewModel();
			AnnouncementDescriptionViewModel = new AnnouncementDescriptionViewModel();
			AnnouncementContactViewModel = new AnnouncementContactViewModel();
			AnnouncementPaymentViewModel = new AnnouncementPaymentViewModel();
		}

		public AnnouncementTypeViewModel AnnouncementTypeViewModel { get; set; }
		public AnnouncementSubtypeViewModel AnnouncementSubtypeViewModel { get; set; }
		public AnnouncementTimePlaceViewModel AnnouncementTimePlaceViewModel { get; set; }
		public AnnouncementDescriptionViewModel AnnouncementDescriptionViewModel { get; set; }
		public AnnouncementContactViewModel AnnouncementContactViewModel { get; set; }
		public AnnouncementPaymentViewModel AnnouncementPaymentViewModel { get; set; }
	}
}
