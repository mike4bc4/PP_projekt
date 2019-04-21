using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BedAndBreakfast.Data
{
    /// <summary>
    /// This class contains abbreviations for gender value stored in database.
    /// </summary>
    public static class ListedDbValues
    {
        public enum Gender
        {
            Male,          
            Female,          
            Unspecified          
        };

        public enum Currency {
            USD,
            EUR,
            JPY,
            GBP,
            CHF,
            PLN
        };

        public enum Language {
            en_EN,
            en_US,
            pl_PL,
            fr_FR,
            it_IT,
            ru_RU,
            es_ES
        }

		/// <summary>
		/// Represents types of announcements that user may be host of.
		/// </summary>
		public enum AnnouncementType {
			House,
			Entertainment,
			Food
		}

		public enum HouseSubtype {
			Apartment,
			House,
			Bungalow,
			Hotel,
			Other
		}

		public enum EntertainmentSubtype
		{
			Party,
			Art,
			Trip,
			Science,
			Other
		}

		public enum FoodSubtype
		{
			Restaurant,
			Foodtruck,
			Bar,
			Pub,
			Other
		}

		public enum HouseSharedPart {
			Exterior,
			Room,
			Rooms,
			RoomWithBathroom,
			RoomsWithBathroom,
			Floor,
			FloorWithBathroom,
			House,
			Other,
			NotApplicable
		}

		public enum ContactType {
			EmailAddress,
			PhoneNumber,
			Other
		}

		public enum PaymentMethod {
			PayPal,
			BankTransfer,
			Other
		}





		public static List<SelectListItem> CreateListOfItems<T>(string currentlySelectedValue) {
            List<SelectListItem> list = new List<SelectListItem>();
            foreach (T item in Enum.GetValues(typeof(T)))
            {
                list.Add(new SelectListItem
                {
                    Text = item.ToString(),
                    Value = item.ToString(),
                    Selected = (currentlySelectedValue == item.ToString())
                });

            }
            return list;
        }
      
    }
}
