using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using OfflineMessaging.Models;

namespace OfflineMessaging.Infrastructure
{
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
        {
            BlockedUser = new List<Block>();
            Messages = new List<Message>();
        }

        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; }
        [Required]
        [MaxLength(100)]
        public string LastName { get; set; }
        [Required]
        public byte Level { get; set; }
        [Required]
        public DateTime JoinDate { get; set; }

        public ICollection<Block> BlockedUser { get; set; }
        public ICollection<Message> Messages { get; set; }
        public ICollection<Login_Logs> Login_Logs { get; set; }


        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager, string authenticationType)
        {
            var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);
            // Add custom user claims here
            return userIdentity;
        }
    }
}