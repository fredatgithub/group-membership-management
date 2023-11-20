﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Repositories.EntityFramework.Contexts;

#nullable disable

namespace Repositories.EntityFramework.Contexts.Migrations
{
    [DbContext(typeof(GMMContext))]
    partial class GMMContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.22")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("DestinationOwnerSyncJob", b =>
                {
                    b.Property<Guid>("DestinationOwnersId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("SyncJobsId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("DestinationOwnersId", "SyncJobsId");

                    b.HasIndex("SyncJobsId");

                    b.ToTable("DestinationOwnerSyncJob");
                });

            modelBuilder.Entity("Models.DestinationName", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("LastUpdatedTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("Name");

                    b.ToTable("DestinationNames");
                });

            modelBuilder.Entity("Models.DestinationOwner", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasDefaultValueSql("NEWSEQUENTIALID()");

                    b.Property<DateTime>("LastUpdatedTime")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("ObjectId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("ObjectId");

                    b.ToTable("DestinationOwners");
                });

            modelBuilder.Entity("Models.JobNotification", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasDefaultValueSql("NEWSEQUENTIALID()");

                    b.Property<bool>("Disabled")
                        .HasColumnType("bit");

                    b.Property<int>("NotificationTypeID")
                        .HasColumnType("int");

                    b.Property<Guid>("SyncJobId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("NotificationTypeID");

                    b.HasIndex("SyncJobId", "NotificationTypeID")
                        .IsUnique();

                    b.ToTable("JobNotifications");
                });

            modelBuilder.Entity("Models.NotificationType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<bool>("Disabled")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("NotificationTypes");
                });

            modelBuilder.Entity("Models.PurgedSyncJob", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasDefaultValueSql("NEWID()");

                    b.Property<bool>("AllowEmptyDestination")
                        .HasColumnType("bit");

                    b.Property<string>("Destination")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("DryRunTimeStamp")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IgnoreThresholdOnce")
                        .HasColumnType("bit");

                    b.Property<bool>("IsDryRunEnabled")
                        .HasColumnType("bit");

                    b.Property<DateTime>("LastRunTime")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("LastSuccessfulRunTime")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("LastSuccessfulStartTime")
                        .HasColumnType("datetime2");

                    b.Property<int>("Period")
                        .HasColumnType("int");

                    b.Property<DateTime>("PurgedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Query")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Requestor")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("RunId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Status")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("TargetOfficeGroupId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("ThresholdPercentageForAdditions")
                        .HasColumnType("int");

                    b.Property<int>("ThresholdPercentageForRemovals")
                        .HasColumnType("int");

                    b.Property<int>("ThresholdViolations")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("PurgedSyncJobs");
                });

            modelBuilder.Entity("Models.Setting", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasDefaultValueSql("NEWSEQUENTIALID()");

                    b.Property<string>("Key")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("Key")
                        .IsUnique()
                        .HasFilter("[Key] IS NOT NULL");

                    b.ToTable("Settings");
                });

            modelBuilder.Entity("Models.Status", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("SortPriority")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Statuses", (string)null);
                });

            modelBuilder.Entity("Models.SyncJob", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasDefaultValueSql("NEWSEQUENTIALID()");

                    b.Property<bool>("AllowEmptyDestination")
                        .HasColumnType("bit");

                    b.Property<string>("Destination")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("DryRunTimeStamp")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IgnoreThresholdOnce")
                        .HasColumnType("bit");

                    b.Property<bool>("IsDryRunEnabled")
                        .HasColumnType("bit");

                    b.Property<DateTime>("LastRunTime")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("LastSuccessfulRunTime")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("LastSuccessfulStartTime")
                        .HasColumnType("datetime2");

                    b.Property<int>("Period")
                        .HasColumnType("int");

                    b.Property<string>("Query")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Requestor")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("RunId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Status")
                        .HasColumnType("nvarchar(450)");

                    b.Property<Guid>("TargetOfficeGroupId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("ThresholdPercentageForAdditions")
                        .HasColumnType("int");

                    b.Property<int>("ThresholdPercentageForRemovals")
                        .HasColumnType("int");

                    b.Property<int>("ThresholdViolations")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("Status")
                        .IsUnique()
                        .HasFilter("[Status] IS NOT NULL");

                    b.ToTable("SyncJobs");
                });

            modelBuilder.Entity("DestinationOwnerSyncJob", b =>
                {
                    b.HasOne("Models.DestinationOwner", null)
                        .WithMany()
                        .HasForeignKey("DestinationOwnersId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Models.SyncJob", null)
                        .WithMany()
                        .HasForeignKey("SyncJobsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Models.DestinationName", b =>
                {
                    b.HasOne("Models.SyncJob", "SyncJob")
                        .WithOne("DestinationName")
                        .HasForeignKey("Models.DestinationName", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("SyncJob");
                });

            modelBuilder.Entity("Models.JobNotification", b =>
                {
                    b.HasOne("Models.NotificationType", "NotificationType")
                        .WithMany()
                        .HasForeignKey("NotificationTypeID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Models.SyncJob", "SyncJob")
                        .WithMany()
                        .HasForeignKey("SyncJobId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("NotificationType");

                    b.Navigation("SyncJob");
                });

            modelBuilder.Entity("Models.SyncJob", b =>
                {
                    b.HasOne("Models.Status", "StatusDetails")
                        .WithOne()
                        .HasForeignKey("Models.SyncJob", "Status")
                        .HasPrincipalKey("Models.Status", "Name")
                        .OnDelete(DeleteBehavior.NoAction);

                    b.Navigation("StatusDetails");
                });

            modelBuilder.Entity("Models.SyncJob", b =>
                {
                    b.Navigation("DestinationName");
                });
#pragma warning restore 612, 618
        }
    }
}
