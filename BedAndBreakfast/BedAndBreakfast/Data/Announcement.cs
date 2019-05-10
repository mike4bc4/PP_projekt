using BedAndBreakfast.Models;
using BedAndBreakfast.Settings;
using Microsoft.AspNetCore.Mvc;
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
		public byte Type { get; set; }
		public byte Subtype { get; set; }
		public byte? SharedPart { get; set; }

		public Address Address { get; set; }

        public int AddressFK { get; set; }

        // How long should announcement exist. 
        [DataType(DataType.DateTime)]
		public DateTime From { get; set; }
		[DataType(DataType.DateTime)]
		public DateTime To { get; set; }

		public string Description { get; set; }
		// TODO: store images?

		public List<AnnouncementToContact> AnnouncementToContacts { get; set; }
		public List<AnnouncementToPayment> AnnouncementToPayments { get; set; }
        public List<AnnouncementToTag> AnnouncementToTags { get; set; }
        public List<AnnouncementToSchedule> AnnouncementToSchedules { get; set; }

        public User User { get; set; }
		[MaxLength(450)]
		public string UserFK { get; set; }

        /// <summary>
        /// Disables announcement whatever from and to date is.
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Removed announcements are not displayed to users anymore - even to owners.
        /// </summary>
        public bool Removed { get; set; } = false;

        public int? MaxReservations { get; set; }
        public byte Timetable { get; set; }

    }
}
