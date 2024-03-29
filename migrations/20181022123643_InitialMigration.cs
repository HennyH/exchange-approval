﻿using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ExchangeApproval.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StaffLogons",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Username = table.Column<string>(nullable: false),
                    Role = table.Column<string>(nullable: false),
                    Salt = table.Column<byte[]>(nullable: false),
                    PasswordHash = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StaffLogons", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StudentApplications",
                columns: table => new
                {
                    StudentApplicationId = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    SubmittedAt = table.Column<DateTime>(nullable: false),
                    LastUpdatedAt = table.Column<DateTime>(nullable: false),
                    CompletedAt = table.Column<DateTime>(nullable: false),
                    StudentName = table.Column<string>(nullable: true),
                    StudentNumber = table.Column<string>(nullable: true),
                    ExchangeDate = table.Column<DateTime>(nullable: false),
                    Major1st = table.Column<string>(nullable: true),
                    Major2nd = table.Column<string>(nullable: true),
                    ExchangeUniversityCountry = table.Column<string>(nullable: true),
                    ExchangeUniversityHref = table.Column<string>(nullable: true),
                    ExchangeUniversityName = table.Column<string>(nullable: true),
                    Notes = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentApplications", x => x.StudentApplicationId);
                });

            migrationBuilder.CreateTable(
                name: "UnitSets",
                columns: table => new
                {
                    UnitSetId = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    StudentApplicationId = table.Column<int>(nullable: true),
                    ExchangeUniversityCountry = table.Column<string>(nullable: true),
                    ExchangeUniversityHref = table.Column<string>(nullable: true),
                    ExchangeUniversityName = table.Column<string>(nullable: true),
                    SubmittedAt = table.Column<DateTime>(nullable: false),
                    LastUpdatedAt = table.Column<DateTime>(nullable: false),
                    CompletedAt = table.Column<DateTime>(nullable: false),
                    IsEquivalent = table.Column<bool>(nullable: true),
                    IsContextuallyApproved = table.Column<bool>(nullable: true),
                    EquivalentUWAUnitLevel = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnitSets", x => x.UnitSetId);
                    table.ForeignKey(
                        name: "FK_UnitSets_StudentApplications_StudentApplicationId",
                        column: x => x.StudentApplicationId,
                        principalTable: "StudentApplications",
                        principalColumn: "StudentApplicationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExchangeUnits",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(nullable: true),
                    Title = table.Column<string>(nullable: true),
                    Href = table.Column<string>(nullable: true),
                    UnitSetId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExchangeUnits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExchangeUnits_UnitSets_UnitSetId",
                        column: x => x.UnitSetId,
                        principalTable: "UnitSets",
                        principalColumn: "UnitSetId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UWAUnits",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(nullable: true),
                    Title = table.Column<string>(nullable: true),
                    Href = table.Column<string>(nullable: true),
                    UnitSetId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UWAUnits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UWAUnits_UnitSets_UnitSetId",
                        column: x => x.UnitSetId,
                        principalTable: "UnitSets",
                        principalColumn: "UnitSetId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "StaffLogons",
                columns: new[] { "Id", "PasswordHash", "Role", "Salt", "Username" },
                values: new object[] { 1, "InAdOAPwcGKRUuWmechPgSW7oKTL9rdL7YwnZWl8HP0=", "StudentOffice", new byte[] { 228, 229, 226, 199, 166, 209, 206, 69, 214, 2, 152, 38, 135, 211, 129, 129 }, "admin" });

            migrationBuilder.CreateIndex(
                name: "IX_ExchangeUnits_UnitSetId",
                table: "ExchangeUnits",
                column: "UnitSetId");

            migrationBuilder.CreateIndex(
                name: "IX_UnitSets_StudentApplicationId",
                table: "UnitSets",
                column: "StudentApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_UWAUnits_UnitSetId",
                table: "UWAUnits",
                column: "UnitSetId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExchangeUnits");

            migrationBuilder.DropTable(
                name: "StaffLogons");

            migrationBuilder.DropTable(
                name: "UWAUnits");

            migrationBuilder.DropTable(
                name: "UnitSets");

            migrationBuilder.DropTable(
                name: "StudentApplications");
        }
    }
}
