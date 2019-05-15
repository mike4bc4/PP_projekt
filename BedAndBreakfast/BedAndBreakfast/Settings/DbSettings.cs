using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BedAndBreakfast.Settings
{
    public class DbSettings
    {
        public int RequiredAge { get; set; }
        public DateTime MinimumBirthDate { get; set; }
        public int PasswordMaxLength { get; set; }
        public int PasswordMinLength { get; set; }
        public int MaxTagLength { get; set; }
        public int MaxHelpPageSize { get; set; }
        public int MaxAnnouncementDescSize { get; set; }
        public int MaxHelpPageTitleSize { get; set; }
        public int MaxAddressInputLength { get; set; }
        public int DefHelpPages { get; set; }
        public int DefUsersDisplayed { get; set; }
        public int DefAnnouncementsDisplayed { get; set; }
        public int MaxReviewContentLength { get; set; }
    }
}
