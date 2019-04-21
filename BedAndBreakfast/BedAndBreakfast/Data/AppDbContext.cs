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
        public DbSet<NotificationsSetting> NotificationSettings { get; set; }
        public DbSet<PrivacySetting> PrivacySettings { get; set; }
		public DbSet<Address> Addresses { get; set; }


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

            // One user has one message settings.
            modelBuilder.Entity<User>()
                .HasOne(u => u.NotificationsSetting)
                .WithOne(s => s.User)
                .HasForeignKey<NotificationsSetting>(s => s.UserFK);

            // One privacy setting has one user.
            modelBuilder.Entity<PrivacySetting>()
                .HasOne(s => s.User)
                .WithOne(u => u.PrivacySetting)
                .HasForeignKey<PrivacySetting>(s => s.UserFK);

			// Each user profile has one address.
			modelBuilder.Entity<Profile>()
				.HasOne(p => p.Address)
				.WithOne(a => a.Profile)
				.HasForeignKey<Profile>(p => p.AddressFK);
			

        }

    }
}
