using BedAndBreakfast.Data;
using BedAndBreakfast.Settings;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BedAndBreakfast.Models.ServicesLogic
{
    /// <summary>
    /// Provides methods for user account services handling e.g. in user account 
    /// controller.
    /// </summary>
    public static class UserAccountServiceLogic
    {
        public static Profile CreateProfile(CreateAccountViewModel viewModel) {
            if (viewModel == null) {
                return null;
            }

            var profile = new Profile
            {
                FirstName = viewModel.FirstName,
                LastName = viewModel.LastName,
                BirthDate = viewModel.BirthDate
            };
            return profile;
        }

        public static User CreateUser(CreateAccountViewModel viewModel, Profile profile) {
            if (viewModel == null) {
                return null;
            }

            var user = new User
            {
                UserName = viewModel.EmailAddress,
                Email = viewModel.EmailAddress,
                Profile = profile
            };
            return user;
        }


        public async static Task<bool> AddUserAndDependenciesToDB(User user, UserManager<User> userManager, CreateAccountViewModel viewModel, AppDbContext context) {

            user.PrivacySetting = new PrivacySetting
            {
                User = user,
                UserFK = user.Id
            };
            user.NotificationsSetting = new NotificationsSetting
            {
                User = user,
                UserFK = user.Id
            };

            var createResult = await userManager.CreateAsync(user, viewModel.Password);
            var addToRoleResult = await userManager.AddToRoleAsync(user, Role.User);
            var addMsgSettingsResult = IdentityResult.Success;


            return (createResult.Succeeded && addToRoleResult.Succeeded && addMsgSettingsResult.Succeeded);
        }

        public static bool IsAccountLocked(AppDbContext context, LogInViewModel viewModel) {
            User user = (from u in context.Users
                         where u.UserName == viewModel.Login
                         select u).FirstOrDefault();
            if (user == null) {
                return false;
            }
            return user.IsLocked;
        }

        public static void CopyNotificationSettings(NotificationsSetting origin, EditNotificationsViewModel setting)
        {
            origin.GeneralByEmail = setting.GeneralByEmail;
            origin.GeneralByMobileApp = setting.GeneralByMobileApp;
            origin.GeneralBySMS = setting.GeneralBySMS;
            origin.GeneralByPhone = setting.GeneralByPhone;

            origin.DiscountAnTipsByEmail = setting.DiscountAnTipsByEmail;
            origin.DiscountAnTipsMobileApp = setting.DiscountAnTipsMobileApp;
            origin.DiscountAnTipsBySMS = setting.DiscountAnTipsBySMS;
            origin.DiscountAnTipsByPhone = setting.DiscountAnTipsByPhone;

            origin.RemindByEmail = setting.RemindByEmail;
            origin.RemindByMobileApp = setting.RemindByMobileApp;
            origin.RemindBySMS = setting.RemindBySMS;
            origin.RemindByPhone = setting.RemindByPhone;

            origin.RulesAndCommunityByEmail = setting.RulesAndCommunityByEmail;
            origin.RulesAndCommunityByMobileApp = setting.RulesAndCommunityByMobileApp;
            origin.RulesAndCommunityBySMS = setting.RulesAndCommunityBySMS;
            origin.RulesAndCommunityByPhone = setting.RulesAndCommunityByPhone;

            origin.ServiceByEmail = setting.ServiceByEmail;
            origin.ServiceByMobileApp = setting.ServiceByMobileApp;
            origin.ServiceBySMS = setting.ServiceBySMS;
            origin.ServiceByPhone = setting.ServiceByPhone;
        }

        

    }
}
