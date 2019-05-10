using BedAndBreakfast.Data;
using BedAndBreakfast.Settings;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BedAndBreakfast.Models.ServicesLogic
{
    public static class AnnouncementServiceLogic
    {

        public static bool IsAnnouncementViewModelValid(EditAnnouncementViewModel viewModel)
        {
            // Validate received view model.
            if (viewModel.Type == null)
            {
                return false;
            }
            // Continue validation only if current model state is valid.
            // Check subtype.
            if (viewModel.Type == (byte)EnumeratedDbValues.AnnouncementType.House)
            {
                if (viewModel.Subtype == null || viewModel.SharedPart == null)
                {
                    return false;
                }
            }
            else
            {
                if (viewModel.Subtype == null)
                {
                    return false;
                }
            }
            // Check address.
            if (viewModel.Country == null || viewModel.Region == null || viewModel.City == null || viewModel.Street == null || viewModel.StreetNumber == null)
            {
                return false;
            }
            int len = IoCContainer.DbSettings.Value.MaxAddressInputLength;
            if (viewModel.Country.Length > len || viewModel.Region.Length > len || viewModel.City.Length > len
                || viewModel.Street.Length > len || viewModel.StreetNumber.Length > len)
            {
                return false;
            }
            // Check time.
            if (DateTime.Compare(viewModel.From, viewModel.To) > 0 || DateTime.Compare(viewModel.From, DateTime.Today) < 0)
            {
                return false;
            }
            // Check description.
            if (viewModel.Description == null)
            {
                return false;
            }
            // Check contacts
            if (viewModel.ContactMethods.Count() > 0)
            {
                foreach (var item in viewModel.ContactMethods)
                {
                    if (string.IsNullOrEmpty(item.Key))
                    {
                        return false;
                    }
                }
            }
            else
            {
                return false;
            }
            // Check payment methods.
            if (viewModel.PaymentMethods.Count() > 0)
            {
                foreach (var item in viewModel.PaymentMethods)
                {
                    if (string.IsNullOrEmpty(item.Key))
                    {
                        return false;
                    }
                }
            }
            else
            {
                return false;
            }
            // Check timetable options
            switch (viewModel.Timetable)
            {
                case 1:
                    if (viewModel.MaxReservations == null || viewModel.MaxReservations == 0)
                    {
                        return false;
                    }
                    break;
                case 2:
                    // If list does not exist or is empty report validation error.
                    if (viewModel.ScheduleItems == null || viewModel.ScheduleItems.Count() == 0)
                    {
                        return false;
                    }
                    bool scheduleItemsNotEmpty = false;
                    foreach (var item in viewModel.ScheduleItems)
                    {
                        // At the same time check if list is not full of null values.
                        if (item != null)
                        {
                            scheduleItemsNotEmpty = true;
                        }
                        // For each item in schedule items.
                        /// Represents set of time ranges: 0:00-1:00, 1:00-2:00...
                        List<bool> availableHours = new List<bool> {
                            true,true,true,true,true,true,true,true,
                            true,true,true,true,true,true,true,true,
                            true,true,true,true,true,true,true,true
                        };
                        /// Represents set of time ranges: 0:00-1:00, 1:00-2:00...
                        List<bool> currentItemAvailableHours = new List<bool> {
                            true,true,true,true,true,true,true,true,
                            true,true,true,true,true,true,true,true,
                            true,true,true,true,true,true,true,true
                        };
                        for (var i = 0; i < viewModel.ScheduleItems.Count(); i++)
                        {
                            if (viewModel.ScheduleItems[i] != null && viewModel.ScheduleItems[i] != item)
                            {
                                // Mark hours as unavailable for every other schedule item.
                                for (var j = viewModel.ScheduleItems[i].From; j < viewModel.ScheduleItems[i].To; j++)
                                {
                                    availableHours[j] = false;
                                }
                            }
                        }
                        // Mark hours as unavailable fro currently validated item.
                        for (var i = item.From; i < item.To; i++)
                        {
                            currentItemAvailableHours[i] = false;
                        }
                        for (var i = 0; i < availableHours.Count(); i++)
                        {
                            // If any unavailable hour for all other items overlaps with currently validated item available hours report validation error.
                            if (availableHours[i] == false && currentItemAvailableHours[i] == false)
                            {
                                return false;
                            }
                        }
                        // If no time range error detected check if amount of max reservations is correct (not null or 0).
                        if (item.MaxReservations == null || item.MaxReservations == 0)
                        {
                            return false;
                        }
                    }
                    // If list is full of null values report validation error.
                    if (!scheduleItemsNotEmpty)
                    {
                        return false;
                    }
                    break;
            }
            // Everything is correct.
            return true;
        }

        /// <summary>
        /// Saves announcement to database. If such announcement is already present
        /// in database it will be updated. Also generates browser tags for announcement. 
        /// </summary>
        /// <param name="viewModel"></param>
        /// <param name="context"></param>
        /// <param name="announcementOwner"></param>
        /// <param name="newModel"></param>
        /// <returns></returns>
        public static async Task SaveAnnouncementToDatabase(EditAnnouncementViewModel viewModel, AppDbContext context, User announcementOwner, bool newModel)
        {
            Announcement announcement;
            if (newModel)
                announcement = new Announcement();
            else
            {
                // If model is edited it has to be found in database by provided ID.
                announcement = context.Announcements.Include(a => a.Address).Where(a => a.ID == viewModel.ID).Single();
                // All payment methods and contact relations are removed to avoid duplicate relations and
                // saving back those which were removed by user.
                ClearContactsAndPaymentMethodRelations(announcement, context);
                // Remove tag relations.
                ClearTagRelations(announcement, context);
                // Remove schedule items relations.
                ClearScheduleItemsRelations(announcement, context);
            }

            Address viewModelAddress = new Address
            {
                Country = viewModel.Country,
                Region = viewModel.Region,
                City = viewModel.City,
                Street = viewModel.Street,
                StreetNumber = viewModel.StreetNumber
            };

            Address addressInDatabase = SearchEngine.FindAddressByContent(viewModelAddress, context);

            // If there is already similar address in database add it.
            if (addressInDatabase != null)
            {
                announcement.Address = addressInDatabase;
                announcement.AddressFK = addressInDatabase.ID;
            }
            else
            {
                announcement.Address = viewModelAddress;
            }

            announcement.Type = (byte)viewModel.Type;
            announcement.Subtype = (byte)viewModel.Subtype;
            announcement.SharedPart = viewModel.SharedPart;
            announcement.From = viewModel.From;
            announcement.To = viewModel.To;
            announcement.Description = viewModel.Description;
            announcement.User = announcementOwner;
            announcement.Timetable = (byte)viewModel.Timetable;
            announcement.MaxReservations = viewModel.MaxReservations;

            // Add all contacts relations.
            foreach (KeyValuePair<string, byte> contact in viewModel.ContactMethods)
            {
                AdditionalContact viewModelContact = new AdditionalContact { Type = contact.Value, Data = contact.Key };
                AdditionalContact contactInDatabase = SearchEngine.FindAdditionalContact(viewModelContact, context);
                if (contactInDatabase != null)
                {
                    context.AnnouncementToContacts.Add(new AnnouncementToContact
                    { Announcement = announcement, AdditionalContact = contactInDatabase });
                }
                else
                {
                    context.AnnouncementToContacts.Add(new AnnouncementToContact
                    { Announcement = announcement, AdditionalContact = viewModelContact });
                }
            }
            // Add all payment methods relations.
            foreach (KeyValuePair<string, byte> payment in viewModel.PaymentMethods)
            {
                PaymentMethod viewModelPaymentMethod = new PaymentMethod { Type = payment.Value, Data = payment.Key };
                PaymentMethod paymentMethodInDatabase = SearchEngine.FindPaymentMethod(viewModelPaymentMethod, context);
                if (paymentMethodInDatabase != null)
                {

                    context.AnnouncementToPayments.Add(new AnnouncementToPayment
                    { Announcement = announcement, PaymentMethod = paymentMethodInDatabase });
                }
                else
                {
                    context.AnnouncementToPayments.Add(new AnnouncementToPayment
                    { Announcement = announcement, PaymentMethod = viewModelPaymentMethod });
                }
            }
            // Add all schedule items relations.
            foreach (var item in viewModel.ScheduleItems)
            {
                ScheduleItem scheduleItemInDB = SearchEngine.FindScheduleItem((byte)item.From, (byte)item.To, (int)item.MaxReservations, context);
                if (scheduleItemInDB == null)
                {
                    ScheduleItem newScheduleItem = new ScheduleItem() { From = (byte)item.From, To = (byte)item.To, MaxReservations = (int)item.MaxReservations };
                    context.ScheduleItems.Add(newScheduleItem);
                    context.AnnouncementToSchedules.Add(new AnnouncementToSchedule() { Announcement = announcement, ScheduleItem = newScheduleItem });
                }
                else
                {
                    context.AnnouncementToSchedules.Add(new AnnouncementToSchedule() { Announcement = announcement, ScheduleItem = scheduleItemInDB });
                }
            }


            // When all data is provided generate tags.
            foreach (string tag in GenerateTags(announcement))
            {
                AnnouncementTag tagInDb = SearchEngine.FindTag(tag, context);
                if (tagInDb != null)
                {
                    context.AnnouncementToTags
                        .Add(new AnnouncementToTag
                        {
                            Announcement = announcement,
                            AnnouncementID = announcement.ID,
                            AnnouncementTag = tagInDb,
                            AnnouncementTagID = tagInDb.Value
                        });
                }
                else
                {
                    AnnouncementTag newAnnouncementTag = new AnnouncementTag { Value = tag };
                    context.AnnouncementTags
                        .Add(newAnnouncementTag);
                    context.AnnouncementToTags
                        .Add(new AnnouncementToTag
                        {
                            Announcement = announcement,
                            AnnouncementID = announcement.ID,
                            AnnouncementTag = newAnnouncementTag
                        });
                }
            }

            await context.SaveChangesAsync();

        }



        /// <summary>
        /// Generates tags which should be applied to announcement based
        /// on address, type and subtype.
        /// </summary>
        /// <param name="announcement"></param>
        /// <returns></returns>
        private static List<string> GenerateTags(Announcement announcement)
        {
            // Add tags based on address.
            List<string> generatedTags = new List<string>
            {
                announcement.Address.Country.ToUpper(),
                announcement.Address.Region.ToUpper(),
                announcement.Address.City.ToUpper(),
                announcement.Address.Street.ToUpper(),
                announcement.Address.StreetNumber.ToUpper()
            };

            // Add tags based on chosen announcement type.
            foreach (string tag in IoCContainer.PredefinedAnnouncementTags.Value.AnnouncementType[announcement.Type])
            {
                generatedTags.Add(tag.ToUpper());
            }

            // Add tags based on chosen announcement subtype.
            switch (announcement.Type)
            {
                case (int)EnumeratedDbValues.AnnouncementType.House:
                    foreach (string tag in IoCContainer.PredefinedAnnouncementTags.Value.HouseSubtype[announcement.Subtype])
                    {
                        generatedTags.Add(tag.ToUpper());
                    }
                    foreach (string tag in IoCContainer.PredefinedAnnouncementTags.Value.HouseSharedPart[(int)announcement.SharedPart])
                    {
                        generatedTags.Add(tag.ToUpper());
                    }
                    break;
                case (int)EnumeratedDbValues.AnnouncementType.Entertainment:
                    foreach (string tag in IoCContainer.PredefinedAnnouncementTags.Value.EntertainmentSubtype[announcement.Subtype])
                    {
                        generatedTags.Add(tag.ToUpper());
                    }
                    break;
                case (int)EnumeratedDbValues.AnnouncementType.Food:
                    foreach (string tag in IoCContainer.PredefinedAnnouncementTags.Value.FoodSubtype[announcement.Subtype])
                    {
                        generatedTags.Add(tag.ToUpper());
                    }
                    break;
            }
            // Remove duplicates.
            return generatedTags.Distinct().ToList();
        }

        /// <summary>
        /// Removes announcement schedule items relations but only in context, so 
        /// changes must be saved to make this operation persistent.
        /// </summary>
        /// <param name="announcement"></param>
        /// <param name="context"></param>
        private static void ClearScheduleItemsRelations(Announcement announcement, AppDbContext context)
        {
            List<AnnouncementToSchedule> announcementToSchedulesToRemove = context.AnnouncementToSchedules.Where(ats => ats.Announcement == announcement).ToList();
            context.AnnouncementToSchedules.RemoveRange(announcementToSchedulesToRemove);
        }

        /// <summary>
        /// Removes announcement tag relations but only in context, so 
        /// changes must be saved to make this operation persistent.
        /// </summary>
        /// <param name="announcement"></param>
        /// <param name="context"></param>
        private static void ClearTagRelations(Announcement announcement, AppDbContext context)
        {
            List<AnnouncementToTag> tagRelationsToRemove = context.AnnouncementToTags.Where(att => att.Announcement == announcement).ToList();
            context.AnnouncementToTags.RemoveRange(tagRelationsToRemove);
        }

        /// <summary>
        /// Removes contact and payment method relations but only in context, so 
        /// changes must be saved to make this operation persistent.
        /// </summary>
        /// <param name="announcement"></param>
        /// <param name="context"></param>
        private static void ClearContactsAndPaymentMethodRelations(Announcement announcement, AppDbContext context)
        {
            List<AnnouncementToContact> contactRelationsToRemove = context.AnnouncementToContacts.Where(ac => ac.Announcement == announcement).ToList();
            List<AnnouncementToPayment> paymentMethodRelationsToRemove = context.AnnouncementToPayments.Where(ap => ap.Announcement == announcement).ToList();
            context.AnnouncementToContacts.RemoveRange(contactRelationsToRemove);
            context.AnnouncementToPayments.RemoveRange(paymentMethodRelationsToRemove);
        }


        /// <summary>
        /// Simply sets user "is host" flag as true.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static async Task MakeUserHost(User user, AppDbContext context)
        {
            user.isHost = true;
            await context.SaveChangesAsync();
        }

        /// <summary>
        /// Creates and returns list of announcements parsed into view model list which
        /// is easier to work with for view.
        /// </summary>
        /// <param name="announcements"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static List<EditAnnouncementViewModel> ParseAnnouncementsToViewModelList(List<Announcement> announcements,
            AppDbContext context)
        {
            if (announcements == null)
            {
                return null;
            }
            List<Dictionary<string, byte>> contacts = GetListOfAdditonalContacts(announcements, context);
            List<Dictionary<string, byte>> payments = GetListOfPaymentMehtods(announcements, context);
            List<List<ScheduleItem>> scheduleItems = GetListOfScheduleItems(announcements, context);

            // Parse list of schedule items from database to list of schedule items of view model.
            List<List<ScheduleItemViewModel>> scheduleItemsViewModels = new List<List<ScheduleItemViewModel>>();
            foreach (List<ScheduleItem> list in scheduleItems)
            {
                List<ScheduleItemViewModel> scheduleItemsViewModel = new List<ScheduleItemViewModel>();
                foreach (ScheduleItem scheduleItem in list)
                {
                    scheduleItemsViewModel.Add(new ScheduleItemViewModel() { From = scheduleItem.From, To = scheduleItem.To, MaxReservations = scheduleItem.MaxReservations });
                }
                scheduleItemsViewModels.Add(scheduleItemsViewModel);
            }


            List<EditAnnouncementViewModel> viewModelList = new List<EditAnnouncementViewModel>();
            int index = 0;
            foreach (Announcement announcement in announcements)
            {
                viewModelList.Add(new EditAnnouncementViewModel
                {
                    ID = announcement.ID,
                    Type = announcement.Type,
                    Subtype = announcement.Subtype,
                    SharedPart = announcement.SharedPart,
                    Country = announcement.Address.Country,
                    Region = announcement.Address.Region,
                    City = announcement.Address.City,
                    Street = announcement.Address.Street,
                    StreetNumber = announcement.Address.StreetNumber,
                    From = announcement.From,
                    To = announcement.To,
                    ContactMethods = contacts[index],
                    Description = announcement.Description,
                    PaymentMethods = payments[index],
                    IsActive = announcement.IsActive,
                    Removed = announcement.Removed,
                    MaxReservations = announcement.MaxReservations,
                    Timetable = announcement.Timetable,
                    ScheduleItems = scheduleItemsViewModels[index]
                });
                index++;
            }
            return viewModelList;
        }

        private static List<List<ScheduleItem>> GetListOfScheduleItems(List<Announcement> usersAnnouncements, AppDbContext context)
        {
            List<List<ScheduleItem>> scheduleItems = new List<List<ScheduleItem>>();
            foreach (Announcement announcement in usersAnnouncements)
            {
                // Get schedule items related with this announcement.
                List<ScheduleItem> announcementScheduleItems = context.AnnouncementToSchedules
                    .Where(ats => ats.Announcement == announcement).Select(ats => ats.ScheduleItem).ToList();
                scheduleItems.Add(announcementScheduleItems);
            }
            return scheduleItems;
        }

        /// <summary>
        /// Finds and returns list of additional contacts for announcements which are
        /// stored as dictionary. Order is based on list of announcements provided.
        /// </summary>
        /// <param name="usersAnnouncements"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        private static List<Dictionary<string, byte>> GetListOfAdditonalContacts(List<Announcement> usersAnnouncements, AppDbContext context)
        {
            var contactData = (from ua in usersAnnouncements
                               join ac in context.AnnouncementToContacts
                               on ua.ID equals ac.AnnouncementID
                               select new { announcementID = ua.ID, contactID = ac.AdditionalContactID });

            List<Dictionary<string, byte>> contacts = new List<Dictionary<string, byte>>();
            foreach (Announcement announcement in usersAnnouncements)
            {
                List<int> announcementsContacsIDs = (from d in contactData
                                                     where d.announcementID == announcement.ID
                                                     select d.contactID).ToList();
                List<AdditionalContact> additionalContacts = (from ac in context.AdditionalContacts
                                                              where announcementsContacsIDs.Contains(ac.ID)
                                                              select ac).ToList();
                Dictionary<string, byte> announcementContacts = new Dictionary<string, byte>();
                foreach (AdditionalContact additionalContact in additionalContacts)
                {
                    announcementContacts.Add(key: additionalContact.Data, value: additionalContact.Type);
                }
                contacts.Add(announcementContacts);
            }
            return contacts;
        }

        /// <summary>
        /// Finds and returns list of payment methods for announcements which are
        /// stored as dictionary. Order is based on list of announcements provided.
        /// </summary>
        /// <param name="usersAnnouncements"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        private static List<Dictionary<string, byte>> GetListOfPaymentMehtods(List<Announcement> usersAnnouncements, AppDbContext context)
        {
            var paymentData = (from ua in usersAnnouncements
                               join ap in context.AnnouncementToPayments
                               on ua.ID equals ap.AnnouncementID
                               select new { announcementID = ua.ID, paymentID = ap.PaymentMethodID });

            List<Dictionary<string, byte>> payments = new List<Dictionary<string, byte>>();
            foreach (Announcement announcement in usersAnnouncements)
            {
                List<int> announcementsPaymentsIDs = (from d in paymentData
                                                      where d.announcementID == announcement.ID
                                                      select d.paymentID).ToList();
                List<PaymentMethod> paymentMethods = (from pm in context.PaymentMethods
                                                      where announcementsPaymentsIDs.Contains(pm.ID)
                                                      select pm).ToList();
                Dictionary<string, byte> announcementPayments = new Dictionary<string, byte>();
                foreach (PaymentMethod paymentMethod in paymentMethods)
                {
                    announcementPayments.Add(key: paymentMethod.Data, value: paymentMethod.Type);
                }
                payments.Add(announcementPayments);
            }
            return payments;
        }
    }

}
