﻿using App.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace App.Data
{
    public class DataContext : DbContext
    {
        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<Detail> Details { get; set; }
        public DbSet<DetailCategory> DetailCategories { get; set; }
        public DbSet<DetailWeight> DetailWeights { get; set; }
        public DbSet<Home> Homes { get; set; }
        public DbSet<HomeDetail> HomeDetails { get; set; }

        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            
            builder.Entity<Detail>()
                .HasOne(r => r.Category)
                .WithMany(r => r.Details)
                .IsRequired();

            builder.Entity<HomeDetail>()
                .HasOne(r => r.Home)
                .WithMany(r => r.Details)
                .IsRequired();
        }
    }
}
