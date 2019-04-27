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

        public static async Task AddAnnouncementToDatabase(EditAnnouncementViewModel viewModel, AppDbContext context, User announcementOwner)
        {
            Announcement announcement = new Announcement();
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
                AdditionalContact contactInDatabase = SearchEngine.FindAdditionalContactByContent(viewModelContact, context);
                if (contactInDatabase != null)
                {
                    context.AnnouncementToContacts.Add(new AnnouncementToContact { Announcement = announcement, AdditionalContact = contactInDatabase });
                }
                else
                {
                    context.AnnouncementToContacts.Add(new AnnouncementToContact { Announcement = announcement, AdditionalContact = viewModelContact });
                }
            }

            foreach (KeyValuePair<string, string> payment in viewModel.PaymentMethods)
            {
                PaymentMethod viewModelPaymentMethod = new PaymentMethod { Type = payment.Value, Data = payment.Key };
                PaymentMethod paymentMethodInDatabase = SearchEngine.FindPaymentMoethodByContent(viewModelPaymentMethod, context);
                if (paymentMethodInDatabase != null)
                {
                    context.AnnouncementToPayments.Add(new AnnouncementToPayment { Announcement = announcement, PaymentMethod = paymentMethodInDatabase });
                }
                else
                {
                    context.AnnouncementToPayments.Add(new AnnouncementToPayment { Announcement = announcement, PaymentMethod = viewModelPaymentMethod });
                }
            }

            await context.SaveChangesAsync();

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


        public static List<EditAnnouncementViewModel> ParseAnnouncementsToViewModelList(List<Announcement> announcements,
            List<Dictionary<string, string>> contacts, List<Dictionary<string, string>> payments)
        {
            if (announcements == null)
            {
                return null;
            }
            List<EditAnnouncementViewModel> viewModelList = new List<EditAnnouncementViewModel>();
            int index = 0;
            foreach (Announcement announcement in announcements) {
                viewModelList.Add(new EditAnnouncementViewModel {
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
                    IsLocked = announcement.IsLocked
                });
                index++;
            }
            return viewModelList;

        }

    }

}
