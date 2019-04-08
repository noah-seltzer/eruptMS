using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using COMP4911Timesheets.Data;
using COMP4911Timesheets.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using COMP4911Timesheets.ViewModels;

namespace COMP4911Timesheets.Controllers
{
    [Authorize(Roles = "PM,PA,AD,RE")]
    public class ResponsibleEngineerReportsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Employee> _usermanager;

        public ResponsibleEngineerReportsController(ApplicationDbContext context, UserManager<Employee> mgr)
        {
            _context = context;
            _usermanager = mgr;
        }

        // GET: ResponsibleEngineerReports/Reports/6
        public async Task<IActionResult> Reports(int? id)
        {
            var workPackages = await _context.WorkPackages.FirstOrDefaultAsync(m => m.ParentWorkPackageId == id);
            var workPackage = await _context.WorkPackages.FindAsync(id);

            TempData["workPackageName"] = workPackage.Name;
            TempData["workPackageId"] = workPackage.WorkPackageId;
            TempData["workPackageStatus"] = workPackage.Status;
            return View(workPackage.ResponsibleEngineerReports);
        }
        
        // ResponsibleEngineerReports/Create/6
        [Authorize(Roles = "AD,RE")]
        public async Task<IActionResult> Create(int? id)
        {
            //TODO: disable if workpackage is closed, or if leaf work package.
            //TODO: Also disable if the Responsible Engineer report has already been created for the week.
            var workPackages = await _context.WorkPackages.FirstOrDefaultAsync(m => m.ParentWorkPackageId == id && m.Status != WorkPackage.CLOSED);

            var workPackage = await _context.WorkPackages.FindAsync(id);
            ResponsibleEngineerReport respEngReport = new ResponsibleEngineerReport
            {
                WeekNumber = Utility.GetWeekNumberByDate(DateTime.Today),
                WorkPackageId = id,
                WorkPackage = workPackage
            };
            return View(respEngReport);

            // if (workPackages == null)
            // {
            //}

            //TempData["info"] = "Responsible Engineer reports can only be created on leaf work packages";
            //var wpTemp = await _context.WorkPackages.FirstOrDefaultAsync(m => m.WorkPackageId == id);
            //return RedirectToAction("index", "ResponsibleEngineerReport", new { id = wpTemp.ProjectId });
        }
        
