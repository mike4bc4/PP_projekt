using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BedAndBreakfast.Data
{
	/// <summary>
	/// This class represents help file section which contains title and content as text.
	/// </summary>
	public class HelpFileSection {

		public string title { get; set; }
		public string content { get; set; }

		public HelpFileSection(string title, string content) {
			this.title = title;
			this.content = content;
		}

	}

	/// <summary>
	/// This class represents help page entity.
	/// </summary>
	public class HelpPage
	{
		/// <summary>
		/// Help page ID
		/// </summary>
		public int ID { get; set; }

		/// <summary>
		/// Tags used to find help page
		/// </summary>
		public List<string> Tags { get; set; }

		/// <summary>
		/// Title of help page
		/// </summary>
		public string Title { get; set; }

		/// <summary>
		/// Sections contained in help file.
		/// </summary>
		public List<HelpFileSection> FileSections { get; set; }
	}
}
