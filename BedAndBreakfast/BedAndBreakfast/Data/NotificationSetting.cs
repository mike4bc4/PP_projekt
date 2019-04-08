using BedAndBreakfast.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BedAndBreakfast.Data
{
    /// <summary>
    /// This class represents database entity which contains information about
    /// messages that are received by user.
    /// </summary>
    public class NotificationsSetting
    {   
        [Key]
        public int ID { get; set; }
        [MaxLength(450)] 
        public string UserFK { get; set; }
        public User User { get; set; }

        public bool GeneralByEmail { get; set; } = true;
        public bool GeneralByMobileApp { get; set; } = true;
        public bool GeneralBySMS { get; set; } = false;
        public bool GeneralByPhone { get; set; } = false;

        public bool RemindByEmail { get; set; } = true;
        public bool RemindByMobileApp { get; set; } = true;
        public bool RemindBySMS { get; set; } = false;
        public bool RemindByPhone { get; set; } = false;

        public bool DiscountAnTipsByEmail { get; set; } = true;
        public bool DiscountAnTipsMobileApp { get; set; } = true;
        public bool DiscountAnTipsBySMS { get; set; } = false;
        public bool DiscountAnTipsByPhone { get; set; } = false;

        public bool RulesAndCommunityByEmail { get; set; } = true;
        public bool RulesAndCommunityByMobileApp { get; set; } = true;
        public bool RulesAndCommunityBySMS { get; set; } = false;
        public bool RulesAndCommunityByPhone { get; set; } = false;

        public bool ServiceByEmail { get; set; } = true;
        public bool ServiceByMobileApp { get; set; } = true;
        public bool ServiceBySMS { get; set; } = false;
        public bool ServiceByPhone { get; set; } = false;
    }
}
