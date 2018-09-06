using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OfflineMessaging.Web.Models
{
    public class MessagedUser
    {
        public string UserName { get; set; }
        public string FullName { get; set; }
        public DateTime Time { get; set; }
    }
}