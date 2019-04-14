using BedAndBreakfast.Settings;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BedAndBreakfast.Models
{
    public class EditHelpPageViewModel
    {
        public bool WasEdited { get; set; }

        [Display(Name = "Is locked")]
        public bool IsLocked { get; set; }

        [Display(Name = "Tags")]
        [Required(ErrorMessage = "Required")]
        public string Tags { get; set; }

        [Display(Name = "Title")]
        [MaxLength(DbRestrictionsContainer.MaxHelpPageTitleSize, ErrorMessage = "TooLong")]
        [Required(ErrorMessage = "Required")]
        public string Title { get; set; }

        [Display(Name = "Content")]
        [MaxLength(DbRestrictionsContainer.MaxHelpPageSize, ErrorMessage = "TooLong")]
        [Required(ErrorMessage = "Required")]
        public string Content { get; set; }
    }
}
