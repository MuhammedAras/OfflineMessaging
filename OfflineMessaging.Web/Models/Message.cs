using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OfflineMessaging.Web.Models
{
    public class Message
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime Time { get; set; }
        public string SenderId { get; set; }
        public string ReceiverId { get; set; }
    }    
}