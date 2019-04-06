using BedAndBreakfast.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BedAndBreakfast.Settings
{
    public static class PredefinedTablesContainer
    {
        public static List<MsgTypeDictionary> MsgTypeDictionaries = new List<MsgTypeDictionary>
        {
            new MsgTypeDictionary{Code = "GEN", Name = "General"},
            new MsgTypeDictionary{Code = "REM", Name = "Reminds"},
            new MsgTypeDictionary{Code = "DSC", Name = "Discounts and tips"},
            new MsgTypeDictionary{Code = "RUL", Name = "Rules and community"},
            new MsgTypeDictionary{Code = "SRV", Name = "Service"}
        };

    }
}
