using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;


namespace BedAndBreakfast.Models
{
    public class EditNotificationsViewModel
    {
        [Display(Name = "Mail")]
        public bool GeneralByEmail { get; set; }
        [Display(Name = "Mobile App")]
        public bool GeneralByMobileApp { get; set; }
        [Display(Name = "SMS")]
        public bool GeneralBySMS { get; set; }
        [Display(Name = "Phone")]
        public bool GeneralByPhone { get; set; }

        [Display(Name = "Mail")]
        public bool RemindByEmail { get; set; }
        [Display(Name = "Mobile App")]
        public bool RemindByMobileApp { get; set; }
        [Display(Name = "SMS")]
        public bool RemindBySMS { get; set; }
        [Display(Name = "Phone")]
        public bool RemindByPhone { get; set; }

        [Display(Name = "Mail")]
        public bool DiscountAnTipsByEmail { get; set; }
        [Display(Name = "Mobile App")]
        public bool DiscountAnTipsMobileApp { get; set; }
        [Display(Name = "SMS")]
        public bool DiscountAnTipsBySMS { get; set; }
        [Display(Name = "Phone")]
        public bool DiscountAnTipsByPhone { get; set; }

        [Display(Name = "Mail")]
        public bool RulesAndCommunityByEmail { get; set; }
        [Display(Name = "Mobile App")]
        public bool RulesAndCommunityByMobileApp { get; set; }
        [Display(Name = "SMS")]
        public bool RulesAndCommunityBySMS { get; set; }
        [Display(Name = "Phone")]
        public bool RulesAndCommunityByPhone { get; set; }


        [Display(Name = "Mail")]
        public bool ServiceByEmail { get; set; }
        [Display(Name = "Mobile App")]
        public bool ServiceByMobileApp { get; set; }
        [Display(Name = "SMS")]
        public bool ServiceBySMS { get; set; }
        [Display(Name = "Phone")]
        public bool ServiceByPhone { get; set; }
    }
}
