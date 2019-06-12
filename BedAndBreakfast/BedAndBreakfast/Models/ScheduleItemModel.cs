using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BedAndBreakfast.Models
{
    public class ScheduleItemModel
    {
        public int From { get; set; }
        public int To { get; set; }
        public int? MaxReservations { get; set; }
    }
}
