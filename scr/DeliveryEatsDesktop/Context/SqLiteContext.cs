using desktop.Models;
using Microsoft.EntityFrameworkCore;

namespace desktop.Context
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
            optionsBuilder.UseSqlite("FileName=storage.db");
        }
    }
}
