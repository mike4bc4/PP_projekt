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
        /// Creates conversation based on received data. Announcement and schedule items
        /// are related to conversation but are not necessary. Returns ID number of created
        /// conversation or null if validation fails. Note that returned objects are JSON type.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="userNames"></param>
        /// <param name="dateStarted"></param>
        /// <param name="announcementID"></param>
        /// <param name="scheduleItemsIDs"></param>
        /// <param name="readOnly"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.User + "," + Role.Admin)]
        public async Task<IActionResult> CreateConversation(string title, List<string> userNames, DateTime dateStarted, int? announcementID, List<int> scheduleItemsIDs, bool readOnly = false)
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
            Announcement announcement = null;
            if (announcementID != null)
            {
                announcement = await context.Announcements.Where(a => a.ID == announcementID).SingleOrDefaultAsync();
                if (announcement == null)
                {
                    return Json(null);
                }
            }
            List<ScheduleItem> scheduleItems = new List<ScheduleItem>();
            if (scheduleItemsIDs.Count() != 0)
            {
                foreach (int scheduleItemID in scheduleItemsIDs)
                {
                    ScheduleItem sh = await context.ScheduleItems.Where(s => s.ScheduleItemID == scheduleItemID).SingleOrDefaultAsync();
                    if (sh == null)
                    {
                        return Json(null);
                    }
                    scheduleItems.Add(sh);
                }
            }

            // Adding new conversation and its relations.
            Conversation addedConversation = new Conversation()
            {
                DateStarted = dateStarted,
                ReadOnly = readOnly,
                Title = title,
                Announcement = announcement
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

            List<ScheduleItemToConversation> scheduleItemToConversations = new List<ScheduleItemToConversation>();
            foreach (ScheduleItem scheduleItem in scheduleItems)
            {
                scheduleItemToConversations.Add(new ScheduleItemToConversation()
                {
                    Conversation = addedConversation,
                    ScheduleItem = scheduleItem,
                });
            }
            await context.ScheduleItemToConversations.AddRangeAsync(scheduleItemToConversations);

            int result = await context.SaveChangesAsync();
            return Json(addedConversation.ConversationID);
        }

        /// <summary>
        /// Creates simple message with content, date send, sender and related conversation.
        /// Returns amount of modified (added) records in database. If validation fails
        /// returns null. Value of 0 is returned if message cannot be send because
        /// conversation is marked as read only. Note that returned objects are JSON type.
        /// </summary>
        /// <param name="conversationID"></param>
        /// <param name="content"></param>
        /// <param name="dateSend"></param>
        /// <param name="senderUserName"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.User + "," + Role.Admin)]
        public async Task<IActionResult> AddMessage(int? conversationID, string content, DateTime dateSend, string senderUserName)
        {
            // Validation
            if (conversationID == null ||
                string.IsNullOrEmpty(content) ||
                content.Count() > IoCContainer.DbSettings.Value.MaxConversationMessageLength ||
                dateSend == null ||
                dateSend.Date < DateTime.Today.AddDays(-1).Date ||
                dateSend.Date > DateTime.Today.AddDays(1).Date ||
                string.IsNullOrEmpty(senderUserName))
            {
                return Json(null);
            }
            Conversation conversation = await context.Conversations.Where(c => c.ConversationID == conversationID).SingleOrDefaultAsync();
            if (conversation == null)
            {
                return Json(null);
            }
            // Signalize with different output if message cannot be send.
            // Only administrator may send message through read only lock.
            if (conversation.ReadOnly == true && !HttpContext.User.IsInRole(Role.Admin)) {
                return Json(0);
            }

            User sender = await context.Users.Where(u => u.UserName == senderUserName).SingleOrDefaultAsync();
            if (sender == null)
            {
                return Json(null);
            }

            // Add messages
            Message message = new Message()
            {
                Content = content,
                Conversation = conversation,
                DateSend = dateSend,
                Sender = sender,
            };
            await context.Messages.AddAsync(message);

            int result = await context.SaveChangesAsync();
            return Json(result);
        }

        /// <summary>
        /// Gets current user and if present collects his conversations,
        /// parses it to suitable form and sends as returns as JSON object.
        /// If there is no such user returns JSON null. If amount of messages
        /// is equal to 0 returns JSON object with 0 value.
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = Role.User + "," + Role.Admin)]
        public async Task<IActionResult> GetConversations()
        {
            User currentUser = await userManager.GetUserAsync(HttpContext.User);
            if (currentUser == null)
            {
                return Json(null);
            }

            List<Conversation> currentUserConversations = context.UserToConversations
                .Include(utc => utc.Conversation)
                .Where(utc => utc.User == currentUser)
                .OrderByDescending(utc => utc.Conversation.DateStarted)
                .Select(utc => utc.Conversation).ToList();
            if (currentUserConversations.Count() == 0)
            {
                return Json(0);
            }
            List<ConversationViewModel> conversations = new List<ConversationViewModel>();
            foreach (Conversation conversation in currentUserConversations)
            {
                // Get schedule items from database.
                List<ScheduleItem> conversationScheduleItems = await context.ScheduleItemToConversations
                    .Include(s => s.Conversation)
                    .Include(s => s.ScheduleItem)
                    .Where(s => s.Conversation == conversation)
                    .Select(s => s.ScheduleItem)
                    .ToListAsync();

                // Parse schedule items to view model.
                List<ScheduleItemModel> scheduleItemModels = new List<ScheduleItemModel>();
                foreach (ScheduleItem scheduleItem in conversationScheduleItems)
                {
                    scheduleItemModels.Add(new ScheduleItemModel()
                    {
                        From = scheduleItem.From,
                        To = scheduleItem.To,
                        MaxReservations = scheduleItem.MaxReservations,
                    });
                }

                // Check if conversation is marked as hidden for current user.
                HiddenConversationToUser hiddenConversationToUser = await context.HiddenConversationToUsers
                    .Include(hc => hc.User)
                    .Include(hc => hc.Conversation)
                    .Where(hc => hc.User == currentUser)
                    .Where(hc => hc.Conversation == conversation)
                    .SingleOrDefaultAsync();


                // Add conversation to view model.
                conversations.Add(new ConversationViewModel()
                {
                    AnnouncementID = conversation.AnnouncementID,
                    ConversationID = conversation.ConversationID,
                    DateStarted = conversation.DateStarted,
                    ReadOnly = conversation.ReadOnly,
                    Title = conversation.Title,
                    ScheduleItems = scheduleItemModels,
                    IsHidden = (hiddenConversationToUser != null) ? true : false,
                });
            }
            return Json(conversations);
        }

        /// <summary>
        /// Allows to get all messages related to specified conversation.
        /// Returns list of messages or null if validation fails. List is ordered
        /// descending so latest messages are at the beginning. Note that method returns
        /// JSON objects.
        /// </summary>
        /// <param name="conversationID"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.User + "," + Role.Admin)]
        public async Task<IActionResult> GetMessages(int conversationID)
        {
            Conversation conversation = await context.Conversations
                .Where(c => c.ConversationID == conversationID).SingleOrDefaultAsync();
            if (conversation == null)
            {
                return Json(null);
            }
            List<Message> messages = await context.Messages
                .Include(m => m.Sender)
                .Where(m => m.Conversation == conversation)
                .OrderByDescending(m => m.DateSend)
                .ToListAsync();
            if (messages.Count == 0)
            {
                return Json(0);
            }
            List<MessageViewModel> messageViewModels = new List<MessageViewModel>();
            foreach (Message message in messages)
            {
                User sender = await context.Users
                    .Include(u => u.Profile)
                    .Where(u => u == message.Sender).SingleOrDefaultAsync();
                if (sender == null)
                {
                    return Json(null);
                }

                messageViewModels.Add(new MessageViewModel()
                {
                    Content = message.Content,
                    DateSend = message.DateSend,
                    SenderUserName = sender.UserName,
                    SenderFirstName = sender.Profile?.FirstName,
                    SenderLastName = sender.Profile?.LastName,
                });
            }
            return Json(messageViewModels);
        }

        /// <summary>
        /// Allows to create relation which indicates that conversation
        /// is hidden for specific user also allows to revert this action
        /// by removing mentioned relation. If validation of input data fails
        /// returns null else returns amount of rows affected in database.
        /// Note that method returns JSON objects.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="conversationID"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.User + "," + Role.Admin)]
        public async Task<IActionResult> HideRevertConversation(string userName, int? conversationID, bool hide = true) {
            User user = null;
            Conversation conversation = null;
            if (userName != null && conversationID != null) {
                user = await context.Users.Where(u => u.UserName == userName).SingleOrDefaultAsync();
                conversation = await context.Conversations.Where(c => c.ConversationID == conversationID).SingleOrDefaultAsync();
            }
            if (user == null || conversation == null) {
                return Json(null);
            }
            if (hide == true)
            {
                HiddenConversationToUser hiddenConversationToUser = new HiddenConversationToUser()
                {
                    Conversation = conversation,
                    User = user,
                };
                await context.HiddenConversationToUsers.AddAsync(hiddenConversationToUser);
            }
            else {
                HiddenConversationToUser hiddenConversationToUser = await context.HiddenConversationToUsers
                    .Include(hc => hc.User)
                    .Include(hc => hc.Conversation)
                    .Where(hc => hc.User == user)
                    .Where(hc => hc.Conversation == conversation)
                    .SingleOrDefaultAsync();
                if (hiddenConversationToUser == null) {
                    return Json(null);
                }
                context.HiddenConversationToUsers.Remove(hiddenConversationToUser);
            }
            var result = await context.SaveChangesAsync();
            return Json(result);
        }
    }
}
