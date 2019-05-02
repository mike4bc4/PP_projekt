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
	/// This class represents help page entity.
	/// </summary>
	public class HelpPage
	{

		public int ID { get; set; }

		public List<HelpPageHelpTag> HelpPageHelpTag { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
		public string Content { get; set; }

        /// <summary>
        /// By default each page is not locked but can
        /// be deactivated so it will not appear in
        /// users browser.
        /// </summary>
        public bool IsLocked { get; set; } = false;
    }
}
