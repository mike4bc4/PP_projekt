using BedAndBreakfast.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BedAndBreakfast.Data
{
    public class Reservation
    {
        [Key]
        public int ReservationID { get; set; }
        public Announcement Announcement { get; set; }
        public int AnnouncementID { get; set; }
        public DateTime Date { get; set; }
        public DateTime ReservationDate { get; set; }
        public ScheduleItem ScheduleItem { get; set; }
        public int? ScheduleItemID { get; set; }
        public User User { get; set; }
        public string UserID { get; set; }

    }
}
