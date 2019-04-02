using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BedAndBreakfast.Data;
using Microsoft.AspNetCore.Mvc;

namespace BedAndBreakfast.Controllers
{
	/// <summary>
	/// This class shoudl be used to call methods realted to help pages like searching and displaying help page.
	/// </summary>
    public class HelpController : Controller
    {
		private AppDbContext context;

		public HelpController(AppDbContext context) {
			this.context = context;
		}

		/// <summary>
		/// Displays search page.
		/// </summary>
		/// <returns></returns>
		public IActionResult Search() {
			// TODO: Pass to ViewData list of FAQ
			// View displays saerch box and FAQ

			return View();
		}

		/// <summary>
		/// Performs search for help pages in database.
		/// </summary>
		/// <param name="searchString"></param>
		/// <returns></returns>
		public IActionResult Search(string searchString) {

			// TODO: Return list of search results.
			// TODO: Redirect to view which allows to choose result (hyperling which redirects to DisplayPage action with page reference)
			return View();
		}

		/// <summary>
		/// Redirects to view with specified help page based on help page reference.
		/// </summary>
		/// <param name="pageID"></param>
		/// <returns></returns>
		public IActionResult DisplayPage(HelpPage page) {
			// TODO: Display page in suitable form.
			return View();
		}

		

    }
}