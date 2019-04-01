using BedAndBreakfast.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BedAndBreakfast.Models
{
    public class CreateAccountViewModel
    {
        [Display(Name = "Email Address")]
        [Required(ErrorMessage = "Required")]
        [DataType(DataType.EmailAddress, ErrorMessage = "InvalidEmail")]
        public string EmailAddress { get; set; }

        [Display(Name = "First Name")]
        [Required(ErrorMessage = "Required")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        [Required(ErrorMessage = "Required")]
        public string LastName { get; set; }

        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Required")]
        [MinLength(ConfigContainer.PasswordMinLength, ErrorMessage = "TooShort")]
        [MaxLength(ConfigContainer.PasswordMaxLength, ErrorMessage = "TooLong")]
        public string Password { get; set; }

        [Display(Name = "Birth Date")]
        [DataType(DataType.Date, ErrorMessage = "EmptyBirthDate")]
        [Required(ErrorMessage = "Required")]
        [Remote(action: "ValidBirthDate", controller: "Validation", ErrorMessage = "InvalidBirthDate")]
        public DateTime BirthDate { get; set; }
    }
}
