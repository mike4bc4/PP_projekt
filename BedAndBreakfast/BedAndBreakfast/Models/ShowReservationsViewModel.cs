using BedAndBreakfast.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BedAndBreakfast.Models
{
    public class ShowReservationsViewModel
    {
        public ShowReservationsViewModel(ScheduleItem scheduleItem) {
            if (scheduleItem != null)
            {
                ScheduleItem = new ScheduleItemViewModel()
                {
                    From = scheduleItem.From,
                    To = scheduleItem.To,
                    MaxReservations = scheduleItem.MaxReservations
                };
            }
        }

        public int AnnouncementID { get; set; }
        public int AnnouncementType { get; set; }
        public int AnnouncementSubtype { get; set; }
        public int? HouseSharedPart { get; set; }
        public string Country { get; set; }
        public string Region { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string StreetNumber { get; set; }
        public DateTime Date { get; set; }
        public DateTime ReservationDate { get; set; }
        public int? ScheduleItemID { get; set; }
        public ScheduleItemViewModel ScheduleItem { get; set; } = null;
        public int Amount { get; set; }
    }
}
