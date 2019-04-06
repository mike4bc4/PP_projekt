using BedAndBreakfast.Data;
using BedAndBreakfast.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BedAndBreakfast.Models
{

    /// <summary>
    /// Container for searching methods based on database queries.
    /// </summary>
    public static class SearchEngine
    {
        /// <summary>
        /// Finds all help pages based on query.
        /// </summary>
        /// <param name="query">Raw string inserted by user.</param>
        /// <param name="context">Database context</param>
        /// <returns>List of pages descending by search score or null if query is incorrect.</returns>
        public static List<HelpPage> FindPagesByQueryTags(string query, AppDbContext context) {
            if (string.IsNullOrEmpty(query)) {
                return null;
            }

            // Get normalized query tags.
            List<string> queryTags = StringManager.RemoveSpecials(query.ToUpper()).Split(' ').ToList();

            // Join many to many.
            var data1 = (from ht in context.HelpTags
                         join hthp in context.HelpPageHelpTags
                         on ht.ID equals hthp.HelpTagID
                         join hp in context.HelpPages
                         on hthp.HelpPageID equals hp.ID
                         select new { ht.Value, hp.ID });

            // Select by tags.
            var data2 = data1.Where(d => queryTags.Contains(d.Value.ToUpper()));

            // Group by page ID and count search score.
            var data3 = (from d in data2
                          group d by d.ID into g
                          select new { page = g.Key, score = g.Count() })
                          .OrderByDescending(g => g.score)
                          .ToList();

            // Get pages by score.
            List<HelpPage> pagesByScore = new List<HelpPage>();
            foreach (var result in data3)
            {
                pagesByScore.Add(context.HelpPages.Find(result.page));
            }

            return pagesByScore;

        }
       

        /// <summary>
        /// Finds few pages from top of database table sorted by descending order.
        /// Page amount is related to general application settings.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static List<HelpPage> FindTopPages(AppDbContext context) {
            List<HelpPage> helpPages = context.HelpPages
                .OrderBy(hp => hp.ID).Take(GeneralSettings.DefHelpPages)
                .ToList();
            return helpPages;
        }

    }
}
