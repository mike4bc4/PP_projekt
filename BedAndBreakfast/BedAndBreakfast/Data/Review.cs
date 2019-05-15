using BedAndBreakfast.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BedAndBreakfast.Data
{
    public class Review
    {
        [Key]
        public int ReviewID { get; set; }
        public int AnnouncementID { get; set; }
        public Announcement Announcement { get; set; }
        public string UserID { get; set; }
        public User User { get; set; }
        public string Name { get; set; }
        public byte? Rating { get; set; }
        public string Content { get; set; }
        public DateTime ReviewDate { get; set; }
    }
}
