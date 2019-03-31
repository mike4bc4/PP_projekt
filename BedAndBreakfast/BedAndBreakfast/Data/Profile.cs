using BedAndBreakfast.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BedAndBreakfast.Data
{
    public class Profile
    {

        // Keys

        [Key]
        public int ProfileID { get; set; }

        public User User { get; set; }

        // Data fields

        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; }

        public char Gender { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }

        [MaxLength(10)]
        public string PrefLanguage { get; set; }

        [DataType(DataType.Currency)]
        public string PrefCurrency { get; set; }

        [MaxLength(50)]
        public string Country { get; set; }

        [MaxLength(50)]
        public string Region { get; set; }

        [MaxLength(50)]
        public string City { get; set; }

        [MaxLength(50)]
        public string Street { get; set; }

        [MaxLength(10)]
        public string StreetNumber { get; set; }

        [MaxLength(1024)]
        public string PresonalDescription { get; set; }

        [MaxLength(100)]
        public string School { get; set; }

        [MaxLength(100)]
        public string Work { get; set; }

        [DataType(DataType.EmailAddress)]
        public string BackupEmailAddress { get; set; }


    }
}
