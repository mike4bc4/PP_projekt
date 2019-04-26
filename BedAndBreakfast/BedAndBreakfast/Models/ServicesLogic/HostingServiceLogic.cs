using BedAndBreakfast.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BedAndBreakfast.Models.ServicesLogic
{
    public static class HostingServiceLogic { 
    
        public static bool IsAnnouncementViewModelValid(CreateAnnouncementViewModel viewModel) {
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
    
        public static void AddAnnouncementToDatabase(CreateAnnouncementViewModel viewModel) {
            Address address = new Address();
            Announcement announcement = new Announcement();
            announcement.Type = viewModel.Type;
            announcement.Subtype = viewModel.Subtype;
            announcement.SharedPart = viewModel.SharedPart;
           

        }


    }

}
