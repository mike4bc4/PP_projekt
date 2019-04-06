using BedAndBreakfast.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BedAndBreakfast.Data
{
    /// <summary>
    /// This class represents entity of user privacy settings which allow or disallow sharing profile data.
    /// </summary>
    public class PrivacySetting
    {
        [Key]
        public int ID { get; set; }
        [MaxLength(450)]
        public string UserFK { get; set; }
        public User User { get; set; }
        public bool ShowProfileToFriends { get; set; } = true;
        public bool ShowProfileToWorld { get; set; } = true;
    }
}
