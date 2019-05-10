using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BedAndBreakfast.Data
{
    /// <summary>
    /// This class contains abbreviations for values stored in database.
    /// </summary>
    public static class EnumeratedDbValues
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
			House = 0,
			Entertainment = 1,
			Food = 2
		}

		public enum HouseSubtype {
			Apartment = 0,
			House = 1,
			Bungalow = 2,
			Hotel = 3,
			Other = 4
		}

		public enum EntertainmentSubtype
		{
			Party = 0,
			Art = 1,
			Trip = 2,
			Science = 3,
			Other = 4
		}

		public enum FoodSubtype
		{
			Restaurant = 0,
			Foodtruck = 1,
			Bar = 2,
			Pub = 3,
			Other = 4
		}

		public enum HouseSharedPart {
			Exterior = 0,
			Room = 1,
			Rooms = 2,
			RoomWithBathroom = 3,
			RoomsWithBathroom = 4,
			Floor = 5,
			FloorWithBathroom = 6,
			House = 7,
			Other = 8,
			NotApplicable = 9
		}

		public enum ContactType {
			EmailAddress = 0,
			PhoneNumber = 1,
			Other = 2
		}

		public enum PaymentMethod {
			PayPal = 0,
			BankTransfer = 1,
			Other = 2
		}

        public enum AnnouncementSortOptions {
            TypeAsc = 0,
            TypeDesc = 1,
            SubtypeAsc = 2,
            SubtypeDesc = 3,
            FromDateAsc = 4,
            FromDateDesc = 5,
            ToDateAsc = 6,
            ToDateDesc = 7,
            IsActive = 8,
            IsInactive = 9
        }

        public enum AnnouncementTimetableOptions {
            Off = 0,
            PerDay = 1,
            PerHour = 2
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

        public static Dictionary<string, int> ParseEnumToDictionary<T>() {
            Array enumVaules = Enum.GetValues(typeof(T));
            Dictionary<string, int> dict = new Dictionary<string, int>();
            foreach (int enumValue in enumVaules) {
                dict.Add(Enum.GetName(typeof(T), enumValue), enumValue);
            }
            return dict;
        }
      
    }
}
