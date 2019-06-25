using BedAndBreakfast.Settings;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BedAndBreakfast.Models
{
    /// <summary>
    /// This view model is used to transport data between administration controller
    /// and views related to it. 
    /// </summary>
    public class EditUserViewModel
    {
        /// <summary>
        /// This field is used as readonly in view to pass data about edited user in case of
        /// user name change.
        /// </summary>
        public string UserName { get; set; }

        [Display(Name = "User name/Email address")]
        [DataType(DataType.EmailAddress, ErrorMessage = "EmailAddressError")]
        [Required(ErrorMessage = "Required")]
        public string NewUserName { get; set; }

        [Display(Name = "New password")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Required")]
        [Remote(controller: "Validation", action: "ValidNewPasswordLength")]
        public string NewPassword { get; set; }

        [Compare("NewPassword", ErrorMessage = "PassNotMatch")]
        [Display(Name = "Repeat new password")]
        [DataType(DataType.Password)]
        public string RepeatNewPassword { get; set; }

        [Display(Name = "Is locked")]
        public bool IsLocked { get; set; }
    }
}
