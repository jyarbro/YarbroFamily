using App.Data.Models;
using Microsoft.EntityFrameworkCore;
using Nrrdio.Utilities.Loggers;

namespace App.Data {
    public class DataContext : DbContext {
        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<HomeReviewFeature> HomeReviewFeatures { get; set; }
        public DbSet<HomeReviewFeatureLevel> HomeReviewFeatureLevels { get; set; }
        public DbSet<HomeReviewFeatureCategory> HomeReviewFeatureCategories { get; set; }
        public DbSet<HomeReviewUserWeight> HomeReviewUserWeights { get; set; }
        public DbSet<HomeReviewHome> HomeReviewHomes { get; set; }
        public DbSet<HomeReviewHomeFeature> HomeReviewHomeFeatures { get; set; }
        public DbSet<HomeReviewHomeFeatureLevel> HomeReviewHomeFeatureLevels { get; set; }
        public DbSet<HomeReviewLink> HomeReviewLinks { get; set; }
        public DbSet<HomeReviewBaseScoreModifier> HomeReviewBaseScoreModifiers { get; set; }
        public DbSet<LogEntry> YnabFeederLog { get; set; }

        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder) {
            base.OnModelCreating(builder);

            builder.Entity<HomeReviewFeature>()
                .HasOne(o => o.Category)
                .WithMany(o => o.Details)
                .IsRequired();

            builder.Entity<HomeReviewFeatureLevel>()
                .HasOne(o => o.Preference)
                .WithMany(o => o.Levels);

            builder.Entity<HomeReviewHomeFeature>()
                .HasOne(o => o.Home)
                .WithMany(o => o.Details)
                .IsRequired();

            builder.Entity<HomeReviewHomeFeature>()
                .HasOne(o => o.Detail)
                .WithMany(o => o.Details)
                .IsRequired();

            builder.Entity<HomeReviewHomeFeatureLevel>()
                .HasOne(o => o.Home)
                .WithMany(o => o.PreferenceLevels)
                .IsRequired();

            builder.Entity<HomeReviewHomeFeatureLevel>()
                .HasOne(o => o.Preference)
                .WithMany(o => o.HomePreferenceLevels)
                .IsRequired();

            builder.Entity<HomeReviewHomeFeatureLevel>()
                .HasOne(o => o.PreferenceLevel)
                .WithMany(o => o.HomePreferenceLevels)
                .IsRequired();

            builder.Entity<HomeReviewLink>()
                .HasOne(o => o.Home)
                .WithMany(o => o.Links)
                .IsRequired();

            builder.Entity<HomeReviewBaseScoreModifier>()
                .HasIndex(o => o.Type);

            builder.Entity<HomeReviewUserWeight>()
                .HasOne(o => o.Detail)
                .WithMany(o => o.Weights);

            builder.Entity<HomeReviewUserWeight>()
                .HasOne(o => o.Level)
                .WithMany(o => o.Weights);

            builder.Entity<HomeReviewUserWeight>()
                .HasOne(o => o.User)
                .WithMany(o => o.Preferences)
                .IsRequired();

            builder.Entity<HomeReviewUserWeight>()
                .HasOne(o => o.CreatedBy);
        }
    }
}
