using App.Data.Models;
using Microsoft.EntityFrameworkCore;
using Nrrdio.Utilities.Loggers;

namespace App.Data {
    public class DataContext : DbContext {
        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<HomeReviewPreference> HomeReviewDetails { get; set; }
        public DbSet<HomeReviewPreferenceLevel> HomeReviewPreferenceLevels { get; set; }
        public DbSet<HomeReviewPreferenceCategory> HomeReviewDetailCategories { get; set; }
        public DbSet<HomeReviewUserPreference> HomeReviewUserPreferences { get; set; }
        public DbSet<HomeReviewHome> HomeReviewHomes { get; set; }
        public DbSet<HomeReviewHomePreference> HomeReviewHomeDetails { get; set; }
        public DbSet<HomeReviewHomePreferenceLevel> HomeReviewHomePreferenceLevels { get; set; }
        public DbSet<HomeReviewLink> HomeReviewLinks { get; set; }
        public DbSet<HomeReviewScoreModifier> HomeReviewScoreModifiers { get; set; }
        public DbSet<LogEntry> YnabFeederLog { get; set; }

        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder) {
            base.OnModelCreating(builder);

            builder.Entity<HomeReviewPreference>()
                .HasOne(o => o.Category)
                .WithMany(o => o.Details)
                .IsRequired();

            builder.Entity<HomeReviewPreferenceLevel>()
                .HasOne(o => o.Preference)
                .WithMany(o => o.Levels);

            builder.Entity<HomeReviewHomePreference>()
                .HasOne(o => o.Home)
                .WithMany(o => o.Details)
                .IsRequired();

            builder.Entity<HomeReviewHomePreference>()
                .HasOne(o => o.Detail)
                .WithMany(o => o.Details)
                .IsRequired();

            builder.Entity<HomeReviewHomePreferenceLevel>()
                .HasOne(o => o.Home)
                .WithMany(o => o.PreferenceLevels)
                .IsRequired();

            builder.Entity<HomeReviewHomePreferenceLevel>()
                .HasOne(o => o.Preference)
                .WithMany(o => o.HomePreferenceLevels)
                .IsRequired();

            builder.Entity<HomeReviewHomePreferenceLevel>()
                .HasOne(o => o.PreferenceLevel)
                .WithMany(o => o.HomePreferenceLevels)
                .IsRequired();

            builder.Entity<HomeReviewLink>()
                .HasOne(o => o.Home)
                .WithMany(o => o.Links)
                .IsRequired();

            builder.Entity<HomeReviewScoreModifier>()
                .HasIndex(o => o.Type);

            builder.Entity<HomeReviewUserPreference>()
                .HasOne(o => o.Detail)
                .WithMany(o => o.Weights);

            builder.Entity<HomeReviewUserPreference>()
                .HasOne(o => o.Level)
                .WithMany(o => o.Weights);

            builder.Entity<HomeReviewUserPreference>()
                .HasOne(o => o.User)
                .WithMany(o => o.Preferences)
                .IsRequired();

            builder.Entity<HomeReviewUserPreference>()
                .HasOne(o => o.CreatedBy);
        }
    }
}
