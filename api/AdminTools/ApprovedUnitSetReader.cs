using System.Collections.Generic;
using System.IO;
using ExchangeApproval.Data;
using CsvHelper;
using System.Linq;
using System;

namespace ExchangeApproval.AdminTools
{
    public class EquivalenceUnitSetRow
    {
        public int UnitSetId { get; set; }
        public bool IsExchangeUnit { get; set; }
        public string UniversityCountry { get; set; }
        public string UniversityName { get; set; }
        public string UniversityHref { get; set; }
        public string UnitTitle { get; set; }
        public string UnitCode { get; set; }
        public string UnitHref { get; set; }
        public UWAUnitLevel EquivalentUWAUnitLevel { get; set; }
    }

    public static class EquivalenceUnitSetsReader
    {
        public static IList<(int line, string error)> ValidateRows(IReadOnlyCollection<EquivalenceUnitSetRow> rows)
        {
            var errors = new List<string>();
            var numberedRows = rows.Select((r, i) => new { LineNumber = i + 1, Row = r });

            var invalidUnitLevel = numberedRows
                .Where(r => r.Row.EquivalentUWAUnitLevel == UWAUnitLevel.Zero)
                .Select(r => (r.LineNumber, "Approved unit set cannot be equivalent to a level 0 unit."));
            var badUnitLevelForGroup = numberedRows
                .GroupBy(r => r.Row.UnitSetId)
                .Where(g => g.Select(r => r.Row.EquivalentUWAUnitLevel).Distinct().Count() > 1)
                .SelectMany(g => g)
                .Select(r => (r.LineNumber, "The group of unit sets must have the same equivalence level for each row."));
            var missingExchangeUnit = numberedRows
                .GroupBy(r => r.Row.UnitSetId)
                .Where(g => !g.Any(r => r.Row.IsExchangeUnit))
                .SelectMany(g => g)
                .Select(r => (r.LineNumber, "A unit set group must include at least one exchange unit."));
            var missingUnitCode = numberedRows
                .Where(r => string.IsNullOrWhiteSpace(r.Row.UnitCode))
                .Select(r => (r.LineNumber, "A unit set must include a unit code."));
            var missingUniversityName = numberedRows
                .Where(r => string.IsNullOrWhiteSpace(r.Row.UniversityName))
                .Select(r => (r.LineNumber, "A unit set must a university name."));
            return invalidUnitLevel
                .Union(badUnitLevelForGroup)
                .Union(missingExchangeUnit)
                .Union(missingUnitCode)
                .ToList();
        }

        public static (IList<(int line, string error)> Errors, IEnumerable<UnitSet> unitSets) LoadEquivalencies(TextReader reader)
        {
            var csv = new CsvReader(reader);
            try
            {
                var rows = csv.GetRecords<EquivalenceUnitSetRow>().ToList();
                var errors = ValidateRows(rows);
                if (errors.Count > 0)
                {
                    return (errors, null);
                }
                var approvedUnitSets = rows
                    .GroupBy(r => r.UnitSetId)
                    .Select(g => new UnitSet
                    {
                        IsEquivalent = true,
                        IsContextuallyApproved = null,
                        EquivalentUWAUnitLevel = g.First().EquivalentUWAUnitLevel,
                        ExchangeUniversityName = g.First().UniversityName,
                        ExchangeUniversityCountry = g.First().UniversityCountry,
                        ExchangeUniversityHref = g.First().UniversityHref,
                        ExchangeUnits = g
                            .Where(r => r.IsExchangeUnit)
                            .Select(r => new ExchangeUnit
                            {
                                Code = r.UnitCode,
                                Title = r.UnitTitle,
                                Href = r.UnitHref
                            }).ToList(),
                        UWAUnits = g
                            .Where(r => !r.IsExchangeUnit)
                            .Select(r => new UWAUnit
                            {
                                Code = r.UnitCode,
                                Title = r.UnitTitle,
                                Href = r.UnitHref
                            }).ToList()
                    });
                return (null, approvedUnitSets);
            }
            catch (ValidationException ex)
            {
                return (new[] { (0, ex.Message.Split(".").First()) }, null);
            }
        }

        public static IEnumerable<EquivalenceUnitSetRow> DumpEquivalencies(ExchangeDbContext db)
        {
            return db.UnitSets
                .Where(us => us.StudentApplicationId == null)
                .Select(us => new
                {
                    ExchangeUnits = us.ExchangeUnits.Select(u => new EquivalenceUnitSetRow
                    {
                        UnitSetId = us.UnitSetId,
                        IsExchangeUnit = true,
                        UniversityCountry = us.ExchangeUniversityCountry,
                        UniversityName = us.ExchangeUniversityName,
                        UniversityHref = us.ExchangeUniversityHref,
                        UnitTitle = u.Title,
                        UnitCode = u.Code,
                        UnitHref = u.Href,
                        EquivalentUWAUnitLevel = us.EquivalentUWAUnitLevel.GetValueOrDefault(UWAUnitLevel.One)
                    }),
                    UWAUnits = us.UWAUnits.Select(u => new EquivalenceUnitSetRow
                    {
                        UnitSetId = us.UnitSetId,
                        IsExchangeUnit = false,
                        UniversityCountry = ReferenceData.UWACountry,
                        UniversityName = ReferenceData.UWAName,
                        UniversityHref = ReferenceData.UWAHref,
                        UnitTitle = u.Title,
                        UnitCode = u.Code,
                        UnitHref = u.Href,
                        EquivalentUWAUnitLevel = us.EquivalentUWAUnitLevel.GetValueOrDefault(UWAUnitLevel.One)
                    })
                })
                .SelectMany(agg => agg.ExchangeUnits.Union(agg.UWAUnits))
                .OrderBy(r => r.UnitSetId)
                .ThenBy(r => r.IsExchangeUnit)
                .ThenBy(r => r.UnitCode)
                .ToList();
        }

        public static void UpdateEquivalenciesInDatabase(ExchangeDbContext db, IEnumerable<UnitSet> newManualUnitApprovals)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                var existingManualUnitSets = db.UnitSets.Where(u => u.StudentApplicationId == null).ToList();
                db.RemoveRange(existingManualUnitSets);
                db.AddRange(newManualUnitApprovals);
                db.SaveChanges();
                transaction.Commit();
            }
        }
    }
}