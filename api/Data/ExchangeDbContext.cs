using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography;

namespace ExchangeApproval.Data
{
    public static class ReferenceData
    {
        public readonly static string UWAName = "University of Western Australia";
        public readonly static string UWAHref = "https://www.uwa.edu.au/";
    }

    public class ExchangeDbContext : DbContext
    {
        public ExchangeDbContext(DbContextOptions<ExchangeDbContext> options)
            : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            UWAStaffLogon.OnModelCreating(modelBuilder);
        }

        public DbSet<StudentApplication> StudentApplications { get; set; }
        public DbSet<UnitSet> UnitSets { get; set; }
        public DbSet<ExchangeUnit> ExchangeUnits { get; set; }
        public DbSet<UWAUnit> UWAUnits { get; set; }
        public DbSet<UWAStaffLogon> StaffLogons { get; set; }
    }

    public enum UWAUnitContext { Elective, Core, Complementary };

    public enum UWAUnitLevel
    {
        Zero,
        One,
        Two,
        Three,
        Four,
        GtFour
    };

    public static class UWAUnitLevelExtensions
    {
        public static string GetLabel(this UWAUnitLevel level)
        {
            switch (level)
            {
                case UWAUnitLevel.Zero:
                    return "Insufficent";
                case UWAUnitLevel.One:
                    return "1000";
                case UWAUnitLevel.Two:
                    return "2000";
                case UWAUnitLevel.Three:
                    return "3000";
                case UWAUnitLevel.Four:
                    return "4000";
                default:
                    return ">4000";
            }
        }
    }

    public enum UWAStaffRole { StudentOffice, UnitCoordinator };

    public class UWAStaffLogon
    {
        public int Id { get; set; }
        [Required]
        public string Username { get; set; }
        public UWAStaffRole Role { get; set; }
        [Required]
        public byte[] Salt { get; set; }
        [Required]
        public string PasswordHash { get; set; }

        public static void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<UWAStaffLogon>()
                .Property(l => l.Role)
                .HasConversion<string>();
        }

        public static byte[] GenerateSalt()
        {
            byte[] salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            return salt;
        }

        public static string HashPassword(string password, byte[] salt)
        {
            return Convert.ToBase64String(
                KeyDerivation.Pbkdf2(
                    password: password,
                    salt: salt,
                    prf: KeyDerivationPrf.HMACSHA1,
                    iterationCount: 10000,
                    numBytesRequested: 256 / 8
                )
            );
        }
    }

    public enum ApplicationStatus
    {
        New,
        Incomplete,
        Completed,
        Deleted
    }

    public class StudentApplication
    {
        public int StudentApplicationId { get; set; }
        public DateTime SubmittedAt { get; set; }
        public DateTime LastUpdatedAt { get; set; }
        public DateTime CompletedAt { get; set; }
        public string StudentName { get; set; }
        public string StudentNumber { get; set; }
        public DateTime ExchangeDate { get; set; }
        public string Major1st { get; set; }
        public string Major2nd { get; set; }
        public string ExchangeUniversityCountry { get; set; }
        public string ExchangeUniversityHref { get; set; }
        public string ExchangeUniversityName { get; set; }
        public string Notes { get; set; }
        public virtual ICollection<UnitSet> UnitSets { get; set; }
    }

    public class UnitSet
    {
        public int UnitSetId { get; set; }
        public int? StudentApplicationId { get; set; }
        public virtual StudentApplication StudentApplication { get; set; }
        public string ExchangeUniversityCountry { get; set; }
        public string ExchangeUniversityHref { get; set; }
        public string ExchangeUniversityName { get; set; }
        public DateTime SubmittedAt { get; set; }
        public DateTime LastUpdatedAt { get; set; }
        public DateTime CompletedAt { get; set; }
        public virtual ICollection<ExchangeUnit> ExchangeUnits { get; set; }
        public virtual ICollection<UWAUnit> UWAUnits { get; set; }
        public bool? IsEquivalent { get; set; }
        public bool? IsContextuallyApproved { get; set; }
        public UWAUnitLevel? EquivalentUWAUnitLevel { get; set; }
    }

    public class ExchangeUnit
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Title { get; set; }
        public string Href { get; set; }
        public int? UnitSetId { get; set; }
        public virtual UnitSet UnitSet { get; set; }
    }

    public class UWAUnit
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Title { get; set; }
        public string Href { get; set; }
        public int? UnitSetId { get; set; }
        public virtual UnitSet UnitSet { get; set; }
    }
}