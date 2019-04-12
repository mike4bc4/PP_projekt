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
        public static List<FindUserViewModel> MapUserToViewModel(List<User> users) {
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
    }
}
