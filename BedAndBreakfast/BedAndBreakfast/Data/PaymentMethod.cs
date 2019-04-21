using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BedAndBreakfast.Data
{
	public class PaymentMethod
	{
		[Key]
		public int ID { get; set; }
		/// <summary>
		/// Type of payment e.g. Paypal or bank transfer.
		/// </summary>
		[MaxLength(100)]
		public string Type { get; set; }
		/// <summary>
		/// Data related to payment method like account number or paypal link.
		/// </summary>
		[MaxLength(200)]
		public string Data { get; set; }
		//TODO: hash account number?

		public List<AnnouncementToPayment> AnnouncementToPayments { get; set; }


	}
}
