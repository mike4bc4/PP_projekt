using BedAndBreakfast.Settings;
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
        [MinLength(DbRestrictionsContainer.PasswordMinLength, ErrorMessage = "TooShort")]
        [MaxLength(DbRestrictionsContainer.PasswordMaxLength, ErrorMessage = "TooLong")]
        public string CurrentPassword { get; set; }

        [Display(Name = "New password")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Required")]
        [MinLength(DbRestrictionsContainer.PasswordMinLength, ErrorMessage = "TooShort")]
        [MaxLength(DbRestrictionsContainer.PasswordMaxLength, ErrorMessage = "TooLong")]
        public string NewPassword { get; set; }

        [Display(Name = "Repeat password")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "NotMatch")]
        [MinLength(DbRestrictionsContainer.PasswordMinLength, ErrorMessage = "TooShort")]
        [MaxLength(DbRestrictionsContainer.PasswordMaxLength, ErrorMessage = "TooLong")]
        public string PrepeatNewPassword { get; set; }
    }
}
