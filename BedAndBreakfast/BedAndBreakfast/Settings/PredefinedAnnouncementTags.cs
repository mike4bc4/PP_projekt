using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BedAndBreakfast.Settings
{
    public class PredefinedAnnouncementTags
    {
        public List<List<string>> AnnouncementType { get; set; }
        public List<List<string>> HouseSubtype { get; set; }
        public List<List<string>> EntertainmentSubtype { get; set; }
        public List<List<string>> FoodSubtype { get; set; }
        public List<List<string>> HouseSharedPart { get; set; }
    }
}
