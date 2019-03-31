using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BedAndBreakfast.Models
{
    public class EditProfileViewModel
    {
        [Display(Name = "Your first name")]
        [Required(ErrorMessage = "Required")]
        [MaxLength(50, ErrorMessage = "TooLong")]
        public string FirstName { get; set; }

        [Display(Name = "Your last name")]
        [Required(ErrorMessage = "Required")]
        [MaxLength(50, ErrorMessage = "TooLong")]
        public string LastName { get; set; }

        // TODO: Change type to allow chose from list.
        [Display(Name = "Your gender")]
        public char? Gender { get; set; }

        // TODO: Should be this field editable only by administrator?
        [Display(Name = "Your date of birth")]
        [Required(ErrorMessage = "Required")]
        [DataType(DataType.Date, ErrorMessage = "InvalidDateFormat")]
        [Remote("ValidBirthDate", "Validation", ErrorMessage = "InvalidBirthDate")]
        public DateTime BirthDate { get; set; }

        // TODO: Change type to allow chose from list.
        [Display(Name = "Your preferred language")]
        public string PrefLanguage { get; set; }

        // TODO: Change type to allow chose from list.
        [Display(Name = "Your preferred currency")]
        [DataType(DataType.Currency)]
        public string PrefCurrency { get; set; }

        // TODO: Define validation rules for address?
        [Display(Name = "Country which you live in")]
        [MaxLength(50, ErrorMessage = "TooLong")]
        public string Country { get; set; }

        [Display(Name = "Region which you live in")]
        [MaxLength(50, ErrorMessage = "TooLong")]
        public string Region { get; set; }

        [Display(Name = "City which you live in")]
        [MaxLength(50, ErrorMessage = "TooLong")]
        public string City { get; set; }

        [Display(Name = "Street where your house is")]
        [MaxLength(50, ErrorMessage = "TooLong")]
        public string Street { get; set; }

        [Display(Name = "Street number")]
        [MaxLength(10, ErrorMessage = "TooLong")]
        public string StreetNumber { get; set; }

        [Display(Name = "Your personal description")]
        [MaxLength(1024, ErrorMessage = "TooLong")]
        public string PresonalDescription { get; set; }

        [Display(Name = "School which you attend")]
        [MaxLength(100, ErrorMessage = "TooLong")]
        public string School { get; set; }

        [Display(Name = "Your workplace")]
        [MaxLength(100, ErrorMessage = "TooLong")]
        public string Work { get; set; }

        [Display(Name = "Additional email address")]
        [DataType(DataType.EmailAddress, ErrorMessage = "InvalidEmail")]
        public string BackupEmailAddress { get; set; }
    }
}
