using Microsoft.AspNetCore.Mvc.ViewEngines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BedAndBreakfast.Models.ServicesLogic
{
    public static class SharedServiceLogic
    {
        public static bool IsFindUserViewModelEmpty(FindUserViewModel viewModel)
        {
            if (viewModel.FristName == null &&
                viewModel.LastName == null &&
                viewModel.UserName == null &&
                !viewModel.IsLocked)
            {
                return true;
            }
            return false;

        }

     }

}
