using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BedAndBreakfast.Models
{
    /// <summary>
    /// This view model is used to pass data about user context to administrator.
    /// In edit user view it is readonly.
    /// </summary>
    public class EditUserContextViewModel
    {
        [Display(Name = "User name/Email address")]
        public string UserName { get; set; }

        [Display(Name = "First name")]
        public string FirstName { get; set; }

        [Display(Name = "Last name")]
        public string LastName { get; set; }

        [Display(Name = "Gender")]
        public char? Gender { get; set; }

        // As it's readonly date is presented as string which is short date value.
        [Display(Name = "Birth date")]
        public string BirthDate { get; set; }

        [Display(Name = "Full address")]
        public string FullAddress { get; set; }

        [Display(Name = "Backup email address")]
        public string BackupEmail { get; set; }

        [Display(Name = "Is locked")]
        public bool IsLocked { get; set; }
    }
}
