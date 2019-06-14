using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BedAndBreakfast.Models
{
    public class AnnouncementViewModel
    {
        public AnnouncementPreviewModel AnnouncementPreviewModel { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public List<ContactPaymentItem> Contacts { get; set; }
        public List<ContactPaymentItem> Payments { get; set; }
        public int Timetable { get; set; }
        public int? PerDayReservations { get; set; }
        public List<ScheduleItemModel> ScheduleItems { get; set; }
    }
}
