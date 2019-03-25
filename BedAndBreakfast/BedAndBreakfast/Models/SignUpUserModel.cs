using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BedAndBreakfast.Models
{
    public class SignUpUserModel
    {
        //protected IStringLocalizer<StringResources> localizer;
        //public SignUpUserModel(IStringLocalizer<StringResources> _localizer) {
        //    localizer = _localizer;
        //}

        [Display(Name = "Login")]
        [Required(ErrorMessage = "asd")]
        public string Login { get; set; }

        [Display(Name = "Email Address")]
        [Required(ErrorMessage = "Email Address is required.")]
        [DataType(DataType.EmailAddress)]
        public string EmailAddress { get; set; }

        [Display(Name = "Confirm Email Address")]
        [DataType(DataType.EmailAddress)]
        [Compare("EmailAddress", ErrorMessage = "Email Address and Confirm Email Address does not match.")]
        public string ConfirmEmailAddress { get; set; }

        [Display(Name = "First Name")]
        [Required(ErrorMessage = "First Name is required.")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        [Required(ErrorMessage = "Last Name is required.")]
        public string LastName { get; set; }

        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; }

        [Display(Name = "Confirm Password")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Password and Confirm Password does not match.")]
        public string ConfirmPassword { get; set; }
    }
}
