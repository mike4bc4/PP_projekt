using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BedAndBreakfast.Models
{
    public class AnnouncementPreviewModel
    {
        public int AnnouncementID { get; set; }
        public string Country { get; set; }
        public string Region { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string StreetNumber { get; set; }
        public int Type { get; set; }
        public int Subtype { get; set; }
        public int? SharedPart { get; set; }
        public string Description { get; set; }
        public double? AverageRating { get; set; }
        public int ReviewsCount { get; set; }
    }
}