        // POST: ResponsibleEngineerReports/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "AD,RE")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("WeekNumber,WorkPackageId,WorkPackage,Comments,WorkAccomplished,WorkPlanned,Problem,ProblemAnticipated")] ResponsibleEngineerReport report)
        {
            if (ModelState.IsValid)
            {
                report.WorkPackage = await _context.WorkPackages.FindAsync(report.WorkPackageId);
                var workPackageREReports = report.WorkPackage.ResponsibleEngineerReports;
                
                //Invalid if there has already been a REreport created for this week.
                if (workPackageREReports != null && workPackageREReports.Any(r => r.WeekNumber == report.WeekNumber))
                {
                    ViewBag.CodeError = "Responsible Engineer Report already created for this week.";
                    return View(report);
                }

                //Invalid if work package is closed
                if (report.WorkPackage.Status == WorkPackage.CLOSED) {
                    ViewBag.CodeError = "Cannot create Responsible Engineer Report for closed work packages.";
                    return View(report);
                }

                //Invalid if work package is a parent
                if (report.WorkPackage.ChildWorkPackages == null) {
                    ViewBag.CodeError = "Can only create Responsible Engineer Report for leaf work packages.";
                    return View(report);
                }

                report.Status = ResponsibleEngineerReport.VALID;

                _context.Add(report);

                _context.SaveChanges();

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(ReportDetails), report.ResponsibleEngineerReportId);
            }
            return View(report);
        }

        // GET: ResponsibleEngineerReports/ReportDetails/5
        public async Task<IActionResult> ReportDetails(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }

            var project = _context.Projects.Where(m => m.ProjectId == id).FirstOrDefault();
            var users = _usermanager.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            var projectEmployee = _context.ProjectEmployees
                .Where(u => u.ProjectId == id && u.EmployeeId == users.Id).FirstOrDefault();

            if (!User.IsInRole(role: "PM") && !User.IsInRole(role: "PA") && !User.IsInRole(role: "AD"))
            {
                TempData["info"] = "Please login with AD, PM OR PA";
                return RedirectToAction("Index", "ProjectSumReport");
            }

            if ((User.IsInRole(role: "PM") || User.IsInRole(role: "PA")) && projectEmployee == null)
            {
                TempData["info"] = "You are not the project's PM or PA, Please choose the currect project";
                return RedirectToAction("Index", "ProjectSumReport");
            }

            if (project == null)
            {
                return NotFound();
            }

            var manager = _context.ProjectEmployees
                .Where(e => e.ProjectId == id && e.Role == ProjectEmployee.PROJECT_MANAGER)
                .FirstOrDefault();

            var assistant = _context.ProjectEmployees
                .Where(e => e.ProjectId == id && e.Role == ProjectEmployee.PROJECT_ASSISTANT)
                .FirstOrDefault();

            //Check authorization to see report
            var uid = (await _usermanager.GetUserAsync(User)).Id;
            if (!User.IsInRole("AD") && manager.EmployeeId != uid && assistant != null && assistant.EmployeeId != uid)
                return RedirectToAction("asdf");

            ViewData["ProjectName"] = project.Name;

            List<ProjectSumReport> projectSumReports = new List<ProjectSumReport>();

            var workPackages = await _context.WorkPackages.Where(u => u.ProjectId == id).ToListAsync();

            foreach (WorkPackage tempWorkPackage in workPackages)
            {
                ProjectSumReport tempReport = new ProjectSumReport();
                double aHour = 0;
                double PMHour = 0;
                double REHour = 0;
                double aCost = 0;
                double PMCost = 0;
                double RECost = 0;
                var budgets = await _context.Budgets.Where(u => u.WorkPackageId == tempWorkPackage.WorkPackageId).ToListAsync();
                foreach (Budget tempBudget in budgets)
                {
                    var payGrade = await _context.PayGrades.Where(p => p.PayGradeId == tempBudget.PayGradeId).FirstOrDefaultAsync();
                    if (tempBudget.Type == Budget.ACTUAL)
                    {
                        aHour += tempBudget.Hour;
                        aCost += payGrade.Cost * aHour;
                    }
                    else if (tempBudget.Type == Budget.ESTIMATE)
                    {
                        PMHour += tempBudget.Hour;
                        PMCost += payGrade.Cost * PMHour;
                        REHour += tempBudget.REHour;
                        RECost += payGrade.Cost * REHour;
                    }

                }

                var workPackageReport = await _context.WorkPackageReports.FirstOrDefaultAsync(wpk => wpk.WorkPackageId == tempWorkPackage.WorkPackageId);

                tempReport.WorkPackageCode = tempWorkPackage.WorkPackageCode;
                tempReport.WorkPackageName = tempWorkPackage.Name;
                tempReport.ACost = aCost;
                tempReport.RECost = RECost;
                tempReport.AHour = aHour / 8;
                tempReport.REHour = REHour / 8;
                tempReport.PMHour = PMHour / 8;
                tempReport.PMCost = PMCost;

                if (REHour != 0)
                {
                    double tempVar = (int)(aHour / REHour * 10000);
                    tempReport.Variance = tempVar / 100;
                }
                else {
                    tempReport.Variance = 0;
                }

                if (workPackageReport != null) { 
                    tempReport.Comment = workPackageReport.Comments;
                }
                projectSumReports.Add(tempReport);
            }

            return View(projectSumReports);
        }

        private bool ProjectExists(int id)
        {
            return _context.Projects.Any(e => e.ProjectId == id);
        }
    }
}
