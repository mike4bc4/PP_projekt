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
        /// Checks if caller is able to create announcement and redirects to announcement page.
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = Role.User)]
        public IActionResult EditAnnouncement(bool newModel = true)
        {
            dynamic model = new ExpandoObject();
            model.newModel = newModel;
            TempData["newModel"] = newModel;
            //return View("UnderConstruction");

            return View(model);
        }


        /// <summary>
        /// Returns specified partial view from hosting container.
        /// </summary>
        /// <param name="partialViewName"></param>
        /// <param name="viewModel"></param>
        /// <returns></returns>
		public IActionResult GetPartialViewWithData(string partialViewName, EditAnnouncementViewModel viewModel)
        {
            return PartialView("PartialViews/" + partialViewName, viewModel);
        }


        /// <summary>
        /// Validates announcement view model and puts it into database
        /// if it is correct and user is able to create announcement.
        /// Also changes user role to host if he had user role before.
        /// If view model is incorrect view data is updated with proper flag
        /// that results in rendering message.
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.User)]
        public async Task<IActionResult> SaveAnnouncement(EditAnnouncementViewModel viewModel)
        {
            User currentUser = await userManager.GetUserAsync(HttpContext.User);
            bool announcementCorrect = AnnouncementServiceLogic.IsAnnouncementViewModelValid(viewModel);
            if (announcementCorrect)
            {
                viewModel.IsCorrect = true;
                bool newModel = (bool)TempData["newModel"];
                await AnnouncementServiceLogic.SaveAnnouncementToDatabase(viewModel, context, currentUser, newModel);
                // Change user role to host if it's his first announcement.
                if (!currentUser.IsHost)
                {
                    await AnnouncementServiceLogic.MakeUserHost(currentUser, context);
                }
            }

            return Json(new { page = ControllerExtensions.ParseViewToStringAsync(this, viewModel, "PartialViews/SaveAnnouncement", true).Result, announcementCorrect });
        }


        [Authorize(Roles = Role.User)]
        public async Task<IActionResult> ListUserAnnouncements(string sortingMethod, string queryString)
        {

            User currentUser = await userManager.GetUserAsync(HttpContext.User);
            // Get only these announcements which were not removed by user.
            List<Announcement> usersAnnouncements = context.Announcements
                .Include(a => a.Address)
                .Where(a => a.User == currentUser)
                .Where(a => a.Removed == false)
                .ToList();

            List<EditAnnouncementViewModel> viewModel = AnnouncementServiceLogic.ParseAnnouncementsToViewModelList(usersAnnouncements, context);

            dynamic model = new ExpandoObject();
            model.announcements = viewModel;

            return View(model);
        }

        public IActionResult GetAnnouncementOwnerInfo(int announcementID)
        {
            Announcement announcement = context.Announcements.Include(a => a.User).Include(a => a.User.Profile).Where(a => a.ID == announcementID).SingleOrDefault();
            if (announcement == null)
            {
                return Json(null);
            }
            var userData = new
            {
                userName = announcement.User.UserName,
                firstName = announcement.User.Profile.FirstName,
                lastName = announcement.User.Profile.LastName
            };
            return Json(userData);
        }


        [Authorize(Roles = Role.User)]
        public async Task<IActionResult> ChangeAnnouncementsStatus(List<int> announcementsIDs, bool? areActive)
        {
            List<Announcement> announcements = (from a in context.Announcements
                                                where announcementsIDs.Contains(a.ID)
                                                select a).ToList();
            foreach (Announcement announcement in announcements)
            {
                if (areActive != null)
                    announcement.IsActive = (bool)areActive;
                else
                    announcement.Removed = true;
            }
            await context.SaveChangesAsync();
            return Json(true);
        }

        public IActionResult Browse(string annBrowserQuery)
        {
            dynamic model = new ExpandoObject();
            // Find only these announcements which fit in active time range,
            // were not deactivated or removed by owner and parse them to view model.
            List<EditAnnouncementViewModel> viewModel = AnnouncementServiceLogic
                .ParseAnnouncementsToViewModelList(SearchEngine.FindAnnoucements(annBrowserQuery, context), context)
                .Where(vm => (DateTime.Compare(vm.From, DateTime.Today) <= 0))
                .Where(vm => (DateTime.Compare(vm.To, DateTime.Today) >= 0))
                .Where(vm => vm.IsActive == true)
                .Where(vm => vm.Removed == false).ToList();

            model.announcements = viewModel;
            model.query = annBrowserQuery;

            return View(model);
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
                return null;
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
            return Json(new { reservationsPerUser });
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
                return Json(new { updated = (newReservationsAmount - reservations.Count()) });
            }
            else if (reservations.Count() < newReservationsAmount)
            {
                List<Reservation> reservationsToAdd = new List<Reservation>();
                for (int i = 0; i < newReservationsAmount - reservations.Count(); i++)
                {
                    reservationsToAdd.Add(new Reservation() { Announcement = announcement, Date = date, ScheduleItem = schItem, User = user });
                }
                context.Reservations.AddRange(reservationsToAdd);
                context.SaveChanges();
                return Json(new { updated = (reservations.Count() - newReservationsAmount) });
            }
            else
            {
                return Json(new { updated = 0 });
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
                }
                for (int i = 0; i < reservation.Reservations; i++)
                {
                    reservationsToAdd.Add(new Reservation()
                    {
                        Announcement = announcement,
                        Date = reservation.Date,
                        ScheduleItem = scheduleItem,
                        User = currentUser
                    });
                }
            }
            await context.AddRangeAsync(reservationsToAdd);
            int addedReservationRecords = await context.SaveChangesAsync();
            return Json(addedReservationRecords);
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
        /// and schedule item. Reservations are ordered by date in descending order (most recent first).
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
                    ScheduleItemID = grp.First().ScheduleItemID,
                    Amount = grp.Count()
                })
                .OrderByDescending(grp => grp.Date)
                .ToList();
            return Json(reservations);
        }

    }
}