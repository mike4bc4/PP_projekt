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
