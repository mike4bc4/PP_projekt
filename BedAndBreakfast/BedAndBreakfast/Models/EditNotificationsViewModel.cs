using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BedAndBreakfast.Models
{
    public class EditNotificationsViewModel
    {
        public bool GeneralByEmail { get; set; }
        public bool GeneralByMobileApp { get; set; }
        public bool GeneralBySMS { get; set; }
        public bool GeneralByPhone { get; set; }

        public bool RemindByEmail { get; set; }
        public bool RemindByMobileApp { get; set; }
        public bool RemindBySMS { get; set; }
        public bool RemindByPhone { get; set; }

        public bool DiscountAnTipsByEmail { get; set; }
        public bool DiscountAnTipsMobileApp { get; set; }
        public bool DiscountAnTipsBySMS { get; set; }
        public bool DiscountAnTipsByPhone { get; set; }

        public bool RulesAndCommunityByEmail { get; set; }
        public bool RulesAndCommunityByMobileApp { get; set; }
        public bool RulesAndCommunityBySMS { get; set; }
        public bool RulesAndCommunityByPhone { get; set; }

        public bool ServiceByEmail { get; set; }
        public bool ServiceByMobileApp { get; set; }
        public bool ServiceBySMS { get; set; }
        public bool ServiceByPhone { get; set; }
    }
}
