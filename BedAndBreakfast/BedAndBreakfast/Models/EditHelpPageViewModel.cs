using BedAndBreakfast.Settings;
using Microsoft.AspNetCore.Mvc;
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
        [Remote(controller: "Validation", action: "ValidHelpPageTitleLength")]
        [Required(ErrorMessage = "Required")]
        public string Title { get; set; }

        [Display(Name = "Content")]
        [Remote(controller: "Validation", action: "ValidHelpPageSize")]
        [Required(ErrorMessage = "Required")]
        public string Content { get; set; }
    }
}
