using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BedAndBreakfast.Data;
using BedAndBreakfast.Models;
using BedAndBreakfast.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BedAndBreakfast.Controllers
{
	/// <summary>
	/// This class should be used to call methods related to help pages like searching and displaying help page.
	/// </summary>
    public class HelpController : Controller
    {
		private AppDbContext context;

        public HelpController(AppDbContext context) {
			this.context = context;
		}

		/// <summary>
		/// Displays search page. And passes default number of first help pages in database
        /// or less if there is not enough of them.
		/// </summary>
		/// <returns></returns>
		public IActionResult Browse() {
            var helpPages = context.HelpPages.ToList();
            if (helpPages.Count() > GeneralSettings.DefHelpPages)
            {
                var helpPagesShort = helpPages.Take(GeneralSettings.DefHelpPages).ToList();
                helpPages = helpPagesShort;
             
            }
            ViewData["helpPages"] = helpPages;
            return View();
		}

		/// <summary>
		/// Performs search for help pages in database.
		/// </summary>
		/// <param name="searchString"></param>
		/// <returns></returns>
        [HttpPost]
		public IActionResult Search(string query) {

            List<TagSearchEngine.Item> browsedTags = new List<TagSearchEngine.Item>();
            List<TagSearchEngine.Item> browsedItems = new List<TagSearchEngine.Item>();
            List<TagSearchEngine.Relation> itemTagRelation = new List<TagSearchEngine.Relation>();
            foreach (HelpTag helpTag in context.HelpTags) {
                browsedTags.Add(new TagSearchEngine.Item(helpTag.ID, helpTag.Value));
            }
            foreach (HelpPage helpPage in context.HelpPages)
            {
                browsedItems.Add(new TagSearchEngine.Item(helpPage.ID, null));
            }
            foreach (HelpPageHelpTag hpht in context.HelpPageHelpTags) {
                itemTagRelation.Add(new TagSearchEngine.Relation(hpht.HelpPageID, hpht.HelpTagID));
            }
            TagSearchEngine eng = new TagSearchEngine(query, browsedTags, browsedItems, itemTagRelation);

            List<int> results = eng.search();


            /*

            string normalizedQuery = query.ToUpper();

            // Get query tags as list of separate words.
            List<string> queryTags = StringManager.RemoveSpecials(normalizedQuery).Split(' ').ToList();

            // Get tags in database as list with related entities.
            List<HelpTag> helpTags = context.HelpTags.Include(ht => ht.HelpPageHelpTag).ToList();

            // Initialize search score.
            Dictionary<int, int> helpPageScores = new Dictionary<int, int>();
            List<HelpPage> helpPages = context.HelpPages.ToList();
            foreach (HelpPage helpPage in helpPages) {
                helpPageScores.Add(helpPage.ID, 0);
            }

            foreach (string queryTag in queryTags) {
                foreach (HelpTag helpTag in helpTags) {
                    if (helpTag.Value.ToUpper().Contains(queryTag)) {
                        // If asked tag matches one of tags stored in db.
                        List<HelpPageHelpTag> relations = helpTag.HelpPageHelpTag;
                        foreach (HelpPageHelpTag hpht in relations) {
                            helpPageScores[hpht.HelpPageID] += 1;
                        }
                    }
                }
            }

            // Sort dictionary by score.
            var dictionaryList = helpPageScores.ToList();
            dictionaryList.Sort((x, y) => y.Value.CompareTo(x.Value));

            // List only found pages.
            List<HelpPage> pagesByScore = new List<HelpPage>();
            foreach (var result in dictionaryList)
            {
                if (result.Value > 0)
                {
                    pagesByScore.Add(context.HelpPages.Find(result.Key));
                }
            }

            // Pass found pages to view.
            ViewData["helpPages"] = pagesByScore;

            */

            if (results == null) {
                return RedirectToAction("Browse");
            }

            // List only found pages.
            List<HelpPage> pagesByScore = new List<HelpPage>();
            foreach (var result in results)
            {
                    pagesByScore.Add(context.HelpPages.Find(result));
            }

            // Pass found pages to view.
            ViewData["helpPages"] = pagesByScore;

            return View("Browse");
		}

		/// <summary>
		/// Redirects to view with specified help page based on help page reference.
		/// </summary>
		/// <param name="pageID"></param>
		/// <returns></returns>
		public IActionResult Display(int hPage) {
            var helpPage = context.HelpPages.Find(hPage);
            ViewData["helpPage"] = helpPage;
			return View();
		}

		

    }
}