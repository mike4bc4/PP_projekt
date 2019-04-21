using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BedAndBreakfast.Data
{
	public class AnnouncementToPayment
	{
		[Key]
		public int ID { get; set; }
		public int AnnouncementID { get; set; }
		public Announcement Announcement { get; set; }
		public int PaymentMethodID { get; set; }
		public PaymentMethod PaymentMethod { get; set; }
	}
}
