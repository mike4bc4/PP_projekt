using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BedAndBreakfast.Models
{
    public class LogInViewModel
    {
        [Display(Name = "Login")]
        [Required(ErrorMessage = "Required")]
        public string Login { get; set; }

        [Display(Name = "Password")]
        [Required(ErrorMessage = "Required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
