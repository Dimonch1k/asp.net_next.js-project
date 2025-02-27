﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using backend_c_;

#nullable disable

namespace backend_c_.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20250118191728_updated_mediaFile")]
    partial class updated_mediaFile
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("backend_c_.Entity.AccessLog", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("AccessTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("AccessType")
                        .HasColumnType("integer");

                    b.Property<int>("FileId")
                        .HasColumnType("integer");

                    b.Property<int?>("MediaFileId")
                        .HasColumnType("integer");

                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.Property<int?>("UserId1")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("FileId");

                    b.HasIndex("MediaFileId");

                    b.HasIndex("UserId");

                    b.HasIndex("UserId1");

                    b.ToTable("AccessLogs");
                });

            modelBuilder.Entity("backend_c_.Entity.FileVersion", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("FileId")
                        .HasColumnType("integer");

                    b.Property<int?>("MediaFileId")
                        .HasColumnType("integer");

                    b.Property<string>("VersionName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("VersionPath")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("FileId");

                    b.HasIndex("MediaFileId");

                    b.ToTable("FileVersions");
                });

            modelBuilder.Entity("backend_c_.Entity.MediaFile", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("FileName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("FilePath")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("FileSize")
                        .HasColumnType("integer");

                    b.Property<string>("FileType")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.Property<int?>("UserId1")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.HasIndex("UserId1");

                    b.ToTable("Files");
                });

            modelBuilder.Entity("backend_c_.Entity.SharedFile", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("FileId")
                        .HasColumnType("integer");

                    b.Property<int?>("MediaFileId")
                        .HasColumnType("integer");

                    b.Property<int>("OwnerId")
                        .HasColumnType("integer");

                    b.Property<int>("SharedWithId")
                        .HasColumnType("integer");

                    b.Property<int?>("UserId")
                        .HasColumnType("integer");

                    b.Property<int?>("UserId1")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("FileId");

                    b.HasIndex("MediaFileId");

                    b.HasIndex("OwnerId");

                    b.HasIndex("SharedWithId");

                    b.HasIndex("UserId");

                    b.HasIndex("UserId1");

                    b.ToTable("SharedFiles");
                });

            modelBuilder.Entity("backend_c_.Entity.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("FullName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("TimeZoneId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("backend_c_.Entity.AccessLog", b =>
                {
                    b.HasOne("backend_c_.Entity.MediaFile", "File")
                        .WithMany()
                        .HasForeignKey("FileId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("backend_c_.Entity.MediaFile", null)
                        .WithMany("AccessLogs")
                        .HasForeignKey("MediaFileId");

                    b.HasOne("backend_c_.Entity.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("backend_c_.Entity.User", null)
                        .WithMany("AccessLogs")
                        .HasForeignKey("UserId1");

                    b.Navigation("File");

                    b.Navigation("User");
                });

            modelBuilder.Entity("backend_c_.Entity.FileVersion", b =>
                {
                    b.HasOne("backend_c_.Entity.MediaFile", "File")
                        .WithMany()
                        .HasForeignKey("FileId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("backend_c_.Entity.MediaFile", null)
                        .WithMany("Versions")
                        .HasForeignKey("MediaFileId");

                    b.Navigation("File");
                });

            modelBuilder.Entity("backend_c_.Entity.MediaFile", b =>
                {
                    b.HasOne("backend_c_.Entity.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("backend_c_.Entity.User", null)
                        .WithMany("Files")
                        .HasForeignKey("UserId1");

                    b.Navigation("User");
                });

            modelBuilder.Entity("backend_c_.Entity.SharedFile", b =>
                {
                    b.HasOne("backend_c_.Entity.MediaFile", "File")
                        .WithMany()
                        .HasForeignKey("FileId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("backend_c_.Entity.MediaFile", null)
                        .WithMany("SharedFiles")
                        .HasForeignKey("MediaFileId");

                    b.HasOne("backend_c_.Entity.User", "Owner")
                        .WithMany()
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("backend_c_.Entity.User", "SharedWith")
                        .WithMany()
                        .HasForeignKey("SharedWithId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("backend_c_.Entity.User", null)
                        .WithMany("OwnedFiles")
                        .HasForeignKey("UserId");

                    b.HasOne("backend_c_.Entity.User", null)
                        .WithMany("SharedWithFiles")
                        .HasForeignKey("UserId1");

                    b.Navigation("File");

                    b.Navigation("Owner");

                    b.Navigation("SharedWith");
                });

            modelBuilder.Entity("backend_c_.Entity.MediaFile", b =>
                {
                    b.Navigation("AccessLogs");

                    b.Navigation("SharedFiles");

                    b.Navigation("Versions");
                });

            modelBuilder.Entity("backend_c_.Entity.User", b =>
                {
                    b.Navigation("AccessLogs");

                    b.Navigation("Files");

                    b.Navigation("OwnedFiles");

                    b.Navigation("SharedWithFiles");
                });
#pragma warning restore 612, 618
        }
    }
}
