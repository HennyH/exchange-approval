﻿// <auto-generated />
using System;
using ExchangeApproval.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ExchangeApproval.Migrations
{
    [DbContext(typeof(ExchangeDbContext))]
    partial class ExchangeDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.4-rtm-31024")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("ExchangeApproval.Data.ExchangeUnit", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Code");

                    b.Property<string>("Href");

                    b.Property<string>("Title");

                    b.Property<int?>("UnitSetId");

                    b.HasKey("Id");

                    b.HasIndex("UnitSetId");

                    b.ToTable("ExchangeUnits");
                });

            modelBuilder.Entity("ExchangeApproval.Data.StudentApplication", b =>
                {
                    b.Property<int>("StudentApplicationId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ExchangeUniversityCountry");

                    b.Property<string>("ExchangeUniversityHref");

                    b.Property<string>("ExchangeUniversityName");

                    b.Property<DateTime>("LastUpdatedAt");

                    b.Property<string>("Major1st");

                    b.Property<string>("Major2nd");

                    b.Property<string>("Notes");

                    b.Property<string>("StudentName");

                    b.Property<string>("StudentNumber");

                    b.Property<DateTime>("SubmittedAt");

                    b.HasKey("StudentApplicationId");

                    b.ToTable("StudentApplications");
                });

            modelBuilder.Entity("ExchangeApproval.Data.UnitSet", b =>
                {
                    b.Property<int>("UnitSetId")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("EquivalentUWAUnitLevel");

                    b.Property<string>("ExchangeUniversityCountry");

                    b.Property<string>("ExchangeUniversityHref");

                    b.Property<string>("ExchangeUniversityName");

                    b.Property<bool?>("IsContextuallyApproved");

                    b.Property<bool?>("IsEquivalent");

                    b.Property<int?>("StudentApplicationId");

                    b.HasKey("UnitSetId");

                    b.HasIndex("StudentApplicationId");

                    b.ToTable("UnitSets");
                });

            modelBuilder.Entity("ExchangeApproval.Data.UWAStaffLogon", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("PasswordHash")
                        .IsRequired();

                    b.Property<string>("Role")
                        .IsRequired();

                    b.Property<byte[]>("Salt")
                        .IsRequired();

                    b.Property<string>("Username")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("StaffLogons");

                    b.HasData(
                        new { Id = 1, PasswordHash = "siD36VF+8Kbt7QnBd6iwNuWLZLjb0GUfgRAxLP2lNc8=", Role = "StudentOffice", Salt = new byte[] { 25, 219, 44, 17, 72, 57, 55, 150, 4, 103, 114, 160, 215, 255, 235, 232 }, Username = "admin" }
                    );
                });

            modelBuilder.Entity("ExchangeApproval.Data.UWAUnit", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Code");

                    b.Property<string>("Href");

                    b.Property<string>("Title");

                    b.Property<int?>("UnitSetId");

                    b.HasKey("Id");

                    b.HasIndex("UnitSetId");

                    b.ToTable("UWAUnits");
                });

            modelBuilder.Entity("ExchangeApproval.Data.ExchangeUnit", b =>
                {
                    b.HasOne("ExchangeApproval.Data.UnitSet", "UnitSet")
                        .WithMany("ExchangeUnits")
                        .HasForeignKey("UnitSetId");
                });

            modelBuilder.Entity("ExchangeApproval.Data.UnitSet", b =>
                {
                    b.HasOne("ExchangeApproval.Data.StudentApplication", "StudentApplication")
                        .WithMany("UnitSets")
                        .HasForeignKey("StudentApplicationId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ExchangeApproval.Data.UWAUnit", b =>
                {
                    b.HasOne("ExchangeApproval.Data.UnitSet", "UnitSet")
                        .WithMany("UWAUnits")
                        .HasForeignKey("UnitSetId");
                });
#pragma warning restore 612, 618
        }
    }
}
