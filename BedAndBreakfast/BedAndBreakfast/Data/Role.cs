using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BedAndBreakfast.Data
{
    /// <summary>
    /// This class contains constant string representations of roles used for authorization.
    /// </summary>
    public class Role
    {
        public const string Admin = "Administrator";
        public const string User = "BnBUser";
        public const string Host = "BnBHost";
    }
}
