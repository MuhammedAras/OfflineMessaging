﻿
using OfflineMessaging.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;


namespace OfflineMessaging.Infrastructure
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
            Configuration.ProxyCreationEnabled = false;
            Configuration.LazyLoadingEnabled = false;
        }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Block> Blocks { get; set; }
        public DbSet<Login_Logs> Login_Logs { get; set; }


        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

    }
}