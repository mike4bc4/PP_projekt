using BedAndBreakfast.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BedAndBreakfast.Models.ServicesLogic
{
    public static class HostingServiceLogic
    {

        public static bool IsAnnouncementViewModelValid(EditAnnouncementViewModel viewModel)
        {
            // Validate received view model.
            bool announcementCorrect = true;
            if (viewModel.Type == null)
            {
                announcementCorrect = false;
            }
            // Continue validation only if current model state is valid.
            // Check subtype.
            if (announcementCorrect == true && viewModel.Type == ListedDbValues.AnnouncementType.House.ToString())
            {
                if (viewModel.Subtype == null || viewModel.SharedPart == null)
                {
                    announcementCorrect = false;
                }
            }
            else
            {
                if (viewModel.Subtype == null)
                {
                    announcementCorrect = false;
                }
            }
            // Check address.
            if (announcementCorrect == true && viewModel.Country == null || viewModel.Region == null || viewModel.City == null || viewModel.Street == null || viewModel.StreetNumber == null)
            {
                announcementCorrect = false;
            }
            // Check time.
            if (announcementCorrect == true && DateTime.Compare(viewModel.From, viewModel.To) > 0 || DateTime.Compare(viewModel.From, DateTime.Today) < 0)
            {
                announcementCorrect = false;
            }
            // Check description.
            if (announcementCorrect == true && viewModel.Description == null)
            {
                announcementCorrect = false;
            }
            // Check contacts
            if (announcementCorrect == true && viewModel.ContactMethods.Count() > 0)
            {
                if (string.IsNullOrEmpty(viewModel.ContactMethods.FirstOrDefault().Key) || string.IsNullOrEmpty(viewModel.ContactMethods.FirstOrDefault().Value))
                {
                    announcementCorrect = false;
                }
            }
            else
            {
                announcementCorrect = false;
            }
            // Check payment methods.
            if (announcementCorrect == true && viewModel.PaymentMethods.Count() > 0)
            {
                if (string.IsNullOrEmpty(viewModel.PaymentMethods.FirstOrDefault().Key) || string.IsNullOrEmpty(viewModel.PaymentMethods.FirstOrDefault().Value))
                {
                    announcementCorrect = false;
                }
            }
            else
            {
                announcementCorrect = false;
            }
            return announcementCorrect;
        }

        public static async Task SaveAnnouncementToDatabase(EditAnnouncementViewModel viewModel, AppDbContext context, User announcementOwner, bool newModel)
        {
            Announcement announcement;
            if (newModel)
                announcement = new Announcement();
            else
            {
                // If model is edited it has to be found in database by provided ID.
                announcement = context.Announcements.Where(a => a.ID == viewModel.ID).Single();
                // All payment methods and contact relations are removed to avoid duplicate relations and
                // saving back those which were removed by user.
                ClearContactsAndPaymentMethods(announcement, context);
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

            announcement.Type = viewModel.Type;
            announcement.Subtype = viewModel.Subtype;
            announcement.SharedPart = viewModel.SharedPart;
            announcement.From = viewModel.From;
            announcement.To = viewModel.To;
            announcement.Description = viewModel.Description;
            announcement.User = announcementOwner;

            foreach (KeyValuePair<string, string> contact in viewModel.ContactMethods)
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

            foreach (KeyValuePair<string, string> payment in viewModel.PaymentMethods)
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

            await context.SaveChangesAsync();

        }

        /// <summary>
        /// Removes contact and payment method relations but only in context, so 
        /// changes must be saved to make this operation persistent.
        /// </summary>
        /// <param name="announcement"></param>
        /// <param name="context"></param>
        private static void ClearContactsAndPaymentMethods(Announcement announcement, AppDbContext context) {
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
            List<Dictionary<string, string>> contacts = GetListOfAdditonalContacts(announcements, context);
            List<Dictionary<string, string>> payments = GetListOfPaymentMehtods(announcements, context);
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
                    Removed = announcement.Removed
                });
                index++;
            }
            return viewModelList;
        }

        /// <summary>
        /// Finds and returns list of additional contacts for announcements which are
        /// stored as dictionary. Order is based on list of announcements provided.
        /// </summary>
        /// <param name="usersAnnouncements"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        private static List<Dictionary<string, string>> GetListOfAdditonalContacts(List<Announcement> usersAnnouncements, AppDbContext context)
        {
            var contactData = (from ua in usersAnnouncements
                               join ac in context.AnnouncementToContacts
                               on ua.ID equals ac.AnnouncementID
                               select new { announcementID = ua.ID, contactID = ac.AdditionalContactID });

            List<Dictionary<string, string>> contacts = new List<Dictionary<string, string>>();
            foreach (Announcement announcement in usersAnnouncements)
            {
                List<int> announcementsContacsIDs = (from d in contactData
                                                     where d.announcementID == announcement.ID
                                                     select d.contactID).ToList();
                List<AdditionalContact> additionalContacts = (from ac in context.AdditionalContacts
                                                              where announcementsContacsIDs.Contains(ac.ID)
                                                              select ac).ToList();
                Dictionary<string, string> announcementContacts = new Dictionary<string, string>();
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
        private static List<Dictionary<string, string>> GetListOfPaymentMehtods(List<Announcement> usersAnnouncements, AppDbContext context)
        {
            var paymentData = (from ua in usersAnnouncements
                               join ap in context.AnnouncementToPayments
                               on ua.ID equals ap.AnnouncementID
                               select new { announcementID = ua.ID, paymentID = ap.PaymentMethodID });

            List<Dictionary<string, string>> payments = new List<Dictionary<string, string>>();
            foreach (Announcement announcement in usersAnnouncements)
            {
                List<int> announcementsPaymentsIDs = (from d in paymentData
                                                      where d.announcementID == announcement.ID
                                                      select d.paymentID).ToList();
                List<PaymentMethod> paymentMethods = (from pm in context.PaymentMethods
                                                      where announcementsPaymentsIDs.Contains(pm.ID)
                                                      select pm).ToList();
                Dictionary<string, string> announcementPayments = new Dictionary<string, string>();
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
