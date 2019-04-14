using BedAndBreakfast.Data;
using BedAndBreakfast.Models.ServicesLogic;
using BedAndBreakfast.Settings;
using Microsoft.EntityFrameworkCore;
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
        public static List<HelpPage> FindPagesByQueryTags(string query, AppDbContext context, bool findLocked = false)
        {
            if (string.IsNullOrEmpty(query))
            {
                if (findLocked == true)
                    // If nothing to find but locked pages should be displayed
                    // return top pages with locked ones.
                    return FindTopPages(context, true);
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
            var data2 = from d1 in data1
                        from q in queryTags
                        where d1.Value.ToUpper().Contains(q)
                        select new { d1.Value, d1.ID };

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
                if (findLocked)
                {
                    pagesByScore.Add(context.HelpPages.Find(result.page));
                }
                else {
                    HelpPage page = context.HelpPages.Find(result.page);
                    if (!page.IsLocked) {
                        pagesByScore.Add(page);
                    }
                }
            }

            return pagesByScore;

        }


        /// <summary>
        /// Finds few pages from top of database table sorted by ascending order.
        /// Page amount is related to general application settings.
        /// By default locked pages are not displayed.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static List<HelpPage> FindTopPages(AppDbContext context, bool findLocked = false)
        {
            List<HelpPage> helpPages = new List<HelpPage>();
            if (findLocked == false)
            {
                helpPages = context.HelpPages
                        .Where(hp => hp.IsLocked == false)
                        .OrderBy(hp => hp.ID).Take(GeneralSettings.DefHelpPages)
                        .ToList();
            }
            else {
                helpPages = context.HelpPages
                        .OrderBy(hp => hp.ID).Take(GeneralSettings.DefHelpPages)
                        .ToList();
            }
            return helpPages;
        }

        /// <summary>
        /// Finds few users from top of database table sorted by user names.
        /// This is a deep search which also returns references to user profile.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static List<User> FindTopUsers(AppDbContext context)
        {
            List<User> users = context.Users
                .Include(u => u.Profile)
                .OrderBy(u => u.UserName).Take(GeneralSettings.DefUsersDisplayed)
                .ToList();
            return users;
        }

        /// <summary>
        /// Searches for user by data stored in view model. Filtering is done
        /// step by step (user name > first name > last name > is locked). Note
        /// that lock status is used only if it's true, else it is ignored.
        /// </summary>
        /// <param name="viewModel"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static List<User> FindUsersByViewModel(FindUserViewModel viewModel, AppDbContext context)
        {
            IQueryable<User> users = null;
            // Search by user name.
            if (viewModel.UserName != null)
            {
                users = (from u in context.Users.Include(u => u.Profile)
                         where u.NormalizedUserName.Contains(viewModel.UserName.ToUpper())
                         select u);
            }
            // Search by first name.
            if (viewModel.FristName != null)
            {
                if (users != null)
                {
                    users = (from u in users
                             where u.Profile.FirstName.ToUpper().Contains(viewModel.FristName.ToUpper())
                             select u);
                }
                else
                {
                    users = (from u in context.Users.Include(u => u.Profile)
                             where u.Profile.FirstName.ToUpper().Contains(viewModel.FristName.ToUpper())
                             select u);
                }
            }
            // Search by last name.
            if (viewModel.LastName != null)
            {
                if (users != null)
                {
                    users = (from u in users
                             where u.Profile.LastName.ToUpper().Contains(viewModel.LastName.ToUpper())
                             select u);
                }
                else
                {
                    users = (from u in context.Users.Include(u => u.Profile)
                             where u.Profile.LastName.ToUpper().Contains(viewModel.LastName.ToUpper())
                             select u);
                }
            }
            // Search by locked but only if option is used.
            if (users != null && viewModel.IsLocked)
            {
                users = (from u in users
                         where u.IsLocked == viewModel.IsLocked
                         select u);
            }
            else if (viewModel.IsLocked)
            {
                users = (from u in context.Users.Include(u => u.Profile)
                         where u.IsLocked == viewModel.IsLocked
                         select u);
            }

            // To list mapping.
            if (users != null)
            {
                return users.ToList();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Allows to find all tags related to help page specified 
        /// by id.
        /// </summary>
        /// <param name="helpPageID"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static List<HelpTag> FindTagsForHelpPage(int helpPageID, AppDbContext context)
        {
            var helpPageHelpTags = context.HelpPageHelpTags.Where(hpht => hpht.HelpPageID == helpPageID);
            List<HelpTag> helpTags = (from ht in context.HelpTags
                                      join hpht in helpPageHelpTags
                                      on ht.ID equals hpht.HelpTagID
                                      select ht).ToList();
            return helpTags;
        }





    }
}
