﻿// <auto-generated />
using System;
using Chirp.DBService;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Chirp.DBService.Migrations
{
    [DbContext(typeof(ChirpDBContext))]
    [Migration("20231011111801_InitialCreate")]
    partial class InitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "7.0.12");

            modelBuilder.Entity("Chirp.DBService.Models.Author", b =>
                {
                    b.Property<Guid>("AuthorId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("AuthorId");

                    b.ToTable("Authors");
                });

            modelBuilder.Entity("Chirp.DBService.Models.Cheep", b =>
                {
                    b.Property<Guid>("CheepId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<Guid>("AuthorId")
                        .HasColumnType("TEXT");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("TEXT");

                    b.HasKey("CheepId");

                    b.HasIndex("AuthorId");

                    b.ToTable("Cheeps");
                });

            modelBuilder.Entity("Chirp.DBService.Models.Cheep", b =>
                {
                    b.HasOne("Chirp.DBService.Models.Author", "Author")
                        .WithMany("Cheeps")
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Author");
                });

            modelBuilder.Entity("Chirp.DBService.Models.Author", b =>
                {
                    b.Navigation("Cheeps");
                });
#pragma warning restore 612, 618
        }
    }
}
