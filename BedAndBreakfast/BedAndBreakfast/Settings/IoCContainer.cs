using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BedAndBreakfast.Settings
{
    public static class IoCContainer
    {
        public static IOptions<DbSettings> DbSettings { get; set; }
        public static IOptions<PredefinedAccounts> PredefinedAccounts { get; set; }
    }
}
