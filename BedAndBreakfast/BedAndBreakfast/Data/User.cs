using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BedAndBreakfast.Models
{
    public class User : IdentityUser
    {
        // Put additional user db fields in here.

        //public int ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
