using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BedAndBreakfast.Data
{
    /// <summary>
    /// Entity which is used to perform many to many connection between help pages and help tags.
    /// </summary>
    public class HelpPageHelpTag
    {
        public HelpPage HelpPage { get; set; }
        public int HelpPageID { get; set; }
        public HelpTag HelpTag { get; set; }
        public int HelpTagID { get; set; }
    }
}
