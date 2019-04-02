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
        public DbSet<HelpPage> HelpPages { get; set; }
        public DbSet<HelpTag> HelpTags { get; set; }
        public DbSet<HelpPageHelpTag> HelpPageHelpTags { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {

            // This method must be called to map keys for user identity tables.
            base.OnModelCreating(modelBuilder);

            // One user has one profile.
            modelBuilder.Entity<Profile>()
                .HasOne(p => p.User)
                .WithOne(u => u.Profile)
                .HasForeignKey<User>(u => u.ProfileFK);


            // May help pages has many tags.
            modelBuilder.Entity<HelpPageHelpTag>()
                .HasKey(t => new { t.HelpPageID, t.HelpTagID });

            modelBuilder.Entity<HelpPageHelpTag>()
                .HasOne(hpht => hpht.HelpTag)
                .WithMany(ht => ht.HelpPageHelpTag)
                .HasForeignKey(hpht => hpht.HelpTagID);

            modelBuilder.Entity<HelpPageHelpTag>()
                .HasOne(hpht => hpht.HelpPage)
                .WithMany(hp => hp.HelpPageHelpTag)
                .HasForeignKey(hpht => hpht.HelpPageID);

        }

    }
}
