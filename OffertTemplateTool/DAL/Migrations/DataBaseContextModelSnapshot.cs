﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using OffertTemplateTool.DAL.Context;
using System;

namespace OffertTemplateTool.DAL.Migrations
{
    [DbContext(typeof(DataBaseContext))]
    partial class DataBaseContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.0-rtm-26452")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("OffertTemplateTool.DAL.Models.EstimateConnects", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid?>("EstimateId");

                    b.Property<Guid?>("EstimateLinesId");

                    b.HasKey("Id");

                    b.HasIndex("EstimateId");

                    b.HasIndex("EstimateLinesId");

                    b.ToTable("EstimateConnects");
                });

            modelBuilder.Entity("OffertTemplateTool.DAL.Models.EstimateLines", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<float>("HourCost");

                    b.Property<double>("Hours");

                    b.Property<string>("Specification")
                        .IsRequired();

                    b.Property<float>("TotalCost");

                    b.Property<int>("WefactIdentifier");

                    b.HasKey("Id");

                    b.ToTable("EstimateLines");
                });

            modelBuilder.Entity("OffertTemplateTool.DAL.Models.Estimates", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.HasKey("Id");

                    b.ToTable("Estimates");
                });

            modelBuilder.Entity("OffertTemplateTool.DAL.Models.Offers", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime?>("CreatedAt");

                    b.Property<Guid?>("CreatedById");

                    b.Property<string>("DebtorNumber");

                    b.Property<int>("DocumentCode");

                    b.Property<Guid?>("EstimateId");

                    b.Property<string>("IndexContent");

                    b.Property<int>("IsOpen");

                    b.Property<DateTime?>("LastUpdatedAt");

                    b.Property<string>("ProjectName");

                    b.Property<Guid?>("UpdatedById");

                    b.HasKey("Id");

                    b.HasIndex("CreatedById");

                    b.HasIndex("EstimateId");

                    b.HasIndex("UpdatedById");

                    b.ToTable("Offer");
                });

            modelBuilder.Entity("OffertTemplateTool.DAL.Models.Settings", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Key");

                    b.Property<string>("Value");

                    b.HasKey("Id");

                    b.ToTable("Settings");
                });

            modelBuilder.Entity("OffertTemplateTool.DAL.Models.Users", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Email");

                    b.Property<string>("FirstName");

                    b.Property<string>("Function");

                    b.Property<string>("Initials");

                    b.Property<string>("Insertion");

                    b.Property<string>("LastName");

                    b.Property<int>("PhoneNumber");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("OffertTemplateTool.DAL.Models.EstimateConnects", b =>
                {
                    b.HasOne("OffertTemplateTool.DAL.Models.Estimates", "Estimate")
                        .WithMany()
                        .HasForeignKey("EstimateId");

                    b.HasOne("OffertTemplateTool.DAL.Models.EstimateLines", "EstimateLines")
                        .WithMany()
                        .HasForeignKey("EstimateLinesId");
                });

            modelBuilder.Entity("OffertTemplateTool.DAL.Models.Offers", b =>
                {
                    b.HasOne("OffertTemplateTool.DAL.Models.Users", "CreatedBy")
                        .WithMany()
                        .HasForeignKey("CreatedById");

                    b.HasOne("OffertTemplateTool.DAL.Models.Estimates", "Estimate")
                        .WithMany()
                        .HasForeignKey("EstimateId");

                    b.HasOne("OffertTemplateTool.DAL.Models.Users", "UpdatedBy")
                        .WithMany()
                        .HasForeignKey("UpdatedById");
                });
#pragma warning restore 612, 618
        }
    }
}
