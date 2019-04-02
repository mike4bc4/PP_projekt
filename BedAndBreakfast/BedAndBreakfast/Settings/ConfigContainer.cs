using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BedAndBreakfast.Settings
{
    /// <summary>
    /// This class holds references to main settings and configuration what allows to share
    /// it around web application.
    /// </summary>
    public static class ConfigContainer
    {
        public static IConfiguration Configuration { get; set; }
        public static AdminAccounts adminAccounts { get; set; }
        public static int RequiredAge { get; set; } = 18;
        public static DateTime MinimumBirthDate { get; set; } = new DateTime(1900, 1, 1);

		// These fields have to be const to use them in validation annotations.
        public const int PasswordMaxLength = 50;
        public const int PasswordMinLength = 5;
    }
}
