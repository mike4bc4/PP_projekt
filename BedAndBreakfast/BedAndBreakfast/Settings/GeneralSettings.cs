using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BedAndBreakfast.Settings
{
    /// <summary>
    /// Container for general application settings which are not related to database restrictions
    /// or predefined accounts creation.
    /// </summary>
    public static class GeneralSettings
    {
        /// <summary>
        /// Number of help pages displayed by default - without search query.
        /// </summary>
        public static int DefHelpPages { get; } = 5;

        /// <summary>
        /// Number of users displayed in administration user browser by default.
        /// </summary>
        public static int DefUsersDisplayed { get; } = 10;
    }
}
