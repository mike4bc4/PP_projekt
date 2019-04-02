using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BedAndBreakfast.Settings
{
    /// <summary>
    /// This class should contain all data which is used to customize database tables
    /// and validation rules.
    /// </summary>
    public static class DbRestrictionsContainer
    {
        public static int RequiredAge = 18;
        public static DateTime MinimumBirthDate = new DateTime(1900, 1, 1);
        public const int PasswordMaxLength = 50;
        public const int PasswordMinLength = 5;
        public const int MaxTagLength = 10;
        public const int MaxHelpPageSize = 8192;
        public const int MaxHelpPageTitleSize = 512;
    }
}
