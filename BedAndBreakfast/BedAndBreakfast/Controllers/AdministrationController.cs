using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BedAndBreakfast.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BedAndBreakfast.Controllers
{
    /// <summary>
    /// This controller is used to provide all methods necessary to 
    /// perform web service administration duties.
    /// Only administrator has access to these.
    /// </summary>
    [Authorize(Roles = Role.Admin)]
    public class AdministrationController : Controller
    {
        



    }
}