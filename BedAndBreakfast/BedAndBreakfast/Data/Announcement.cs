using BedAndBreakfast.Models;
using BedAndBreakfast.Settings;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BedAndBreakfast.Data
{
	public class Announcement
	{
		[Key]
		public int ID { get; set; }

		// Enumerable variables.
		[MaxLength(50)]
		public string Type { get; set; }
		[MaxLength(50)]
		public string Subtype { get; set; }
		[MaxLength(50)]
		public string SharedPart { get; set; }

		
		public Address Address { get; set; }

		// How long should announcement exist. 
		[DataType(DataType.DateTime)]
		public DateTime From { get; set; }
		[DataType(DataType.DateTime)]
		public DateTime To { get; set; }


		[MaxLength(DbRestrictionsContainer.MaxAnnouncementDescSize)]
		public string Description { get; set; }
		// TODO: store images?

		public List<AnnouncementToContact> AnnouncementToContacts { get; set; }
		public List<AnnouncementToPayment> AnnouncementToPayments { get; set; }

		public User User { get; set; }
		[MaxLength(450)]
		public string UserFK { get; set; }



	}
}
