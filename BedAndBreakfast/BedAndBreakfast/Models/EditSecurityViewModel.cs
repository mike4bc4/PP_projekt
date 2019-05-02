using BedAndBreakfast.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BedAndBreakfast.Models
{
    public class EditSecurityViewModel
    {
        [Display(Name = "Current password")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Required")]
        [Remote(controller: "Validation", action: "ValidPasswordLenght")]
        public string CurrentPassword { get; set; }

        [Display(Name = "New password")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Required")]
        [Remote(controller: "Validation", action: "ValidPasswordLenght")]
        public string NewPassword { get; set; }

        [Display(Name = "Repeat password")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "NotMatch")]
        [Remote(controller: "Validation", action: "ValidPasswordLenght")]
        public string PrepeatNewPassword { get; set; }
    }
}
