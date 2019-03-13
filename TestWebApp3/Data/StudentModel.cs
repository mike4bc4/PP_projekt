using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TestWebApp3.Data
{
	public class StudentModel
	{
		[Display(Name = "First Name")]
		[Required(ErrorMessage = "First Name is required.")]
		public string FirstName { get; set; }

		[Display(Name = "Last Name")]
		[Required(ErrorMessage = "Last Name is required.")]
		public string LastName { get; set; }

		[Display(Name = "Email Address")]
		[Required(ErrorMessage = "Email Address is required.")]
		[DataType(DataType.EmailAddress)]
		public string EmailAddress { get; set; }

		[Display(Name = "Confirm Email Address")]
		[Compare("EmailAddress", ErrorMessage = "Email Address and Confirm Email Addres fields do not match.")]
		public string ConfirmEmailAddres { get; set; }

		[Display(Name = "School Name")]
		public string SchoolName { get; set; }

		[Display(Name = "Password")]
		[DataType(DataType.Password)]
		[Required(ErrorMessage = "Password is required.")]
		public string Password { get; set; }

		[Display(Name = "Confirm Password")]
		[DataType(DataType.Password)]
		[Compare("Password", ErrorMessage = "Password and Confirm Password fields do not match.")]
		public string ConfirmPassword { get; set; }

	}
}
