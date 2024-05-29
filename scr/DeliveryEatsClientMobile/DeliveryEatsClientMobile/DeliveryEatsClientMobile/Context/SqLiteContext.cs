using System;
using DeliveryEatsClientMobile.Models;
using Microsoft.EntityFrameworkCore;

namespace DeliveryEatsClientMobile.Context
{
    public class SqLiteContext : DbContext
    {
        public DbSet<RefreshToken> refreshTokens { get; set; }

        public SqLiteContext()
        {
            Database.EnsureCreated();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var storagePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)+"/storage.db";
            optionsBuilder.UseSqlite($"Data Source={storagePath}");
        }
    }
}
