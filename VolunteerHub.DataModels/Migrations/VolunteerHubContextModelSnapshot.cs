﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using VolunteerHub.DataModels.Models;

#nullable disable

namespace VolunteerHub.DataModels.Migrations
{
    [DbContext(typeof(VolunteerHubContext))]
    partial class VolunteerHubContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.15")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("ProviderKey")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("RoleId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("LoginProvider")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("Name")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens", (string)null);
                });

            modelBuilder.Entity("VolunteerHub.DataModels.Models.Announcement", b =>
                {
                    b.Property<long>("Id")
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<long>("CreatedAt")
                        .HasColumnType("bigint");

                    b.Property<long>("ProjectId")
                        .HasColumnType("bigint");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<long>("UpdatedAt")
                        .HasColumnType("bigint");

                    b.HasKey("Id")
                        .HasName("announcement_id_primary");

                    b.HasIndex("ProjectId");

                    b.ToTable("Announcement", (string)null);
                });

            modelBuilder.Entity("VolunteerHub.DataModels.Models.Notification", b =>
                {
                    b.Property<long>("Id")
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(255)
                        .IsUnicode(false)
                        .HasColumnType("varchar(255)");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasMaxLength(255)
                        .IsUnicode(false)
                        .HasColumnType("varchar(255)");

                    b.Property<string>("Subject")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.HasKey("Id")
                        .HasName("notification_id_primary");

                    b.ToTable("Notification", (string)null);
                });

            modelBuilder.Entity("VolunteerHub.DataModels.Models.Organization", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<string>("Adress")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("Code");

                    b.Property<string>("Contact")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime");

                    b.HasKey("Id")
                        .HasName("organization_id_primary");

                    b.ToTable("Organization", (string)null);
                });

            modelBuilder.Entity("VolunteerHub.DataModels.Models.Project", b =>
                {
                    b.Property<long>("Id")
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<DateTime?>("EndDate")
                        .HasColumnType("date")
                        .HasColumnName("endDate");

                    b.Property<string>("GoalCode")
                        .IsRequired()
                        .HasMaxLength(255)
                        .IsUnicode(false)
                        .HasColumnType("varchar(255)");

                    b.Property<long>("GoalName")
                        .HasColumnType("bigint");

                    b.Property<long>("OrganizationId")
                        .HasColumnType("bigint");

                    b.Property<string>("OwnerId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint");

                    b.Property<string>("UserId1")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id")
                        .HasName("project_id_primary");

                    b.HasIndex("UserId1");

                    b.HasIndex(new[] { "GoalCode" }, "project_goalcode_unique")
                        .IsUnique();

                    b.ToTable("Project", (string)null);
                });

            modelBuilder.Entity("VolunteerHub.DataModels.Models.ProjectStat", b =>
                {
                    b.Property<long>("Id")
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime");

                    b.Property<long>("ProjectId")
                        .HasColumnType("bigint");

                    b.Property<long>("TotalTasksAsigned")
                        .HasColumnType("bigint");

                    b.Property<long>("TotalTasksCompleted")
                        .HasColumnType("bigint");

                    b.Property<long>("TotalTasksUncompleted")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime");

                    b.HasKey("Id")
                        .HasName("projectstats_id_primary");

                    b.HasIndex("ProjectId");

                    b.ToTable("ProjectStats");
                });

            modelBuilder.Entity("VolunteerHub.DataModels.Models.ProjectTask", b =>
                {
                    b.Property<long>("Id")
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    b.Property<string>("Action")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("AssigneeId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<long>("Description")
                        .HasColumnType("bigint");

                    b.Property<DateTime?>("EndDate")
                        .HasColumnType("datetime");

                    b.Property<bool>("IsTime")
                        .HasColumnType("bit");

                    b.Property<string>("MeasureUnit")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<bool>("NeedsValidation")
                        .HasColumnType("bit");

                    b.Property<long>("Progress")
                        .HasColumnType("bigint");

                    b.Property<long>("ProjectId")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("datetime");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<long?>("SuccessTreshold")
                        .HasColumnType("bigint");

                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id")
                        .HasName("projecttask_id_primary");

                    b.HasIndex("ProjectId");

                    b.HasIndex("UserId");

                    b.ToTable("ProjectTask", (string)null);
                });

            modelBuilder.Entity("VolunteerHub.DataModels.Models.User", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("bit");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<long?>("OrganizationId")
                        .HasColumnType("bigint");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.HasIndex("OrganizationId");

                    b.ToTable("AspNetUsers", (string)null);
                });

            modelBuilder.Entity("VolunteerHub.DataModels.Models.UserStat", b =>
                {
                    b.Property<long>("Id")
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    b.Property<long>("TasksAsigned")
                        .HasColumnType("bigint");

                    b.Property<long>("TasksCompleted")
                        .HasColumnType("bigint");

                    b.Property<long>("TasksUncompleted")
                        .HasColumnType("bigint");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id")
                        .HasName("userstats_id_primary");

                    b.HasIndex("UserId");

                    b.ToTable("UserStats");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("VolunteerHub.DataModels.Models.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("VolunteerHub.DataModels.Models.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("VolunteerHub.DataModels.Models.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("VolunteerHub.DataModels.Models.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("VolunteerHub.DataModels.Models.Announcement", b =>
                {
                    b.HasOne("VolunteerHub.DataModels.Models.Project", "Project")
                        .WithMany("Announcements")
                        .HasForeignKey("ProjectId")
                        .IsRequired()
                        .HasConstraintName("announcement_projectid_foreign");

                    b.Navigation("Project");
                });

            modelBuilder.Entity("VolunteerHub.DataModels.Models.Project", b =>
                {
                    b.HasOne("VolunteerHub.DataModels.Models.User", null)
                        .WithMany("Projects")
                        .HasForeignKey("UserId1");
                });

            modelBuilder.Entity("VolunteerHub.DataModels.Models.ProjectStat", b =>
                {
                    b.HasOne("VolunteerHub.DataModels.Models.Project", "Project")
                        .WithMany("ProjectStats")
                        .HasForeignKey("ProjectId")
                        .IsRequired()
                        .HasConstraintName("projectstats_projectid_foreign");

                    b.Navigation("Project");
                });

            modelBuilder.Entity("VolunteerHub.DataModels.Models.ProjectTask", b =>
                {
                    b.HasOne("VolunteerHub.DataModels.Models.Project", "Project")
                        .WithMany("ProjectTasks")
                        .HasForeignKey("ProjectId")
                        .IsRequired()
                        .HasConstraintName("projecttask_projectid_foreign");

                    b.HasOne("VolunteerHub.DataModels.Models.User", null)
                        .WithMany("ProjectTasks")
                        .HasForeignKey("UserId");

                    b.Navigation("Project");
                });

            modelBuilder.Entity("VolunteerHub.DataModels.Models.User", b =>
                {
                    b.HasOne("VolunteerHub.DataModels.Models.Organization", "Organization")
                        .WithMany("Users")
                        .HasForeignKey("OrganizationId");

                    b.Navigation("Organization");
                });

            modelBuilder.Entity("VolunteerHub.DataModels.Models.UserStat", b =>
                {
                    b.HasOne("VolunteerHub.DataModels.Models.User", null)
                        .WithMany("UserStats")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("VolunteerHub.DataModels.Models.Organization", b =>
                {
                    b.Navigation("Users");
                });

            modelBuilder.Entity("VolunteerHub.DataModels.Models.Project", b =>
                {
                    b.Navigation("Announcements");

                    b.Navigation("ProjectStats");

                    b.Navigation("ProjectTasks");
                });

            modelBuilder.Entity("VolunteerHub.DataModels.Models.User", b =>
                {
                    b.Navigation("ProjectTasks");

                    b.Navigation("Projects");

                    b.Navigation("UserStats");
                });
#pragma warning restore 612, 618
        }
    }
}
