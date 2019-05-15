using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BedAndBreakfast.Models
{
    public class EditPrivacyViewModel
    {
        [Display(Name = "Show friends your profile")]
        public bool ShowProfileToFriends { get; set; }
        [Display(Name = "Show all users your profile")]
        public bool ShowProfileToWorld { get; set; }
    }
}
