using BedAndBreakfast.Settings;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BedAndBreakfast.Data
{
    /// <summary>
    /// This class represents entity for tags used for searching through help pages.
    /// </summary>
    public class HelpTag
    {
        public int ID { get; set; }

        public List<HelpPageHelpTag> HelpPageHelpTag { get; set; }

        public string Value { get; set; }

    }
}
