using BedAndBreakfast.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BedAndBreakfast.Data
{
    /// <summary>
    /// This class represents database entity which contains information about
    /// messages that are received by user.
    /// </summary>
    public class ReceiveMsgSetting
    {

        [Key]
        public int ID { get; set; }
        [MaxLength(3)]
        public string TypeFK { get; set; }
        public MsgTypeDictionary MsgTypeDictionary { get; set; }
        [MaxLength(450)] 
        public string UserFK { get; set; }
        public User User { get; set; }
        public bool ByEmail { get; set; } = true;
        public bool ByMobileApp { get; set; } = true;
        public bool BySMS { get; set; } = false;
        public bool ByPhone { get; set; } = false;
    }
}
