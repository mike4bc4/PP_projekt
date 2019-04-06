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

        // Profile foreign key must allow null to create administrator account properly.
        public int? ProfileFK { get; set; }

        // Data fields

        public Profile Profile { get; set; }
        public List<ReceiveMsgSetting> ReceiveMsgSettings { get; set; }

        
    }
}
