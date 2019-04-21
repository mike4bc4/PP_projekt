using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BedAndBreakfast.Data
{
	/// <summary>
	/// Represents address where event takes place or user lives in.
	/// </summary>
	public class Address
	{
		[Key]
		public int ID { get; set; }

		[MaxLength(50)]
		public string Country { get; set; }

		[MaxLength(50)]
		public string Region { get; set; }

		[MaxLength(50)]
		public string City { get; set; }

		[MaxLength(50)]
		public string Street { get; set; }

		[MaxLength(10)]
		public string StreetNumber { get; set; }

		public Profile Profile { get; set; }
	}
}
