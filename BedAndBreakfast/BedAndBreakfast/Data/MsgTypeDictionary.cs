using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BedAndBreakfast.Data
{
    /// <summary>
    /// This class represents entity fro database which is simple dictionary with types
    /// of messages used in web application.
    /// </summary>
    public class MsgTypeDictionary
    {
        [Key]
        [MaxLength(3)]
        public string Code { get; set; }
        [MaxLength(50)]
        public string Name { get; set; }
        public List<ReceiveMsgSetting> ReceiveMsgSettings { get; set; }
    }
}
