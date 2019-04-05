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
    }
}
