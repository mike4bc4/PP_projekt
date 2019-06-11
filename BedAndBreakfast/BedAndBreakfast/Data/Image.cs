using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BedAndBreakfast.Data
{
    public class Image
    {
        [Key]
        public int ImageID { get; set; }
        public int AnnouncementID { get; set; }
        public Announcement Announcement { get; set; }
        public string ImageName { get; set; }
        public byte[] StoredImage { get; set; }
    }
}
