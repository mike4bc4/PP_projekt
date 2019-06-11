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
        /// <summary>
        /// Generates tags for provided announcement based on type, subtype and
        /// address. Tags that does not exist in database are added and then
        /// relations are created. Note that all changes have to be saved to persist.
        /// </summary>
        /// <param name="announcement"></param>
        /// <returns></returns>
        public static async Task<int> CreateTagsForAnnouncement(Announcement announcement, AppDbContext context)
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
            generatedTags = generatedTags.Distinct().ToList();

            List<AnnouncementTag> dbAnnouncementTags = new List<AnnouncementTag>();
            List<AnnouncementTag> newAnnouncementTags = new List<AnnouncementTag>();
            foreach (string tag in generatedTags)
            {
                AnnouncementTag dbAnnouncementTag = await context.AnnouncementTags
                    .Where(at => at.Value == tag)
                    .SingleOrDefaultAsync();
                if (dbAnnouncementTag != null)
                {
                    dbAnnouncementTags.Add(dbAnnouncementTag);
                }
                else
                {
                    newAnnouncementTags.Add(new AnnouncementTag()
                    {
                        Value = tag,
                    });
                }
            }
            // Add new tags to context.
            await context.AnnouncementTags.AddRangeAsync(newAnnouncementTags);
            // Create relations.
            List<AnnouncementToTag> announcementToTags = new List<AnnouncementToTag>();
            foreach (AnnouncementTag tag in dbAnnouncementTags)
            {
                announcementToTags.Add(new AnnouncementToTag()
                {
                    Announcement = announcement,
                    AnnouncementTag = tag,
                });
            }
            foreach (AnnouncementTag tag in newAnnouncementTags)
            {
                announcementToTags.Add(new AnnouncementToTag()
                {
                    Announcement = announcement,
                    AnnouncementTag = tag,
                });
            }
            // Add relations to context.
            await context.AnnouncementToTags.AddRangeAsync(announcementToTags);

            return 0;
        }

        /// <summary>
        /// Removes announcement schedule items relations but only in context, so 
        /// changes must be saved to make this operation persistent.
        /// </summary>
        /// <param name="announcement"></param>
        /// <param name="context"></param>
        public static void ClearScheduleItemsRelations(Announcement announcement, AppDbContext context)
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
        public static void ClearTagRelations(Announcement announcement, AppDbContext context)
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
        public static void ClearContactsAndPaymentMethodRelations(Announcement announcement, AppDbContext context)
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
            user.IsHost = true;
            await context.SaveChangesAsync();
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
        /// <summary>
        /// Returns list amount of reservations per day or per schedule item related with announcement.
        /// If announcement does not use timetable (timetable option equals 0) returns null.
        /// Note that returned list size is based on announcement related schedule items or is equal 7
        /// for per day timetable (where 3rd element represents reservations per date specified as function parameter).
        /// If list represents reservations per schedule item, it is ordered by schedule item begin hour.
        /// </summary>
        /// <param name="announcementID"></param>
        /// <param name="date"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static List<int?> GetReservations(Announcement announcement, DateTime date, AppDbContext context)
        {
            if (announcement.Timetable == 1)    // Timetable per day
            {
                DateTime dateRangeLowerLimit = date.AddDays(-3);
                DateTime dateRangeUpperLimit = date.AddDays(3);
                List<int?> reservations = new List<int?>();
                List<DateTime> timeRange = new List<DateTime>();
                for (int i = 0; i < 7; i++)
                {
                    timeRange.Add(dateRangeLowerLimit.AddDays(i));
                    reservations.Add(null);
                }
                var reservationsPerDay = context.Reservations.Where(r => r.Announcement == announcement)
                    .Where(r => r.Date.Date >= dateRangeLowerLimit.Date)
                    .Where(r => r.Date.Date <= dateRangeUpperLimit.Date)
                    .GroupBy(r => r.Date)
                    .Select(grp => new { date = grp.Key, reservations = grp.Count() })
                    .ToList();
                int index = 0;
                foreach (DateTime day in timeRange)
                {
                    foreach (var item in reservationsPerDay)
                    {
                        if (day.Date == item.date.Date)
                        {
                            reservations[index] = item.reservations;
                        }
                    }
                    index++;
                }
                return reservations;
            }
            else if (announcement.Timetable == 2)   // Timetable per hour
            {
                List<int> scheduleItemsFromList = new List<int>();
                List<int?> reservations = new List<int?>();
                List<ScheduleItem> announcementScheduleItems = context.AnnouncementToSchedules
                    .Where(ats => ats.Announcement == announcement)
                    .Select(ats => ats.ScheduleItem)
                    .OrderBy(s => s.From)
                    .ToList();
                foreach (ScheduleItem item in announcementScheduleItems)
                {
                    reservations.Add(null);
                    scheduleItemsFromList.Add(item.From);
                }
                var reservationsPerScheduleItem = context.Reservations
                    .Include(r => r.ScheduleItem)
                    .Where(r => r.Announcement == announcement)
                    .Where(r => r.Date.Date == date.Date)
                    .GroupBy(r => r.ScheduleItem)
                    .Select(grp => new { scheduleItemFromDate = grp.Key.From, reservations = grp.Count() });
                int index = 0;
                foreach (int from in scheduleItemsFromList)
                {
                    foreach (var item in reservationsPerScheduleItem)
                    {
                        if (from == item.scheduleItemFromDate)
                        {
                            reservations[index] = item.reservations;
                        }
                    }
                    index++;
                }
                return reservations;
            }
            return null;
        }

        /// <summary>
        /// Either finds address that is the same as address provided in database
        /// or creates new one, adds it to db context and then to provided announcement.
        /// </summary>
        /// <param name="announcementViewModel"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static async Task<int> CreateAddressForAnnouncement(SaveAnnouncementViewModel announcementViewModel, Announcement announcement, AppDbContext context)
        {
            // Check if provided address already exists in database.
            Address newAddress = new Address();
            Address dbAddress = await context.Addresses
                .Where(a => a.Country == announcementViewModel.Country)
                .Where(a => a.Region == announcementViewModel.Region)
                .Where(a => a.City == announcementViewModel.City)
                .Where(a => a.Street == announcementViewModel.Street)
                .Where(a => a.StreetNumber == announcementViewModel.StreetNumber)
                .SingleOrDefaultAsync();
            if (dbAddress != null)
            {
                newAddress = dbAddress;
            }
            else
            {
                // Create new address object if it does not exists.
                newAddress = new Address()
                {
                    Country = announcementViewModel.Country,
                    Region = announcementViewModel.Region,
                    City = announcementViewModel.City,
                    Street = announcementViewModel.Street,
                    StreetNumber = announcementViewModel.StreetNumber,
                };
                // Add address to context
                await context.Addresses.AddAsync(newAddress);
            }
            announcement.Address = newAddress;
            return 0;
        }

        /// <summary>
        /// Creates contacts and relations between provided announcement and
        /// contacts created or these stored in database that are the same as
        /// contacts given in view model. Note that context changes have to be saved
        /// to persist.
        /// </summary>
        /// <param name="announcementViewModel"></param>
        /// <param name="announcement"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static async Task<int> CreateContactsForAnnoucement(SaveAnnouncementViewModel announcementViewModel, Announcement announcement, AppDbContext context)
        {
            // Check if contacts defined in view model exist in database.
            List<AdditionalContact> newContacts = new List<AdditionalContact>();
            List<AdditionalContact> dbContacts = new List<AdditionalContact>();
            foreach (ContactPaymentItem item in announcementViewModel.Contacts)
            {
                AdditionalContact dbContact = await context.AdditionalContacts
                    .Where(ac => ac.Type == item.Type)
                    .Where(ac => ac.Data == item.Value)
                    .SingleOrDefaultAsync();
                // If such contact was found in database add it to list
                // else create new one.
                if (dbContact != null)
                {
                    dbContacts.Add(dbContact);
                }
                else
                {
                    newContacts.Add(new AdditionalContact()
                    {
                        Data = item.Value,
                        Type = (byte)item.Type,
                    });
                }
            }
            // Add new contacts to context.
            await context.AdditionalContacts.AddRangeAsync(newContacts);
            // Create list of relations.
            List<AnnouncementToContact> announcementToContacts = new List<AnnouncementToContact>();
            foreach (AdditionalContact contact in dbContacts)
            {
                announcementToContacts.Add(new AnnouncementToContact()
                {
                    Announcement = announcement,
                    AdditionalContact = contact,
                });
            }
            foreach (AdditionalContact contact in newContacts)
            {
                announcementToContacts.Add(new AnnouncementToContact()
                {
                    Announcement = announcement,
                    AdditionalContact = contact,
                });
            }
            // Add relations to contaxt.
            await context.AnnouncementToContacts.AddRangeAsync(announcementToContacts);
            return 0;
        }

        /// <summary>
        /// Creates payments and relations between provided announcement and
        /// payments created or these stored in database that are the same as
        /// payments given in view model. Note that context changes have to be saved
        /// to persist.
        /// </summary>
        /// <param name="announcementViewModel"></param>
        /// <param name="announcement"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static async Task<int> CreatePaymentsForAnnoucement(SaveAnnouncementViewModel announcementViewModel, Announcement announcement, AppDbContext context)
        {
            // Check if payments defined in view model exist in database.
            List<PaymentMethod> newPayments = new List<PaymentMethod>();
            List<PaymentMethod> dbPayments = new List<PaymentMethod>();
            foreach (ContactPaymentItem item in announcementViewModel.Payments)
            {
                PaymentMethod dbPayment = await context.PaymentMethods
                    .Where(pm => pm.Type == item.Type)
                    .Where(pm => pm.Data == item.Value)
                    .SingleOrDefaultAsync();
                // If such payment was found in database add it to list
                // else create new one.
                if (dbPayment != null)
                {
                    dbPayments.Add(dbPayment);
                }
                else
                {
                    newPayments.Add(new PaymentMethod()
                    {
                        Data = item.Value,
                        Type = (byte)item.Type,
                    });
                }
            }
            // Add new payments to context.
            await context.PaymentMethods.AddRangeAsync(newPayments);
            // Create list of relations.
            List<AnnouncementToPayment> announcementToPayments = new List<AnnouncementToPayment>();
            foreach (PaymentMethod payment in dbPayments)
            {
                announcementToPayments.Add(new AnnouncementToPayment()
                {
                    Announcement = announcement,
                    PaymentMethod = payment,
                });
            }
            foreach (PaymentMethod payment in newPayments)
            {
                announcementToPayments.Add(new AnnouncementToPayment()
                {
                    Announcement = announcement,
                    PaymentMethod = payment,
                });
            }
            // Add relations to context.
            await context.AnnouncementToPayments.AddRangeAsync(announcementToPayments);
            return 0;
        }

        /// <summary>
        /// Creates schedule items and relations to them. Schedule items are not
        /// created if similar to those provided in view model are in database.
        /// Then just simple relations with existing ones are created. Note that changes
        /// need to be saved to persist.
        /// </summary>
        /// <param name="announcementViewModel"></param>
        /// <param name="announcement"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static async Task<int> CreateScheduleItemsForAnnoucement(SaveAnnouncementViewModel announcementViewModel, Announcement announcement, AppDbContext context)
        {
            // Get schedule items that already exist in database or create new ones if the does not.
            List<ScheduleItem> dbScheduleItems = new List<ScheduleItem>();
            List<ScheduleItem> newScheduleItems = new List<ScheduleItem>();
            foreach (ScheduleItemViewModel scheduleItem in announcementViewModel.ScheduleItems)
            {
                ScheduleItem dbScheduleItem = await context.ScheduleItems
                    .Where(si => si.MaxReservations == scheduleItem.MaxReservations)
                    .Where(si => si.From == scheduleItem.From)
                    .Where(si => si.To == scheduleItem.To)
                    .SingleOrDefaultAsync();
                if (dbScheduleItem != null)
                {
                    dbScheduleItems.Add(dbScheduleItem);
                }
                else
                {
                    newScheduleItems.Add(new ScheduleItem()
                    {
                        From = (byte)scheduleItem.From,
                        To = (byte)scheduleItem.To,
                        MaxReservations = (int)scheduleItem.MaxReservations,
                    });
                }
            }
            // Add new schedule items to context.
            await context.ScheduleItems.AddRangeAsync(newScheduleItems);
            // Create relations between announcement and schedule items.
            List<AnnouncementToSchedule> announcementToSchedule = new List<AnnouncementToSchedule>();
            foreach (ScheduleItem scheduleItem in dbScheduleItems)
            {
                announcementToSchedule.Add(new AnnouncementToSchedule()
                {
                    Announcement = announcement,
                    ScheduleItem = scheduleItem,
                });
            }
            foreach (ScheduleItem scheduleItem in newScheduleItems)
            {
                announcementToSchedule.Add(new AnnouncementToSchedule()
                {
                    Announcement = announcement,
                    ScheduleItem = scheduleItem,
                });
            }
            // Add created relations to context.
            await context.AnnouncementToSchedules.AddRangeAsync(announcementToSchedule);
            return 0;
        }

    }
}
