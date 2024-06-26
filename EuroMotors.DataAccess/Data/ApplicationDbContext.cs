﻿using EuroMotors.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EuroMotors.DataAccess.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
		{

		}

		public DbSet<Category> Categories { get; set; }
		public DbSet<CarModel> CarModels { get; set; }
		public DbSet<Product> Products { get; set; }
		public DbSet<ShoppingCart> ShoppingCarts { get; set; }
		public DbSet<ProductImage> ProductImages { get; set; }
		public DbSet<ApplicationUser> ApplicationUsers { get; set; }
		public DbSet<OrderHeader> OrderHeaders { get; set; }
		public DbSet<OrderDetail> OrderDetails { get; set; }


		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{

			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<Category>().HasData(
				new Category { Id = 1, Name = "Запчастини для Т/О", DisplayOrder = 1 },
				new Category { Id = 2, Name = "Шини", DisplayOrder = 2 },
				new Category { Id = 3, Name = "Диски", DisplayOrder = 3 },
				new Category { Id = 4, Name = "Інструменти", DisplayOrder = 4 }
				);
			modelBuilder.Entity<CarModel>().HasData(
				new CarModel { Id = 1, Brand = "Toyota", Model = "Carolla", Year = 2018, DisplayOrder = 1 },
				new CarModel { Id = 2, Brand = "Honda", Model = "Civic", Year = 2020, DisplayOrder = 2 },
				new CarModel { Id = 3, Brand = "Chevrolet", Model = "Camaro", Year = 2015, DisplayOrder = 3 },
				new CarModel { Id = 4, Brand = "Audi", Model = "Q7", Year = 2005, DisplayOrder = 4 }
				);
			modelBuilder.Entity<Product>().HasData(
				new Product
				{
					Id = 1,
					Title = "Оливний фільтр",
					Desctiption = "Lorem Ipsum is simply dummy text of the printing and typesetting industry." +
					" Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took" +
					" a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries," +
					" but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s" +
					" with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing" +
					" software like Aldus PageMaker including versions of Lorem Ipsum.Lorem.",
					VendorCode = "04D11S3AB9",
					Brand = "Bosch",
					ListPrice = 148,
					Price = 140,
					CategoryId = 1,
					CarModelId = 1,
				},
				new Product
				{
					Id = 2,
					Title = "Гальмівні колодки",
					Desctiption = "Lorem Ipsum is simply dummy text of the printing and typesetting industry." +
					" Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took" +
					" a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries,",
					VendorCode = "C2W001ABE",
					Brand = "ABE",
					ListPrice = 614,
					Price = 600,
					CategoryId = 2,
					CarModelId = 2,
				},
				new Product
				{
					Id = 3,
					Title = "Акумулятор 6 CT-77-R S5 Silver Plus",
					Desctiption = "Lorem Ipsum is simply dummy text of the printing and typesetting industry." +
					" Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took" +
					" a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries," +
					" a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries,",
					VendorCode = "0092S50080",
					Brand = "Bosch",
					ListPrice = 4147,
					Price = 4147,
					CategoryId = 3,
					CarModelId = 3,
				},
                new Product
                {
                    Id = 4,
                    Title = "Моторна олива MANNOL ELITE SAE 5W-40 4л (MN7903-4)",
                    Desctiption = "Lorem Ipsum is simply dummy text of the printing and typesetting industry." +
                    " Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took",
                    VendorCode = "1287435",
                    Brand = "Bosch",
                    ListPrice = 1699,
                    Price = 1699,
                    CategoryId = 4,
                    CarModelId = 4,
                }
                );
		}
	}
}
