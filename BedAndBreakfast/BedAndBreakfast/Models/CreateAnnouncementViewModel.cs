using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BedAndBreakfast.Models
{
	/// <summary>
	/// Allows communication between controller and view so user can place
	/// announcement with specified data.
	/// </summary>
	public class CreateAnnouncementViewModel
	{
		// Predefined enumerated values
		public string Type { get; set; }
		public string Subtype { get; set; }
		public string SharedPart { get; set; }

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

		[Display(Name = "Announcement starts")]
		[Required(ErrorMessage = "Required")]
		[DataType(DataType.DateTime)]
		public DateTime From { get; set; }

		[Display(Name = "Announcement ends")]
		[Required(ErrorMessage = "Required")]
		[Remote(controller: "Validation", action: "ValidAnnouncementDate", AdditionalFields = "From,To", ErrorMessage = "InvalidAnnouncementDate")]
		[DataType(DataType.DateTime)]
		public DateTime To { get; set; }

		[Display(Name = "Description")]
		[Required(ErrorMessage = "Required")]
		public string Description { get; set; }
		public Dictionary<string,string> AdditionalContact { get; set; }
		public Dictionary<string,string> PaymentMethod { get; set; }
	}
}
