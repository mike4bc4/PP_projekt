using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BedAndBreakfast.Models
{
    public class ReservationViewModel
    {
        public int AnnouncementID { get; set; }
        public DateTime Date { get; set; }
        public int Reservations { get; set; }
        public int? From { get; set; }
        public int? To { get; set; }
        public int? MaxReservations { get; set; }
    }
}
