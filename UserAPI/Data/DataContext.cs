using Microsoft.EntityFrameworkCore;
using UserAPI.Models;
namespace UserAPI.Data
{
    public class DataContext : DbContext
    {
        public DbSet<Users> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) 
            => optionsBuilder.UseSqlite(connectionString: "DataSource=app.db;Cache=Shared");
    }
}
