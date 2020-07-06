﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Yourthome.Data;

namespace Yourthome.Migrations
{
    [DbContext(typeof(YourthomeContext))]
    [Migration("20200701151638_remotepostgres")]
    partial class remotepostgres
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .HasAnnotation("ProductVersion", "3.1.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("Yourthome.Models.Photo", b =>
                {
                    b.Property<int>("PhotoID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<byte[]>("Image")
                        .HasColumnType("bytea");

                    b.Property<int>("RentalID")
                        .HasColumnType("integer");

                    b.HasKey("PhotoID");

                    b.HasIndex("RentalID");

                    b.ToTable("Photo");
                });

            modelBuilder.Entity("Yourthome.Models.Rental", b =>
                {
                    b.Property<int>("RentalID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int>("Cost")
                        .HasColumnType("integer");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<string>("Furniture")
                        .HasColumnType("text");

                    b.Property<decimal>("Latitude")
                        .HasColumnType("numeric");

                    b.Property<decimal>("Longitude")
                        .HasColumnType("numeric");

                    b.Property<string>("Nearby")
                        .HasColumnType("text");

                    b.Property<int>("PropertyType")
                        .HasColumnType("integer");

                    b.Property<int>("Region")
                        .HasColumnType("integer");

                    b.Property<int>("RentTime")
                        .HasColumnType("integer");

                    b.Property<int>("Rooms")
                        .HasColumnType("integer");

                    b.Property<string>("Street")
                        .HasColumnType("text");

                    b.HasKey("RentalID");

                    b.ToTable("Rental");
                });

            modelBuilder.Entity("Yourthome.Models.Photo", b =>
                {
                    b.HasOne("Yourthome.Models.Rental", "Rental")
                        .WithMany("Photos")
                        .HasForeignKey("RentalID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}