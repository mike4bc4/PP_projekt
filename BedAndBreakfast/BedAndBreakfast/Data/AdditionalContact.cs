using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BedAndBreakfast.Data
{
	/// <summary>
	/// Represents additional contact related to specified announcement.
	/// </summary>
	public class AdditionalContact
	{
		[Key]
		public int ID { get; set; }
		[MaxLength(100)]
		public string Type { get; set; }
		[MaxLength(200)]
		public string Data { get; set; }
		public List<AnnouncementToContact> AnnouncementToContacts { get; set; }
	}
}
