using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OfflineMessaging.Models
{
    public class MessageBindingModel
    {
        
            [Required]
            [Display(Name = "Receiver UserName")]
            public string ReceiverUserName { get; set; }

            [Required]
            [Display(Name = "Message")]
            public string Message { get; set; }
        
    }
}