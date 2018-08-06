using OfflineMessaging.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace OfflineMessaging.Models
{
    public class Block
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public string BlockedUserId { get; set; }
        public DateTime BlockedTime { get; set; }
        public string userId  { get; set; }
        public virtual ApplicationUser BlockedUser { get; set; }
    }
}