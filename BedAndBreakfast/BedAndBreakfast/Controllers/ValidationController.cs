using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BedAndBreakfast.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace BedAndBreakfast.Controllers
{
    public class ValidationController : Controller
    {

        protected IStringLocalizer<ValidationController> localizer;

        public ValidationController(IStringLocalizer<ValidationController> localizer) {
            this.localizer = localizer;
        }

        public JsonResult ValidBirthDate(string birthDate) {
            var date = DateTime.Parse(birthDate);
            return Json(date > ConfigContainer.MinimumBirthDate && date < DateTime.Today.AddYears(-ConfigContainer.RequiredAge));
        }
    }
}