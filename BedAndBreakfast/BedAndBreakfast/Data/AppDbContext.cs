using BedAndBreakfast.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BedAndBreakfast.Data
{
    public class AppDbContext : IdentityDbContext<User>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

        // Tables
        public DbSet<User> Users { get; set; }
        public DbSet<Profile> Profiles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {

            // This method must be called to map keys for user identity tables.
            base.OnModelCreating(modelBuilder);

            // Relationship definition
            //modelBuilder.Entity<User>()
            //    .HasOne(u => u.Profile)
            //    .WithOne(p => p.User);

            modelBuilder.Entity<Profile>()
                .HasOne(p => p.User)
                .WithOne(u => u.Profile)
                .HasForeignKey<User>(u => u.ProfileFK);
        }

    }
}
