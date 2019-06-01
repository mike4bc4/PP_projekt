using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BedAndBreakfast.Data
{
    public class ScheduleItem
    {
        [Key]
        public int ScheduleItemID { get; set; }
        public byte From { get; set; }
        public byte To { get; set; }
        public int MaxReservations { get; set; }
        public List<AnnouncementToSchedule> AnnouncementToSchedules { get; set; }
        public List<Reservation> Reservations { get; set; }
        public List<ScheduleItemToConversation> ScheduleItemToConversations { get; set; }

    }
}
