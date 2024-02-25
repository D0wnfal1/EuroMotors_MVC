using EuroMotors.Models;
using Microsoft.EntityFrameworkCore;

namespace EuroMotors.DataAccess.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Запчастини для Т/О", DisplayOrder = 1 },
                new Category { Id = 2, Name = "Шини", DisplayOrder = 2 },
                new Category { Id = 3, Name = "Диски", DisplayOrder = 3 }
                );
        }
    }
}
