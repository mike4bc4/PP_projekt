using BedAndBreakfast.Data;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BedAndBreakfast.Models.ServicesLogic
{
    /// <summary>
    /// Container for methods used by administration services defined in controller. 
    /// </summary>
    public static class AdministrationServiceLogic
    {
        /// <summary>
        /// Maps user data from database to view model which is safer.
        /// </summary>
        /// <param name="users"></param>
        /// <returns></returns>
        public static List<FindUserViewModel> MapUsersToViewModel(List<User> users) {
            List<FindUserViewModel> viewModelUsers = new List<FindUserViewModel>();
            foreach (User user in users) {
                FindUserViewModel viewModelUser = new FindUserViewModel();
                if (user.Profile != null)
                {
                    viewModelUser = new FindUserViewModel
                    {
                        FristName = user.Profile.FirstName,
                        LastName = user.Profile.LastName,
                        UserName = user.UserName,
                        IsLocked = user.IsLocked
                    };
                }
                else {
                    // Note that administrator does not have profile.
                    viewModelUser = new FindUserViewModel
                    {
                        UserName = user.UserName,
                        IsLocked = user.IsLocked
                    };
                }

                viewModelUsers.Add(viewModelUser);
            }
            return viewModelUsers;
        }

        /// <summary>
        /// Maps user to view model which is safer. Note that separate address parts
        /// are mapped as single string with full address, but only if profile is accessible
        /// because administrators does not have profile.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static EditUserContextViewModel MapUserToViewModel(User user) {
            EditUserContextViewModel viewModel = new EditUserContextViewModel();
            if (user.Profile != null)
            {
                viewModel = new EditUserContextViewModel
                {
                    UserName = user.UserName,
                    FirstName = user.Profile.FirstName,
                    LastName = user.Profile.LastName,
                    Gender = user.Profile.Gender,
                    BirthDate = user.Profile.BirthDate.ToShortDateString(),
                    FullAddress = $"{user.Profile.Street} {user.Profile.StreetNumber} {user.Profile.City} {user.Profile.Region} {user.Profile.Country}",
                    BackupEmail = user.Profile.BackupEmailAddress,
                    IsLocked = user.IsLocked
                };
            }
            else {
                viewModel = new EditUserContextViewModel
                {
                    UserName = user.UserName,
                    IsLocked = user.IsLocked
                };
            }
            return viewModel;
        }

        /// <summary>
        /// Allows to change user name of an user with given old user name.
        /// Throws exception if database error occurs.
        /// </summary>
        /// <param name="currentUserName"></param>
        /// <param name="newUserName"></param>
        /// <param name="context"></param>
        /// <returns>False if new user name is not unique.</returns>
        public static async Task<bool> ChangeUserName(string currentUserName, string newUserName, AppDbContext context, UserManager<User> userManager) {
            IdentityResult updateResult;
            User user = context.Users.Where(u => u.UserName == currentUserName).Single();
            User userTmp = context.Users.Where(u => u.UserName == newUserName).SingleOrDefault();
            // New user name is unique.
            if (userTmp == null)
            {
                user.UserName = newUserName;
                user.Email = newUserName;
                updateResult = await userManager.UpdateAsync(user);
            }
            else {
                // Name not unique.
                return false;
            }

            if (!updateResult.Succeeded) {
                throw new Exception("Error occurred while changing user name in database.");
            }

            return true;
        }

        /// <summary>
        /// Changes user status to locked if its possible.
        /// </summary>
        /// <param name="currentUserName"></param>
        /// <param name="context"></param>
        /// <returns>True if action succeeded, false or exception if failed.</returns>
        public static async Task<bool> LockUser(string currentUserName, AppDbContext context) {
            User user = context.Users.Where(u => u.UserName == currentUserName).Single();
            if (user.IsLocked == false)
            {
                user.IsLocked = true;
            }
            else {
                return false;
            }
            var updateResult = await context.SaveChangesAsync();
            if (updateResult == 0) {
                throw new Exception("Error occurred while locking user in database.");
            }
            return true;
        }

        /// <summary>
        /// Changes user status to unlocked if its possible.
        /// </summary>
        /// <param name="currentUserName"></param>
        /// <param name="context"></param>
        /// <returns>True if action succeeded, false or exception if failed.</returns>
        public static async Task<bool> UnlockUser(string currentUserName, AppDbContext context)
        {
            User user = context.Users.Where(u => u.UserName == currentUserName).Single();
            if (user.IsLocked == true)
            {
                user.IsLocked = false;
            }
            else {
                return false;
            }
            var updateResult = await context.SaveChangesAsync();
            if (updateResult == 0)
            {
                throw new Exception("Error occurred while unlocking user in database.");
            }
            return true;
        }


    }
}
