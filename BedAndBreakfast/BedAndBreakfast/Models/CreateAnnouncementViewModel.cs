using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BedAndBreakfast.Models
{
	/// <summary>
    /// This model is used only for communication between view and controller
    /// and does not take part in validation process so no annotations are needed.
    /// </summary>
	public class EditAnnouncementViewModel
	{
        /// <summary>
        /// Initializes model dictionaries so they do not contain null value and
        /// sets default announcement from and to date which is from tomorrow till
        /// day after.
        /// </summary>
		public EditAnnouncementViewModel() {
			From = DateTime.Today.AddDays(1);
			To = DateTime.Today.AddDays(2);
			ContactMethods = new Dictionary<string, byte>();
			PaymentMethods = new Dictionary<string, byte>();
		}

        public int ID { get; set; }
        public byte? Type { get; set; }
		public byte? Subtype { get; set; }
		public byte? SharedPart { get; set; }
		public string Country { get; set; }
		public string Region { get; set; }
		public string City { get; set; }
		public string Street { get; set; }
		public string StreetNumber { get; set; }
		public DateTime From { get; set; }
		public DateTime To { get; set; }
		public string Description { get; set; }
		public Dictionary<string, byte> ContactMethods { get; set; }
		public Dictionary<string, byte> PaymentMethods { get; set; }
        public bool IsCorrect { get; set; } = false;
        public bool IsActive { get; set; } = true;
        public bool Removed { get; set; }
        public int? Timetable { get; set; }
        public int? MaxReservations { get; set; }
        public List<ScheduleItemViewModel> ScheduleItems { get; set; }
    }

    public class ScheduleItemViewModel {
        public int From { get; set; }
        public int To { get; set; }
        public int? MaxReservations { get; set; }
    }
    
}
