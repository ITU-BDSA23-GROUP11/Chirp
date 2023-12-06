﻿// <auto-generated />
using System;
using Chirp.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Chirp.Infrastructure.Migrations
{
    [DbContext(typeof(ChirpDbContext))]
    [Migration("20231205193612_AddLikes")]
    partial class AddLikes
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.13")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("AuthorAuthor", b =>
                {
                    b.Property<Guid>("FollowedByAuthorId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("FollowsAuthorId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("FollowedByAuthorId", "FollowsAuthorId");

                    b.HasIndex("FollowsAuthorId");

                    b.ToTable("AuthorAuthor");
                });

            modelBuilder.Entity("Chirp.Infrastructure.Models.Author", b =>
                {
                    b.Property<Guid>("AuthorId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("AuthorId");

                    b.ToTable("Authors");
                });

            modelBuilder.Entity("Chirp.Infrastructure.Models.Cheep", b =>
                {
                    b.Property<Guid>("CheepId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("AuthorId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasMaxLength(160)
                        .HasColumnType("nvarchar(160)");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("datetime2");

                    b.HasKey("CheepId");

                    b.HasIndex("AuthorId");

                    b.ToTable("Cheeps");
                });

            modelBuilder.Entity("Chirp.Infrastructure.Models.Like", b =>
                {
                    b.Property<Guid>("LikedByAuthorId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("CheepId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("LikedByAuthorId", "CheepId");

                    b.ToTable("Likes");
                });

            modelBuilder.Entity("AuthorAuthor", b =>
                {
                    b.HasOne("Chirp.Infrastructure.Models.Author", null)
                        .WithMany()
                        .HasForeignKey("FollowedByAuthorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Chirp.Infrastructure.Models.Author", null)
                        .WithMany()
                        .HasForeignKey("FollowsAuthorId")
                        .OnDelete(DeleteBehavior.ClientCascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Chirp.Infrastructure.Models.Cheep", b =>
                {
                    b.HasOne("Chirp.Infrastructure.Models.Author", "Author")
                        .WithMany("Cheeps")
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Author");
                });

            modelBuilder.Entity("Chirp.Infrastructure.Models.Author", b =>
                {
                    b.Navigation("Cheeps");
                });
#pragma warning restore 612, 618
        }
    }
}
