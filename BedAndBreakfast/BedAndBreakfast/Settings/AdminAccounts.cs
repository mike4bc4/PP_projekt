using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BedAndBreakfast.Settings
{
    /// <summary>
    /// This class is reference to settings stored in appsettings.json file.
    /// </summary>
    public class AdminAccounts
    {
        public Account Admin { get; set; }
    }

    public class Account {
        public string Login { get; set; }
        public string Password { get; set; }
    }
}
