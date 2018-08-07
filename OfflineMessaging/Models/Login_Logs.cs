using OfflineMessaging.Infrastructure;
using System;
using System.ComponentModel.DataAnnotations.Schema;


namespace OfflineMessaging.Models
{
    public class Login_Logs
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public String UserId { get; set; }
        public string Status { get; set; }
        public string Ip_Address  { get; set; }
        public DateTime Time { get; set; }

        public virtual ApplicationUser User { get; set; }

    }
}