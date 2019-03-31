using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BedAndBreakfast.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace BedAndBreakfast.Controllers
{
    /// <summary>
    /// This class should be used to call validation methods for jQuery data validation.
    /// </summary>
    public class ValidationController : Controller
    {
        /// <summary>
        /// Resource file handle.
        /// </summary>
        protected IStringLocalizer<ValidationController> localizer;

        public ValidationController(IStringLocalizer<ValidationController> localizer) {
            this.localizer = localizer;
        }

        /// <summary>
        /// Checks if date is between values stored in configuration container class.
        /// </summary>
        /// <param name="birthDate"></param>
        /// <returns>True or false if date is incorrect which results in displaying error message on web page form.</returns>
        public JsonResult ValidBirthDate(string birthDate) {
            var date = DateTime.Parse(birthDate);
            return Json(date > ConfigContainer.MinimumBirthDate && date < DateTime.Today.AddYears(-ConfigContainer.RequiredAge));
        }
    }
}