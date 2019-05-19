using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BedAndBreakfast.Data;
using BedAndBreakfast.Models;
using BedAndBreakfast.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BedAndBreakfast.Controllers
{
    public class MessageController : Controller
    {
        private AppDbContext context;
        private UserManager<User> userManager;

        public MessageController(AppDbContext context, UserManager<User> userManager)
        {
            this.context = context;
            this.userManager = userManager;
        }

        /// <summary>
        /// Redirects user to message box (my conversations).
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = Role.User + "," + Role.Admin)]
        public IActionResult ShowConversations()
        {
            return View();
        }

        /// <summary>
        /// After short validation which includes: title null, 0 and length verification,
        /// user names null and 0 verification, date started null and day before/after today verification
        /// action creates conversation and proper user-conversation relations returns null in case of error
        /// or number of changed rows in database.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="userNames"></param>
        /// <param name="dateStarted"></param>
        /// <param name="readOnly"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.User + "," + Role.Admin)]
        public async Task<IActionResult> CreateConversation(string title, List<string> userNames, DateTime dateStarted, bool readOnly = false)
        {
            // Validation 
            if (string.IsNullOrEmpty(title) ||
                userNames == null ||
                userNames.Count() == 0 ||
                dateStarted == null ||
                dateStarted.Date < DateTime.Today.AddDays(-1).Date ||
                dateStarted.Date > DateTime.Today.AddDays(1).Date ||
                title.Count() > IoCContainer.DbSettings.Value.MaxConversationTitleLength)
            {
                return Json(null);
            }
            List<User> conversationRelatedUsers = context.Users.Where(u => userNames.Contains(u.UserName)).ToList();
            if (conversationRelatedUsers == null ||
                conversationRelatedUsers.Count() == 0)
            {
                return Json(null);
            }
            // Adding new conversation and its relations.
            Conversation addedConversation = new Conversation()
            {
                DateStarted = dateStarted,
                IsHidden = false,
                ReadOnly = readOnly,
                Title = title,
            };
            await context.Conversations.AddAsync(addedConversation);
            List<UserToConversation> userToConversationRelations = new List<UserToConversation>();
            foreach (User user in conversationRelatedUsers)
            {
                userToConversationRelations.Add(new UserToConversation()
                {
                    Conversation = addedConversation,
                    User = user,
                });
            }
            await context.UserToConversations.AddRangeAsync(userToConversationRelations);
            int result = await context.SaveChangesAsync();
            return Json(result);
        }


        [Authorize(Roles = Role.User + "," + Role.Admin)]
        public async Task<IActionResult> AddMessage(int? conversationID, string content, DateTime dateSend)
        {
            // Validation
            if (conversationID == null ||
                string.IsNullOrEmpty(content) ||
                content.Count() > IoCContainer.DbSettings.Value.MaxConversationMessageLength ||
                dateSend == null ||
                dateSend.Date < DateTime.Today.AddDays(-1).Date ||
                dateSend.Date > DateTime.Today.AddDays(1).Date)
            {
                return Json(null);
            }
            Conversation conversation = context.Conversations.Where(c => c.ConversationID == conversationID).SingleOrDefault();
            User currentUser = await userManager.GetUserAsync(HttpContext.User);
            if (conversation == null || currentUser == null || conversation.ReadOnly == true)
            {
                return Json(null);
            }
            // Adding message to conversation.
            Message addedMessage = new Message()
            {
                Content = content,
                Conversation = conversation,
                DateSend = dateSend,
                Sender = currentUser,
            };
            await context.Messages.AddAsync(addedMessage);
            int result = await context.SaveChangesAsync();
            return Json(result);
        }

        [Authorize(Roles = Role.User + "," + Role.Admin)]
        public async Task<IActionResult> GetConversations()
        {
            User currentUser = await userManager.GetUserAsync(HttpContext.User);
            if (currentUser == null)
            {
                return Json(null);
            }

            List<Conversation> currentUserConversations = context.UserToConversations
                .Include(utc => utc.Conversation.Messages)
                .Where(utc => utc.User == currentUser)
                .Select(utc => utc.Conversation).ToList();
            if (currentUserConversations.Count() == 0)
            {
                return Json(0);
            }
            List<ConversationViewModel> conversations = new List<ConversationViewModel>();
            foreach (Conversation conversation in currentUserConversations)
            {
                conversations.Add(new ConversationViewModel()
                {
                    DateStarted = conversation.DateStarted,
                    IsHidden = conversation.IsHidden,
                    MessagesAmount = conversation.Messages.Count(),
                    ReadOnly = conversation.ReadOnly,
                    Title = conversation.Title,
                });
            }
            return Json(conversations);
        }
    }
}
