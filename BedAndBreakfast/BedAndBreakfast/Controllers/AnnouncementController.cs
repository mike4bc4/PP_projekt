using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BedAndBreakfast.Data;
using BedAndBreakfast.Models;
using BedAndBreakfast.Models.ServicesLogic;
using BedAndBreakfast.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace BedAndBreakfast.Controllers
{
    public class AnnouncementController : Controller
    {
        /// <summary>
        /// Service necessary to validate policies. 
        /// </summary>
        private IAuthorizationService authorizationService;
        private AppDbContext context;
        private UserManager<User> userManager;


        public AnnouncementController(IAuthorizationService authorizationService, AppDbContext context, UserManager<User> userManager)
        {
            this.authorizationService = authorizationService;
            this.context = context;
            this.userManager = userManager;
        }


        /// <summary>
        /// Allows to retrieve list of numbers that represent amount of reservations per day or per schedule item.
        /// If announcement uses "per day" timetable action returns list of 7 items where 3rd is amount of
        /// reservations for specified date. If announcement uses "per hour/scheduled timetable" then list represents
        /// amount of reservations per schedule item (ordered ascending by schedule item begin time). Note that list 
        /// always has size of schedule items per day or seven (indexes where is no reservations are null).
        /// </summary>
        /// <param name="announcementID"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public IActionResult GetReservations(int announcementID, DateTime date)
        {
            List<int?> reservations = new List<int?>();
            List<ScheduleItemViewModel> scheduleItemsViewModel = new List<ScheduleItemViewModel>();
            Announcement announcement = context.Announcements.Where(a => a.ID == announcementID).SingleOrDefault();
            if (announcement == null)
            {
                return Json(null);
            }
            List<ScheduleItem> scheduleItems = context.AnnouncementToSchedules
                .Where(s => s.Announcement == announcement)
                .Select(s => s.ScheduleItem)
                .OrderBy(s => s.From)
                .ToList();
            foreach (ScheduleItem scheduleItem in scheduleItems)
            {
                scheduleItemsViewModel.Add(new ScheduleItemViewModel
                {
                    From = scheduleItem.From,
                    To = scheduleItem.To,
                    MaxReservations = scheduleItem.MaxReservations
                });
            }
            reservations = AnnouncementServiceLogic.GetReservations(announcement, date, context);

            return Json(new
            {
                reservations,
                announcement = new
                {
                    id = announcement.ID,
                    from = announcement.From,
                    to = announcement.To,
                    timetable = announcement.Timetable,
                    maxReservations = announcement.MaxReservations
                },
                scheduleItems = scheduleItemsViewModel
            });
        }

        /// <summary>
        /// Returns list of users and reservations per user for specified day or schedule item.
        /// Will return null if there is no announcement or schedule item with specified id/unique data.
        /// </summary>
        /// <param name="announcementID"></param>
        /// <param name="date"></param>
        /// <param name="scheduleItem"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.User)]
        public IActionResult GetUsersReservations(int announcementID, DateTime date, ScheduleItemViewModel scheduleItem)
        {
            Announcement announcement = context.Announcements.Where(a => a.ID == announcementID).SingleOrDefault();
            if (announcement == null)
            {
                return Json(null);
            }
            IQueryable<Tuple<User, int>> data = null;
            if (announcement.Timetable == 1)
            {
                data = context.Reservations
                    .Include(r => r.User)
                    .Include(r => r.User.Profile)
                    .Where(r => r.Announcement == announcement)
                    .Where(r => r.Date.Date == date.Date)
                    .GroupBy(r => r.User)
                    .Select(grp => new Tuple<User, int>(grp.Key, grp.Count()));
            }
            else if (announcement.Timetable == 2)
            {
                ScheduleItem schItem = context.ScheduleItems
                       .Where(s => s.From == scheduleItem.From)
                       .Where(s => s.To == scheduleItem.To)
                       .Where(s => s.MaxReservations == scheduleItem.MaxReservations)
                       .SingleOrDefault();
                if (schItem == null)
                {
                    return Json(null);
                }
                data = context.Reservations
                    .Include(r => r.User)
                    .Include(r => r.User.Profile)
                    .Where(r => r.Announcement == announcement)
                    .Where(r => r.Date.Date == date.Date)
                    .Where(r => r.ScheduleItem == schItem)
                    .GroupBy(r => r.User)
                    .Select(grp => new Tuple<User, int>(grp.Key, grp.Count()));

            }
            List<object> reservationsPerUser = new List<object>();
            foreach (var item in data)
            {
                var userData = new
                {
                    userName = item.Item1.UserName,
                    firstName = item.Item1.Profile.FirstName,
                    lastName = item.Item1.Profile.LastName
                };
                reservationsPerUser.Add(new { userData = userData, reservations = item.Item2 });
            }
            return Json(reservationsPerUser);
        }

        /// <summary>
        /// Updated amount of reservations for specified announcement, user and schedule item (not obligatory).
        /// If provided reservation count is higher than current number of such relations in database then 
        /// reservations are added - else removed. Return number of added (removed) reservations or null
        /// if error occurs.
        /// </summary>
        /// <param name="announcementID"></param>
        /// <param name="userName"></param>
        /// <param name="date"></param>
        /// <param name="newReservationsAmount"></param>
        /// <param name="scheduleItem"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.User)]
        public IActionResult UpdateReservations(int announcementID, string userName, DateTime date, int newReservationsAmount, ScheduleItemViewModel scheduleItem)
        {
            Announcement announcement = context.Announcements.Where(a => a.ID == announcementID).SingleOrDefault();
            User user = context.Users.Where(u => u.UserName == userName).SingleOrDefault();
            // Amount validation - amount of new reservations cannot be lower than 0.
            if (announcement == null || user == null || newReservationsAmount < 0)
            {
                return Json(null);
            }
            List<Reservation> reservations = new List<Reservation>();
            ScheduleItem schItem = null;
            if (announcement.Timetable == 1)        // Per day reservations
            {
                reservations = context.Reservations
                    .Where(r => r.Announcement == announcement)
                    .Where(r => r.User == user)
                    .Where(r => r.Date.Date == date.Date).ToList();
            }
            else if (announcement.Timetable == 2)   // Per hour reservations
            {
                schItem = context.ScheduleItems
                    .Where(s => s.From == scheduleItem.From)
                    .Where(s => s.To == scheduleItem.To)
                    .Where(s => s.MaxReservations == scheduleItem.MaxReservations).SingleOrDefault();
                if (schItem == null)
                {
                    return Json(null);
                }
                reservations = context.Reservations
                    .Where(r => r.Announcement == announcement)
                    .Where(r => r.User == user)
                    .Where(r => r.Date.Date == date.Date)
                    .Where(r => r.ScheduleItem == schItem).ToList();
            }
            if (reservations.Count() > newReservationsAmount)
            {
                List<Reservation> reservationsToRemove = reservations.Take(reservations.Count() - newReservationsAmount).ToList();
                context.Reservations.RemoveRange(reservationsToRemove);
                context.SaveChanges();
                return Json(newReservationsAmount - reservations.Count());
            }
            else if (reservations.Count() < newReservationsAmount)
            {
                List<Reservation> reservationsToAdd = new List<Reservation>();
                for (int i = 0; i < newReservationsAmount - reservations.Count(); i++)
                {
                    reservationsToAdd.Add(new Reservation()
                    {
                        Announcement = announcement,
                        Date = date,
                        ScheduleItem = schItem,
                        User = user,
                        ReservationDate = DateTime.Today,
                    });
                }
                context.Reservations.AddRange(reservationsToAdd);
                context.SaveChanges();
                return Json(reservations.Count() - newReservationsAmount);
            }
            else
            {
                return Json(0);
            }
        }

        /// <summary>
        /// Performs simple validation and inserts specified amount of reservation records into db.
        /// Returns number of modified rows (added reservations) or null if announcement related
        /// to reservation or user who is making reservation is not found.
        /// </summary>
        /// <param name="reservations"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.User)]
        public async Task<IActionResult> MakeReservations(List<ReservationViewModel> reservations)
        {
            List<Reservation> reservationsToAdd = new List<Reservation>();
            Announcement announcement = await context.Announcements.Where(a => a.ID == reservations[0].AnnouncementID).SingleOrDefaultAsync();
            User currentUser = await userManager.GetUserAsync(HttpContext.User);

            if (reservations == null || reservations.Count() == 0 || announcement == null || currentUser == null)
            {
                return Json(null);
            }

            List<int> scheduleItemsIDs = new List<int>();
            foreach (ReservationViewModel reservation in reservations)
            {
                ScheduleItem scheduleItem = null;
                if (reservation.From != null || reservation.To != null || reservation.MaxReservations != null)
                {
                    scheduleItem = context.ScheduleItems
                        .Where(s => s.From == reservation.From)
                        .Where(s => s.To == reservation.To)
                        .Where(s => s.MaxReservations == reservation.MaxReservations)
                        .SingleOrDefault();
                    if (scheduleItem == null)
                    {
                        return Json(null);
                    }
                    scheduleItemsIDs.Add(scheduleItem.ScheduleItemID);
                }
                for (int i = 0; i < reservation.Reservations; i++)
                {
                    reservationsToAdd.Add(new Reservation()
                    {
                        Announcement = announcement,
                        Date = reservation.Date,
                        ScheduleItem = scheduleItem,
                        User = currentUser,
                        ReservationDate = DateTime.Now
                    });
                }
            }
            if (scheduleItemsIDs.Count() == 0)
            {
                scheduleItemsIDs = null;
            }
            else
            {
                scheduleItemsIDs = scheduleItemsIDs.Distinct().ToList();
            }
            await context.AddRangeAsync(reservationsToAdd);
            int addedReservationRecords = await context.SaveChangesAsync();
            return Json(new { announcementID = announcement.ID, scheduleItemsIDs = scheduleItemsIDs, additions = addedReservationRecords });
        }

        /// <summary>
        /// Allows to save review related to specified announcement. Before saving review is validated.
        /// Rating and content are obligatory but user can provide no name or name shorter than amount specified
        /// in database settings. 
        /// </summary>
        /// <param name="announcementID"></param>
        /// <param name="reviewViewModel"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.User)]
        public async Task<IActionResult> PostReview(int announcementID, ReviewViewModel reviewViewModel)
        {
            Announcement announcement = await context.Announcements.Where(a => a.ID == announcementID).SingleOrDefaultAsync();
            User currentUser = await userManager.GetUserAsync(HttpContext.User);
            if (announcement == null || currentUser == null)
            {
                return Json(null);
            }
            // Validate review
            if (string.IsNullOrEmpty(reviewViewModel.Name) ||
                reviewViewModel.Name.Count() > IoCContainer.DbSettings.Value.MaxReviewNameLength ||
                reviewViewModel.ReviewDate == null ||
                string.IsNullOrEmpty(reviewViewModel.Content) ||
                reviewViewModel.Content.Count() > IoCContainer.DbSettings.Value.MaxReviewContentLength)
            {
                return Json(null);
            }
            if (reviewViewModel.Rating < 0 ||
                reviewViewModel.Rating > 10)
            {
                return Json(null);
            }
            // Save review
            Review review = new Review()
            {
                Announcement = announcement,
                User = currentUser,
                Name = reviewViewModel.Name,
                Rating = (byte)reviewViewModel.Rating,
                Content = reviewViewModel.Content,
                ReviewDate = reviewViewModel.ReviewDate
            };
            await context.Reviews.AddAsync(review);
            var result = await context.SaveChangesAsync();
            if (result == 0)
            {
                return Json(null);
            }
            else
            {
                return Json(result);
            }
        }

        /// <summary>
        /// Finds reviews related to announcement specified by id number.
        /// If there is no such announcement null is returned, otherwise this action
        /// returns list of reviews (parsed to view model) ordered descending by review date.
        /// </summary>
        /// <param name="announcementID"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.User)]
        public async Task<IActionResult> GetReviews(int announcementID)
        {
            Announcement announcement = await context.Announcements.Where(a => a.ID == announcementID).SingleOrDefaultAsync();
            if (announcement == null)
            {
                return Json(null);
            }
            List<Review> announcementReviews = await context.Reviews.Where(ar => ar.Announcement == announcement).OrderByDescending(ar => ar.ReviewDate).ToListAsync();
            List<ReviewViewModel> reviewViewModels = new List<ReviewViewModel>();
            foreach (Review review in announcementReviews)
            {
                reviewViewModels.Add(new ReviewViewModel
                {
                    Name = review.Name,
                    Rating = review.Rating,
                    Content = review.Content,
                    ReviewDate = review.ReviewDate
                });
            }
            return Json(new { reviews = reviewViewModels });
        }

        /// <summary>
        /// Redirects user to his/her reservations page.
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = Role.User)]
        public IActionResult ShowReservations()
        {
            return View();
        }

        /// <summary>
        /// Finds ad returns all reservations related to user grouped by announcement, date
        /// and schedule item. Reservations are ordered by reservation date in descending order (most recent first).
        /// If for some reason user cannot be found null is returned.
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = Role.User)]
        [ActionName("UserReservations")]
        public async Task<IActionResult> GetReservations()
        {
            User user = await userManager.GetUserAsync(HttpContext.User);
            if (user == null)
            {
                return Json(null);
            }
            List<ShowReservationsViewModel> reservations = context.Reservations
                .Include(r => r.Announcement)
                .Include(r => r.Announcement.Address)
                .Include(r => r.ScheduleItem)
                .Where(r => r.User == user)
                .GroupBy(r => new { r.Announcement, r.Date, r.ScheduleItem })
                .Select(grp => new ShowReservationsViewModel(grp.First().ScheduleItem)
                {
                    AnnouncementID = grp.First().AnnouncementID,
                    AnnouncementType = grp.First().Announcement.Type,
                    AnnouncementSubtype = grp.First().Announcement.Subtype,
                    HouseSharedPart = grp.First().Announcement.SharedPart,
                    Country = grp.First().Announcement.Address.Country,
                    Region = grp.First().Announcement.Address.Region,
                    City = grp.First().Announcement.Address.City,
                    Street = grp.First().Announcement.Address.Street,
                    StreetNumber = grp.First().Announcement.Address.StreetNumber,
                    Date = grp.First().Date,
                    ReservationDate = grp.First().ReservationDate,
                    ScheduleItemID = grp.First().ScheduleItemID,
                    Amount = grp.Count()
                })
                .OrderByDescending(grp => grp.ReservationDate)
                .ToList();
            return Json(reservations);
        }

        /// <summary>
        /// Returns user name of owner of announcement specified by given ID. If for some reason
        /// user cannot be found null is returned. This function returns Json object.
        /// </summary>
        /// <param name="announcementID"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.User + "," + Role.Admin)]
        public async Task<IActionResult> GetAnnouncementOwnerUserName(int announcementID)
        {
            User announcementOwner = await context.Announcements.Where(a => a.ID == announcementID).Select(a => a.User).SingleOrDefaultAsync();
            if (announcementOwner == null)
            {
                return Json(null);
            }
            return Json(announcementOwner.UserName);
        }

        /// <summary>
        /// Allows to recover schedule item from database with specified
        /// ID number. If validation fails or there is no such schedule item
        /// null is returned. Note that method returns JSON objects.
        /// </summary>
        /// <param name="scheduleItemID"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.User + "," + Role.Admin)]
        public async Task<IActionResult> GetScheduleItem(int? scheduleItemID)
        {
            if (scheduleItemID == null)
            {
                return Json(null);
            }
            ScheduleItem scheduleItem = await context.ScheduleItems
                .Where(si => si.ScheduleItemID == scheduleItemID)
                .SingleOrDefaultAsync();
            if (scheduleItem == null)
            {
                return Json(null);
            }
            ScheduleItemViewModel scheduleItemViewModel = new ScheduleItemViewModel()
            {
                From = scheduleItem.From,
                MaxReservations = scheduleItem.From,
                To = scheduleItem.To,
            };
            return Json(scheduleItemViewModel);
        }

        /// <summary>
        /// Redirects to announcement manager view.
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = Role.User)]
        public IActionResult ManageAnnouncements()
        {
            return View();
        }

        [Authorize(Roles = Role.User)]
        public async Task<IActionResult> GetCurrentUserAnnouncements()
        {
            User currentUser = await userManager.GetUserAsync(HttpContext.User);
            if (currentUser == null)
            {
                return Json(null);
            }
            List<Announcement> usersAnnouncements = await context.Announcements
                .Include(a => a.Address)
                .Where(a => a.Removed == false)
                .Where(a => a.User == currentUser).ToListAsync();
            if (usersAnnouncements.Count() == 0)
            {
                return Json(0);
            }
            List<AnnouncementViewModel> announcementViewModels = new List<AnnouncementViewModel>();
            foreach (Announcement announcement in usersAnnouncements)
            {
                if (announcement.From.Date > DateTime.Today.Date || announcement.To.Date < DateTime.Today.Date)
                {
                    announcement.IsActive = false;
                }
                announcementViewModels.Add(new AnnouncementViewModel()
                {
                    AnnouncementID = announcement.ID,
                    City = announcement.Address.City,
                    Country = announcement.Address.Country,
                    From = announcement.From,
                    IsActive = announcement.IsActive,
                    IsRemoved = announcement.Removed,
                    Region = announcement.Address.Region,
                    SharedPart = announcement.SharedPart,
                    Street = announcement.Address.Street,
                    StreetNumber = announcement.Address.StreetNumber,
                    Subtype = announcement.Subtype,
                    To = announcement.To,
                    Type = announcement.Type,
                    Timetable = announcement.Timetable,
                });
            }
            await context.SaveChangesAsync();
            return Json(announcementViewModels);
        }

        [Authorize(Roles = Role.User)]
        public async Task<IActionResult> RemoveAnnouncements(List<int> announcementIDs)
        {
            if (announcementIDs.Count() == 0)
            {
                return Json(0);
            }
            List<Announcement> announcements = await context.Announcements.Where(a => announcementIDs.Contains(a.ID)).ToListAsync();
            foreach (Announcement announcement in announcements)
            {
                announcement.Removed = true;
                announcement.IsActive = false;
            }
            int result = await context.SaveChangesAsync();
            return Json(result);
        }

        [Authorize(Roles = Role.User)]
        public async Task<IActionResult> ChangeAnnouncementsStatus(List<int> announcementIDs)
        {
            bool error = false;
            if (announcementIDs.Count() == 0)
            {
                return Json(new { result = 0 });
            }
            List<Announcement> announcements = await context.Announcements.Where(a => announcementIDs.Contains(a.ID)).ToListAsync();
            foreach (Announcement announcement in announcements)
            {
                // Activate only these announcements that may be activated.
                if (announcement.IsActive == true || (announcement.From.Date < DateTime.Today.Date && announcement.To.Date > DateTime.Today.Date))
                {
                    announcement.IsActive = !announcement.IsActive;
                }
                else
                {
                    error = true;
                }
            }
            int result = await context.SaveChangesAsync();
            return Json(new { result, error });
        }

        [Authorize(Roles = Role.User)]
        public IActionResult EditAnnouncement()
        {
            return PartialView("EditAnnouncementPartialView");
        }

        [Authorize(Roles = Role.User)]
        public async Task<IActionResult> SaveAnnouncement(List<IFormFile> images, string announcement)
        {
            // Announcement is passed inside FormData object and comes as serialized JSON object and has to be deserialized manually.
            SaveAnnouncementViewModel announcementViewModel = JsonConvert
                .DeserializeObject<SaveAnnouncementViewModel>(announcement);

            return Json(null);
        }

        /// <summary>
        /// Allows to load proper partial view into announcement manager.
        /// Loaded view depends on timetable option.
        /// </summary>
        /// <param name="timetableOption"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.User)]
        public IActionResult LoadTimetable(int timetableOption)
        {
            switch (timetableOption)
            {
                case 1:
                    return PartialView("DailyTimetablePartialView");
                case 2:
                    return PartialView("HourlyTimetablePartialView");
                default:
                    return Json(null);
            }
        }

        /// <summary>
        /// Allows to acquire announcement specified by given announcement id number.
        /// If there is no such announcement null is returned, otherwise save announcement
        /// view model object is returned. Note that action returns JSON objects.
        /// </summary>
        /// <param name="announcementID"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.User)]
        public async Task<IActionResult> GetAnnouncement(int announcementID)
        {
            // Get specified announcement.
            Announcement announcement = await context.Announcements
                .Include(a => a.Address)
                .Where(a => a.ID == announcementID).SingleOrDefaultAsync();
            // Validate
            if (announcement == null)
            {
                return Json(null);
            }
            // Get announcement related contacts.
            List<AdditionalContact> additionalContacts = await context.AnnouncementToContacts
                .Include(ac => ac.AdditionalContact)
                .Where(ac => ac.AnnouncementID == announcementID)
                .Select(ac => ac.AdditionalContact)
                .ToListAsync();
            // Get announcement related payment methods.
            List<PaymentMethod> paymentMethods = await context.AnnouncementToPayments
                .Include(ap => ap.PaymentMethod)
                .Where(ap => ap.AnnouncementID == announcementID)
                .Select(ac => ac.PaymentMethod)
                .ToListAsync();
            // Get announcement related schedule items.
            List<ScheduleItem> scheduleItemsData = await context.AnnouncementToSchedules
                .Include(ats => ats.ScheduleItem)
                .Where(ats => ats.AnnouncementID == announcementID)
                .Select(ats => ats.ScheduleItem)
                .OrderBy(si => si.From)
                .ToListAsync();
            // Parse acquired contacts to view model.
            List<ContactPaymentItem> contacts = new List<ContactPaymentItem>();
            foreach (AdditionalContact additionalContact in additionalContacts)
            {
                contacts.Add(new ContactPaymentItem()
                {
                    Type = additionalContact.Type,
                    Value = additionalContact.Data,
                });
            }
            // Parse acquired payments to view model.
            List<ContactPaymentItem> payments = new List<ContactPaymentItem>();
            foreach (PaymentMethod paymentMethod in paymentMethods)
            {
                payments.Add(new ContactPaymentItem()
                {
                    Type = paymentMethod.Type,
                    Value = paymentMethod.Data,
                });
            }
            // Parse acquired schedule items to view model.
            List<ScheduleItemViewModel> scheduleItems = new List<ScheduleItemViewModel>();
            foreach (ScheduleItem scheduleItem in scheduleItemsData)
            {
                scheduleItems.Add(new ScheduleItemViewModel()
                {
                    From = scheduleItem.From,
                    To = scheduleItem.To,
                    MaxReservations = scheduleItem.MaxReservations,
                });
            }
            // Collect data into single object and send to view.
            SaveAnnouncementViewModel outputAnnouncement = new SaveAnnouncementViewModel()
            {
                AnnouncementID = announcement.ID,
                City = announcement.Address.City,
                Contacts = contacts,
                Country = announcement.Address.Country,
                Description = announcement.Description,
                From = announcement.From,
                Payments = payments,
                PerDayReservations = announcement.MaxReservations,
                Region = announcement.Address.Region,
                ScheduleItems = scheduleItems,
                SharedPart = announcement.SharedPart,
                Street = announcement.Address.Street,
                StreetNumber = announcement.Address.StreetNumber,
                Subtype = announcement.Subtype,
                Timetable = announcement.Timetable,
                To = announcement.To,
                Type = announcement.Type,
            };

            return Json(outputAnnouncement);
        }
    }
}