using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BedAndBreakfast.Data
{
    public class AnnouncementTag
    {
        [Key]
        [MaxLength(50)]
        public string Value { get; set; }
        public List<AnnouncementToTag> AnnouncementToTags { get; set; }
    }
}
