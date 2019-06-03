using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BedAndBreakfast.Data;
using BedAndBreakfast.Models;
using BedAndBreakfast.Models.ServicesLogic;
using BedAndBreakfast.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace BedAndBreakfast.Controllers
{


    /// <summary>
    /// This controller is used to provide all methods necessary to 
    /// perform web service administration duties.
    /// Only administrator has access to these.
    /// </summary>
    [Authorize(Roles = Role.Admin)]
    public class AdministrationController : Controller
    {
        private AppDbContext context;
        private UserManager<User> userManager;

        public AdministrationController(AppDbContext context, UserManager<User> userManager)
        {
            this.context = context;
            this.userManager = userManager;
        }

        /// <summary>
        /// Returns view with default set of users.
        /// </summary>
        /// <returns></returns>
        public IActionResult FindUser()
        {
            ViewData["users"] = AdministrationServiceLogic
                .MapUsersToViewModel(SearchEngine.FindTopUsers(context));
            return View();
        }

        /// <summary>
        /// Returns view with set of users based on previous query.
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult FindUser(FindUserViewModel viewModel)
        {
            if (SharedServiceLogic.IsFindUserViewModelEmpty(viewModel))
            {
                return RedirectToAction("FindUser");
            }
            ViewData["users"] = ViewData["users"] = AdministrationServiceLogic
                .MapUsersToViewModel(SearchEngine.FindUsersByViewModel(viewModel, context));
            return View();
        }

        /// <summary>
        /// Gets specified user from database and sends it to view to show administrator
        /// in context of which user he is working. 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="option"></param>
        /// <returns></returns>
        public IActionResult EditUser(string user, string option)
        {
            ViewData["option"] = option;
            ViewBag.Message = TempData["message"];

            User currentUser = context.Users
                    .Include(u => u.Profile)
                    .Include(u => u.Profile.Address)
                    .Where(u => u.UserName == user)
                    .Single();

            return View(AdministrationServiceLogic.MapUserToViewModel(currentUser));
        }


        /// <summary>
        /// Changes user name and email (also records in database) and then redirects
        /// to EditUser action to reload changed data. 
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> EditUsername(EditUserViewModel viewModel)
        {
            // Check just single key of model state.
            if (ModelState.GetValidationState("UserName") != ModelValidationState.Valid)
            {
                return RedirectToAction("EditUser", new { user = viewModel.UserName, option = "Username" });
            }
            try
            {
                if (await AdministrationServiceLogic.ChangeUserName(viewModel.UserName, viewModel.NewUserName, context, userManager))
                {
                    TempData["message"] = "User name change successful.";
                    return RedirectToAction("EditUser", new { user = viewModel.NewUserName, option = "Username" });
                }
                else
                {
                    TempData["message"] = "New user name is not unique.";
                    return RedirectToAction("EditUser", new { user = viewModel.UserName, option = "Username" });
                }
            }
            catch (Exception e)
            {
                TempData["message"] = e.Message;
                return RedirectToAction("EditUser", new { user = viewModel.UserName, option = "Username" });
            }
        }

        /// <summary>
        /// Changes user account locked status and then redirects
        /// to EditUser action to reload changed data. 
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> EditStatus(EditUserViewModel viewModel)
        {
            try
            {
                bool updateResult;
                if (viewModel.IsLocked)
                {
                    updateResult = await AdministrationServiceLogic.LockUser(viewModel.UserName, context);
                }
                else
                {
                    updateResult = await AdministrationServiceLogic.UnlockUser(viewModel.UserName, context);
                }
                if (updateResult == true)
                {
                    TempData["message"] = "User status change successful.";
                }
                else
                {
                    TempData["message"] = "User status not changed.";
                }
            }
            catch (Exception e)
            {
                TempData["message"] = e.Message;
                return RedirectToAction("EditUser", new { user = viewModel.UserName, option = "Status" });
            }
            return RedirectToAction("EditUser", new { user = viewModel.UserName, option = "Status" });
        }

        /// <summary>
        /// Allows administrator to change user's password.
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        public async Task<IActionResult> EditSecurity(EditUserViewModel viewModel)
        {
            // Do not perform any changes if model for passwords is not valid.
            if (ModelState.GetValidationState("NewPassword") != ModelValidationState.Valid
                || ModelState.GetValidationState("RepeatNewPassword") != ModelValidationState.Valid)
            {
                return RedirectToAction("EditUser", new { user = viewModel.UserName, option = "Security" });
            }

            // Change password.
            User currentUser = context.Users.Where(u => u.UserName == viewModel.UserName).Single();
            string resetToken = await userManager.GeneratePasswordResetTokenAsync(currentUser);
            IdentityResult passwordChangeResult = await userManager.ResetPasswordAsync(currentUser, resetToken, viewModel.NewPassword);
            if (passwordChangeResult.Succeeded)
            {
                TempData["message"] = "Password changed.";
            }
            else
            {
                TempData["message"] = "Password change error.";
            }

            return RedirectToAction("EditUser", new { user = viewModel.UserName, option = "Security" });
        }

        /// <summary>
        /// Allows administrator to view and edit help page specified by id.
        /// </summary>
        /// <param name="hPage"></param>
        /// <returns></returns>
        public async Task<IActionResult> EditHelpPage(int hPage, EditHelpPageViewModel viewModel)
        {

            //Update page before refresh.
            if (viewModel.WasEdited && ModelState.IsValid)
            {
                await AdministrationServiceLogic.UpdateHelpPage(hPage, viewModel, context);
                ViewBag.Message = "Help page saved.";
            }

            HelpPage helpPage = await context.HelpPages.FindAsync(hPage);
            List<HelpTag> helpTags = SearchEngine.FindTagsForHelpPage(hPage, context);

            string helpTagsString = string.Empty;
            foreach (HelpTag tag in helpTags)
            {
                helpTagsString += (tag.Value + " ");
            }

            ViewData["pageTags"] = helpTagsString;
            ViewData["helpPage"] = helpPage;
            return View();
        }

        public async Task<IActionResult> AddHelpPage()
        {
            HelpPage helpPage = new HelpPage
            {
                Content = "New Page",
                IsLocked = true,
                Title = "New Page"
            };

            await context.HelpPages.AddAsync(helpPage);
            int addResult = await context.SaveChangesAsync();
            if (addResult != 0)
            {
                return RedirectToAction("EditHelpPage", new { hPage = helpPage.ID });
            }
            else
            {
                return RedirectToPage("../Help/Browse", new { message = "Fail" });
            }
        }

        /// <summary>
        /// Redirects to manage conversations view.
        /// </summary>
        /// <returns></returns>
        public IActionResult ManageConversations()
        {
            return View();
        }

        /// <summary>
        /// Allows to get default amount (check settings) of users from top of database.
        /// If users db is empty 0 is returned. Note that method returns JSON objects.
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> GetTopUsers()
        {
            List<User> users = await context.Users.Include(u => u.Profile).Take(IoCContainer.DbSettings.Value.ManageConversationsDefaultListedUsers).ToListAsync();
            if (users.Count() == 0)
            {
                return Json(0);
            }
            List<FindUserViewModel> topUsers = new List<FindUserViewModel>();
            foreach (User user in users)
            {
                topUsers.Add(new FindUserViewModel()
                {
                    FirstName = user.Profile?.FirstName,
                    LastName = user.Profile?.LastName,
                    UserName = user.UserName,
                    IsLocked = user.IsLocked,
                });
            }
            return Json(topUsers);
        }

        public async Task<IActionResult> GetAllUsers() {
            List<User> users = await context.Users.Include(u => u.Profile).ToListAsync();
            List<FindUserViewModel> usersFound = new List<FindUserViewModel>();
            foreach (User user in users) {
                usersFound.Add(new FindUserViewModel()
                {
                    FirstName = user.Profile?.FirstName,
                    LastName = user.Profile?.LastName,
                    IsLocked = user.IsLocked,
                    UserName = user.UserName,
                });
            }
            return Json(usersFound);
        }

        public async Task<IActionResult> FindUsersByQuery(string query)
        {
            // If there is no query return default amount of users from top of database.
            if (query == null)
            {
                return await GetTopUsers();
            }
            // Convert query to upper case array of key words.
            List<string> normalizedQuery = query.ToUpper().Trim().Split(null).ToList();
            List<User> users = new List<User>();

            foreach (string key in normalizedQuery)
            {
                List<User> usersTmp = await context.Users.Include(u => u.Profile)
                    .Where(u => u.Profile != null)
                    .Where(u => u.UserName.ToUpper().Contains(key) ||
                    u.Profile.FirstName.ToUpper().Contains(key) ||
                    u.Profile.LastName.ToUpper().Contains(key))
                    .ToListAsync();
                users.AddRange(usersTmp);
                usersTmp = await context.Users.Include(u => u.Profile)
                    .Where(u => u.Profile == null)
                    .Where(u => u.UserName.ToUpper().Contains(key))
                    .ToListAsync();
                users.AddRange(usersTmp);
            }
            users = users.Distinct().ToList();
            List<FindUserViewModel> usersFound = new List<FindUserViewModel>();
            foreach (User user in users)
            {
                usersFound.Add(new FindUserViewModel()
                {
                    FirstName = user.Profile?.FirstName,
                    LastName = user.Profile?.LastName,
                    IsLocked = user.IsLocked,
                    UserName = user.UserName,
                });
            }
            return Json(usersFound);
        }


    }
}