using BedAndBreakfast.Settings;
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

        [MaxLength(DbRestrictionsContainer.MaxHelpPageTitleSize)]
        public string Title { get; set; }

        [MaxLength(DbRestrictionsContainer.MaxHelpPageSize)]
		public string Content { get; set; }
	}
}
