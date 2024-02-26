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
        public DbSet<CarModel> CarModel { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Запчастини для Т/О", DisplayOrder = 1 },
                new Category { Id = 2, Name = "Шини", DisplayOrder = 2 },
                new Category { Id = 3, Name = "Диски", DisplayOrder = 3 }
                );
			modelBuilder.Entity<CarModel>().HasData(
				new CarModel { Id = 1, Brand = "Toyota", Model = "Carolla", Year = 2018, DisplayOrder = 1 },
				new CarModel { Id = 2, Brand = "Honda", Model = "Civic", Year = 2020, DisplayOrder = 2 },
				new CarModel { Id = 3, Brand = "Chevrolet", Model = "Camaro", Year = 2015, DisplayOrder = 3 }
				);
		}
    }
}
