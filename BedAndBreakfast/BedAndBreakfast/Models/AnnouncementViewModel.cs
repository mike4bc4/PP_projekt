using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BedAndBreakfast.Models
{
    public class AnnouncementViewModel
    {
        public int AnnouncementID { get; set; }
        public int Type { get; set; }
        public int Subtype { get; set; }
        public int? SharedPart { get; set; }
        public string Country { get; set; }
        public string Region { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string StreetNumber { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public bool IsActive { get; set; }
        public bool IsRemoved { get; set; }
    }
}
