﻿// <auto-generated />
using System;
using EuroMotors.DataAccess.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace EuroMotors.DataAccess.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("EuroMotors.Models.CarModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Brand")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)");

                    b.Property<int?>("DisplayOrder")
                        .IsRequired()
                        .HasColumnType("int");

                    b.Property<string>("Model")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)");

                    b.Property<int?>("Year")
                        .IsRequired()
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("CarModels");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Brand = "Toyota",
                            DisplayOrder = 1,
                            Model = "Carolla",
                            Year = 2018
                        },
                        new
                        {
                            Id = 2,
                            Brand = "Honda",
                            DisplayOrder = 2,
                            Model = "Civic",
                            Year = 2020
                        },
                        new
                        {
                            Id = 3,
                            Brand = "Chevrolet",
                            DisplayOrder = 3,
                            Model = "Camaro",
                            Year = 2015
                        });
                });

            modelBuilder.Entity("EuroMotors.Models.Category", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("DisplayOrder")
                        .IsRequired()
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)");

                    b.HasKey("Id");

                    b.ToTable("Categories");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            DisplayOrder = 1,
                            Name = "Запчастини для Т/О"
                        },
                        new
                        {
                            Id = 2,
                            DisplayOrder = 2,
                            Name = "Шини"
                        },
                        new
                        {
                            Id = 3,
                            DisplayOrder = 3,
                            Name = "Диски"
                        });
                });

            modelBuilder.Entity("EuroMotors.Models.Product", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Brand")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)");

                    b.Property<int?>("CarModelId")
                        .HasColumnType("int");

                    b.Property<int>("CategoryId")
                        .HasColumnType("int");

                    b.Property<string>("Desctiption")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ImageUrl")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("ListPrice")
                        .HasColumnType("float");

                    b.Property<double>("Price")
                        .HasColumnType("float");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("VendorCode")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("CarModelId");

                    b.HasIndex("CategoryId");

                    b.ToTable("Products");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Brand = "Bosch",
                            CarModelId = 1,
                            CategoryId = 1,
                            Desctiption = "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.Lorem.",
                            ImageUrl = "",
                            ListPrice = 148.0,
                            Price = 140.0,
                            Title = "Оливний фільтр",
                            VendorCode = "04D11S3AB9"
                        },
                        new
                        {
                            Id = 2,
                            Brand = "ABE",
                            CarModelId = 2,
                            CategoryId = 2,
                            Desctiption = "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries,",
                            ImageUrl = "",
                            ListPrice = 614.0,
                            Price = 600.0,
                            Title = "Гальмівні колодки",
                            VendorCode = "C2W001ABE"
                        },
                        new
                        {
                            Id = 3,
                            Brand = "Bosch",
                            CarModelId = 3,
                            CategoryId = 3,
                            Desctiption = "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries,",
                            ImageUrl = "",
                            ListPrice = 4147.0,
                            Price = 4147.0,
                            Title = "Акумулятор 6 CT-77-R S5 Silver Plus",
                            VendorCode = "0092S50080"
                        },
                        new
                        {
                            Id = 4,
                            Brand = "Bosch",
                            CategoryId = 3,
                            Desctiption = "Lorem Ipsum is simply dummy text of the printing and ty +pesetting industry.",
                            ImageUrl = "",
                            ListPrice = 507.0,
                            Price = 500.0,
                            Title = "Свічка розжарювання",
                            VendorCode = "159Rer0080"
                        });
                });

            modelBuilder.Entity("EuroMotors.Models.Product", b =>
                {
                    b.HasOne("EuroMotors.Models.CarModel", "CarModel")
                        .WithMany()
                        .HasForeignKey("CarModelId");

                    b.HasOne("EuroMotors.Models.Category", "Category")
                        .WithMany()
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("CarModel");

                    b.Navigation("Category");
                });
#pragma warning restore 612, 618
        }
    }
}
