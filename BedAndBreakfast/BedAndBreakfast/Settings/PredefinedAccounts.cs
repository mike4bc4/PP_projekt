using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BedAndBreakfast.Settings
{
    public class AdminAccount {
        public string Login { get; set; }
        public string Password { get; set; }
    }

    public class PredefinedAccounts
    {
        public AdminAccount Admin1 { get; set; }
        public AdminAccount Admin2 { get; set; }
    }
}
