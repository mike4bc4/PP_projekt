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
        /// <summary>
        /// Default constructor
        /// </summary>
        public ReceiveMsgSetting() { }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="setting"></param>
        public ReceiveMsgSetting(ReceiveMsgSetting setting) {
            this.ID = setting.ID;
            this.TypeFK = setting.TypeFK;
            this.MsgTypeDictionary = setting.MsgTypeDictionary;
            this.UserFK = setting.UserFK;
            this.User = setting.User;
            this.ByEmail = setting.ByEmail;
            this.ByMobileApp = setting.ByMobileApp;
            this.BySMS = setting.BySMS;
            this.ByPhone = setting.ByPhone;
        }


        [Key]
        public int ID { get; set; }
        [MaxLength(3)]
        public string TypeFK { get; set; }
        public MsgTypeDictionary MsgTypeDictionary { get; set; }
        [MaxLength(450)] 
        public string UserFK { get; set; }
        public User User { get; set; }
        public bool ByEmail { get; set; }
        public bool ByMobileApp { get; set; }
        public bool BySMS { get; set; }
        public bool ByPhone { get; set; }
    }
}
