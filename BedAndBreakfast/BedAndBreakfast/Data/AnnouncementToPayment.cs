using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BedAndBreakfast.Data
{
	public class AnnouncementToPayment
	{
		public int AnnouncementID { get; set; }
		public Announcement Announcement { get; set; }
		public int PaymentMethodID { get; set; }
		public PaymentMethod PaymentMethod { get; set; }
	}
}
