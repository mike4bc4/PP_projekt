using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BedAndBreakfast.Settings
{
    public class AdminAccounts
    {
        public Account Admin1 { get; set; }
    }

    public class Account {
        public string Login { get; set; }
        public string Password { get; set; }
    }
}
