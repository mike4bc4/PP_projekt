using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BedAndBreakfast.Data
{
    /// <summary>
    /// This class contains abbreviations for gender value stored in database.
    /// </summary>
    public class ListedDbValues
    {
        public enum Gender
        {
            Male = 'M',
            Female = 'F',
            Unspecified = 'U'
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
    }
}
