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
    }
}
