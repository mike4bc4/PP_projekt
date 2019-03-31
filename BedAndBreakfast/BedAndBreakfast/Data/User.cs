using BedAndBreakfast.Data;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BedAndBreakfast.Models
{
    public class User : IdentityUser
    {
        // Keys
        //[Key]
        //public int UserID { get; set; }

        // Data fields
        [ForeignKey("ProfileFK")]
        public Profile Profile { get; set; }

        
    }
}
