using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BedAndBreakfast.Settings
{
    /// <summary>
    /// Container for administrator login data.
    /// </summary>
    public class AdministartorLoginData {
        public string login { get; set; }
        public string password { get; set; }
        public AdministartorLoginData(string login, string password) {
            this.login = login;
            this.password = password;
        }
    }

    /// <summary>
    /// Container for predefined user accounts that should be created on database setup.
    /// </summary>
    public static class PredefinedAccountsContainer
    {
        public static List<AdministartorLoginData> administartorAccounts = new List<AdministartorLoginData>()
        {
            new AdministartorLoginData("admin@admin.adm", "admin"),
            new AdministartorLoginData("admin2@admin.adm", "admin2")
        };
    }
}
