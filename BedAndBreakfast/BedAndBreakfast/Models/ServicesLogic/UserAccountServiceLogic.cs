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

        public static List<ReceiveMsgSetting> CreateReceiveMsgSettings(User user, AppDbContext context) {
            if (user == null || context == null) {
                return null;
            }

            List<ReceiveMsgSetting> settings = new List<ReceiveMsgSetting>();
            foreach (MsgTypeDictionary entity in context.MsgTypeDictionaries)
            {
                ReceiveMsgSetting addedSetting = new ReceiveMsgSetting(PredefinedTablesContainer.DefaultReceiveMsgSetting)
                {
                    TypeFK = entity.Code,
                    MsgTypeDictionary = entity,
                    UserFK = user.Id,
                    User = user
                };
                settings.Add(addedSetting);
            }
            return settings;
        }

        public async static Task<bool> AddUserAndDependiencesToDB(User user, UserManager<User> userManager, CreateAccountViewModel viewModel, AppDbContext context) {

            List<ReceiveMsgSetting> addedSettings = CreateReceiveMsgSettings(user, context);
            if (addedSettings == null) {
                return false;
            }

            var createResult = await userManager.CreateAsync(user, viewModel.Password);
            var addToRoleResult = await userManager.AddToRoleAsync(user, Role.User);
            var addUserSettings = IdentityResult.Success;

            foreach (ReceiveMsgSetting setting in addedSettings)
            {
                await context.AddAsync(setting);
            }

            if (await context.SaveChangesAsync() != PredefinedTablesContainer.MsgTypeDictionaries.Count())
            {
                addUserSettings = IdentityResult.Failed();
            }

            return (createResult.Succeeded && addToRoleResult.Succeeded && addUserSettings.Succeeded);
        }


    }
}
