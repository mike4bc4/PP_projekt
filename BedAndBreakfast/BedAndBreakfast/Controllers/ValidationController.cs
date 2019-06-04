using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BedAndBreakfast.Data;
using BedAndBreakfast.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

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
        private readonly IStringLocalizer<ValidationController> validationResources;
        private readonly IStringLocalizer<SharedResources> sharedResources;

        public ValidationController(IStringLocalizer<SharedResources> sharedResources ,IStringLocalizer<ValidationController> validationResources) {
            this.validationResources = validationResources;
            this.sharedResources = sharedResources;
        }

        /// <summary>
        /// Checks if date is between values stored in configuration container class.
        /// </summary>
        /// <param name="birthDate"></param>
        /// <returns>True or false if date is incorrect which results in displaying error message on web page form.</returns>
        public JsonResult ValidBirthDate(string birthDate) {
            var date = DateTime.Parse(birthDate);
            var asd = IoCContainer.DbSettings.Value.RequiredAge;
            return Json(date > IoCContainer.DbSettings.Value.MinimumBirthDate && date < DateTime.Today.AddYears(-IoCContainer.DbSettings.Value.RequiredAge));
        }

		/// <summary>
		/// Checks if date from is earlier than date to and in 
		/// from date is later today.
		/// </summary>
		/// <param name="from"></param>
		/// <param name="to"></param>
		/// <returns></returns>
		public JsonResult ValidAnnouncementDate(DateTime from, DateTime to) {
			if (DateTime.Compare(from, DateTime.Today) <= 0) {
				return Json(false);
			}

			if (DateTime.Compare(from, to) > 0)
			{
				return Json(false);
			}
			else if (DateTime.Compare(from, to) < 0)
			{
				return Json(true);
			}
			else {
				return Json(false);
			}
		}

        public JsonResult ValidCurrentPasswordLenght(string currentPassword) {
            if (currentPassword.Length > IoCContainer.DbSettings.Value.PasswordMaxLength) {
                return Json(sharedResources["TooLongPass"].ToString());
            }
            if (currentPassword.Length < IoCContainer.DbSettings.Value.PasswordMinLength) {
                var asd = sharedResources["TooShort"];
                return Json(sharedResources["TooShortPass"].ToString());
            }
            return Json(true);
        }

        public JsonResult ValidNewPasswordLength(string newPassword)
        {
            if (newPassword.Length > IoCContainer.DbSettings.Value.PasswordMaxLength)
            {
                return Json(sharedResources["TooLongPass"].ToString());
            }
            if (newPassword.Length < IoCContainer.DbSettings.Value.PasswordMinLength)
            {
                var asd = sharedResources["TooShort"];
                return Json(sharedResources["TooShortPass"].ToString());
            }
            return Json(true);
        }

        public JsonResult ValidRepeatNewPasswordLength(string repeatNewPassword)
        {
            if (repeatNewPassword.Length > IoCContainer.DbSettings.Value.PasswordMaxLength)
            {
                return Json(sharedResources["TooLongPass"].ToString());
            }
            if (repeatNewPassword.Length < IoCContainer.DbSettings.Value.PasswordMinLength)
            {
                var asd = sharedResources["TooShort"];
                return Json(sharedResources["TooShortPass"].ToString());
            }
            return Json(true);
        }

        public JsonResult ValidTagLength(string tagValue) {
            return Json(!(tagValue.Length > IoCContainer.DbSettings.Value.MaxTagLength));
        }

        public JsonResult ValidHelpPageSize(string content) {
            if (content.Length > IoCContainer.DbSettings.Value.MaxHelpPageSize) {
                return Json(sharedResources["TooLong"].ToString());
            }
            return Json(true);
        }

        public JsonResult ValidAnnoucnement(string tagValue)
        {
            return Json(!(tagValue.Length > IoCContainer.DbSettings.Value.MaxTagLength));
        }

        public JsonResult ValidHelpPageTitleLength(string title) {
            if (title.Length > IoCContainer.DbSettings.Value.MaxHelpPageTitleSize)
            {
                return Json(sharedResources["TooLong"].ToString());
            }
            else {
                return Json(true);
            }
        }


        public IActionResult IsUserInAdminRole()
        {
            return Json(HttpContext.User.IsInRole(Role.Admin));
        }


    }
}