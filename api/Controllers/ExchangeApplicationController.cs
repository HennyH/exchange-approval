using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using ExchangeApproval.Data;
using ExchangeApproval.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExchangeApproval.Controllers
{
    [Route("/api/application")]
    public class ExchangeApplicationController : Controller
    {
        private readonly ExchangeDbContext _db;

        public ExchangeApplicationController(ExchangeDbContext db)
        {
            this._db = db;
        }

        private static StudentApplication MapApplicationFormToApplication(ExchangeDbContext db, ApplicationFormVM form, bool asStaff)
        {
            var now = DateTime.UtcNow;
            Func<String, UWAUnitLevel?> parseUnitLevel = (label) => {
                object level;
                if (Enum.TryParse(typeof(UWAUnitLevel), label, true, out level)) {
                    return (UWAUnitLevel?)level;
                }
                return (UWAUnitLevel?)null;
            };

            var newApplication = new StudentApplication
            {
                SubmittedAt = now,
                LastUpdatedAt = now,
                StudentName = form.StudentDetailsForm.Name,
                StudentNumber = form.StudentDetailsForm.Email,
                Major1st = form.StudentDetailsForm.Major,
                Major2nd = form.StudentDetailsForm.Major2nd,
                StudentOffice = form.StudentDetailsForm.StudentOffice?.Value,
                ExchangeUniversityCountry = form.ExchangeUniversityForm.UniversityCountry,
                ExchangeUniversityName  = form.ExchangeUniversityForm.UniversityName,
                ExchangeUniversityHref = form.ExchangeUniversityForm.UniversityHomepage,
                Notes = null,
                UnitSets = form.UnitSetForms.Select(f => new UnitSet
                {
                    ExchangeUniversityName  = form.ExchangeUniversityForm.UniversityName,
                    ExchangeUniversityHref = form.ExchangeUniversityForm.UniversityHomepage,
                    IsEquivalent = asStaff
                        ? f.StaffApprovalForm.IsEquivalent.Value
                        : (bool?)null,
                    IsContextuallyApproved = asStaff
                        ? f.StaffApprovalForm.IsContextuallyApproved.Value
                        : (bool?)null,
                    EquivalentUWAUnitLevel = asStaff
                        ? parseUnitLevel(f.StaffApprovalForm.EquivalentUnitLevel.Label)
                        : (UWAUnitLevel?)null,
                    ExchangeUnits = f.ExchangeUnitsForm.Select(u => new ExchangeUnit
                    {
                        Code = u.UnitCode,
                        Title = u.UnitName,
                        Href = u.UnitHref
                    }).ToList(),
                    UWAUnits = f.UWAUnitsFormForm?.Select(u => new UWAUnit
                    {
                        Code = u.UnitCode,
                        Title = u.UnitName,
                        Href = u.UnitHref
                    }).ToList() ?? new List<UWAUnit>(),
                }).ToList(),
            };
            /* Preserve when the student submitted the application */
            if (asStaff && form.ApplicationId.HasValue)
            {
                var oldApplication = db.StudentApplications.FirstOrDefault(a => a.StudentApplicationId == form.ApplicationId);
                if (oldApplication != null)
                {
                    newApplication.SubmittedAt = oldApplication.SubmittedAt;
                }
            }
            return newApplication;
        }

        [HttpGet]
        public ActionResult GetAppliction(int id)
        {
            var application = this._db.StudentApplications
                .Single(a => a.StudentApplicationId == id);
            var form = new ApplicationFormVM
            {
                ApplicationId = application.StudentApplicationId,
                StudentDetailsForm = new StudentDetailsFormVM
                {
                    Name = application.StudentName,
                    Degree = application.Degree,
                    Email = application.StudentNumber,
                    Major = application.Major1st,
                    Major2nd = application.Major2nd,
                    StudentOffice = new SelectOption<string>
                    {
                        Value = application.StudentOffice,
                        Label = application.StudentOffice,
                        Selected = application.StudentOffice != null
                    }
                },
                ExchangeUniversityForm = new ExchangeUniversityFormVM
                {
                    UniversityCountry = application.ExchangeUniversityCountry,
                    UniversityName = application.ExchangeUniversityName,
                    UniversityHomepage = application.ExchangeUniversityHref
                },
                UnitSetForms = application.UnitSets.Select(us => new UnitSetFormVM
                {
                    ExchangeUnitsForm = us.ExchangeUnits.Select(u => new UnitFormVM
                    {
                        UnitName = u.Title,
                        UnitCode = u.Code,
                        UnitHref = u.Href
                    }).ToList(),
                    UWAUnitsFormForm = us.UWAUnits.Select(u => new UnitFormVM
                    {
                        UnitName = u.Title,
                        UnitCode = u.Code,
                        UnitHref = u.Href
                    }).ToList(),
                    StaffApprovalForm = new StaffApprovalFormVM
                    {
                        EquivalentUnitLevel = new SelectOption<UWAUnitLevel?>(
                            us.EquivalentUWAUnitLevel,
                            us.EquivalentUWAUnitLevel?.ToString() ?? "Pending",
                            us.EquivalentUWAUnitLevel.HasValue
                        ),
                        IsContextuallyApproved = new SelectOption<bool?>(
                            us.IsContextuallyApproved,
                            us.IsContextuallyApproved?.ToString() ?? "Pending",
                            us.IsContextuallyApproved.HasValue
                        ),
                        IsEquivalent = new SelectOption<bool?>(
                            us.IsEquivalent,
                            us.IsEquivalent?.ToString() ?? "Pending",
                            us.IsEquivalent.HasValue
                        )
                    }
                }).ToList()
            };
            return Json(form);
        }

        [HttpPost("submit")]
        public ActionResult SubmitApplication([FromBody]ApplicationFormVM form)
        {
            var application = MapApplicationFormToApplication(this._db, form, asStaff: false);
            this._db.Add(application);
            this._db.SaveChanges();
            return Json(form);
        }

        [Authorize]
        [HttpPost("update")]
        public ActionResult UpdateApplication([FromBody]ApplicationFormVM form)
        {
            var now = DateTime.UtcNow;
            using (var transaction = this._db.Database.BeginTransaction())
            {
                var newVersion = MapApplicationFormToApplication(this._db, form, asStaff: true);
                var oldVersion = this._db.StudentApplications.FirstOrDefault(a => a.StudentApplicationId == form.ApplicationId);
                this._db.Remove(oldVersion);
                this._db.SaveChanges();
                newVersion.StudentApplicationId = oldVersion.StudentApplicationId;
                this._db.Add(newVersion);
                this._db.SaveChanges();
                transaction.Commit();
            }

            return new StatusCodeResult((int)HttpStatusCode.NoContent);
        }
    }
}