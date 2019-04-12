using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BedAndBreakfast.Models
{
    public class FindUserViewModel
    {
        [Display(Name = "First name")]
        [MaxLength(250, ErrorMessage = "TooLong")]
        public string FristName { get; set; }

        [Display(Name = "Last name")]
        [MaxLength(250, ErrorMessage = "TooLong")]
        public string LastName { get; set; }

        [Display(Name = "User name")]
        [MaxLength(250, ErrorMessage = "TooLong")]
        public string UserName { get; set; }

        [Display(Name = "Account locked")]
        public bool IsLocked { get; set; }
    }
}
