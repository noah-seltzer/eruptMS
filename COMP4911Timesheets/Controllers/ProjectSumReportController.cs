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
    [Authorize(Roles = "PM,PA,AD")]
    public class ProjectSumReportController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Employee> _usermanager;

        public ProjectSumReportController(ApplicationDbContext context, UserManager<Employee> mgr)
        {
            _context = context;
            _usermanager = mgr;
        }

        // GET: ProjectSumReport
        public async Task<IActionResult> Index()
        {
            var uid = (await _usermanager.GetUserAsync(User)).Id;
            List<Project> managedProjects;

            if (User.IsInRole("AD"))
            {
                managedProjects = await _context.Projects.ToListAsync();
                return View(managedProjects);
            }

            managedProjects = await _context.ProjectEmployees
            .Where(pe => pe.EmployeeId == uid
                        && pe.WorkPackageId == null) // null WP is marker for mgmt roles
            .Join(_context.Projects,
                    p => p.ProjectId,
                    pe => pe.ProjectId,
                    (pe, p) => p)
            .ToListAsync();

            return View(managedProjects);
        }


        // GET: ProjectSumReport/Report/5
        public async Task<IActionResult> Report(int id)
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
                return RedirectToAction(nameof(Index));

            ViewData["ProjectName"] = project.Name;

            List<ProjectSumReport> projectSumReports = new List<ProjectSumReport>();

            var workPackages = await _context.WorkPackages.Where(u => u.ProjectId == id).ToListAsync();

            double PMHourTotal = 0;
            double PMCostTotal = 0;
            double REHourTotal = 0;
            double RECostTotal = 0;
            double AHourTotal = 0;
            double ACostTotal = 0;

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
                        AHourTotal += tempBudget.Hour;
                        aCost += payGrade.Cost * aHour;
                        ACostTotal += payGrade.Cost * aHour;
                    }
                    else if (tempBudget.Type == Budget.ESTIMATE)
                    {
                        PMHour += tempBudget.Hour;
                        PMHourTotal += tempBudget.Hour;
                        PMCost += payGrade.Cost * PMHour * 8;
                        PMCostTotal += payGrade.Cost * PMHour * 8;
                        REHour += tempBudget.REHour;
                        REHourTotal += tempBudget.REHour;
                        RECost += payGrade.Cost * REHour * 8;
                        RECostTotal += payGrade.Cost * REHour * 8;
                    }

                }

                var workPackageReport = await _context.WorkPackageReports.Where(wpk => wpk.WorkPackageId == tempWorkPackage.WorkPackageId).FirstOrDefaultAsync();

                tempReport.WorkPackageCode = tempWorkPackage.WorkPackageCode;
                tempReport.WorkPackageName = tempWorkPackage.Name;
                tempReport.ACost = aCost;
                tempReport.RECost = RECost;
                tempReport.AHour = aHour / 8;
                tempReport.REHour = REHour;
                tempReport.PMHour = PMHour;
                tempReport.PMCost = PMCost;

                if (PMHour != 0)
                {
                    double tempVar = (int)((REHour - PMHour) / PMHour * 10000);
                    tempReport.Variance = tempVar / 100;
                }
                else
                {
                    tempReport.Variance = 0;
                }

                if (REHour != 0)
                {
                    double tempComp = (int)(aHour / REHour * 10000);
                    tempReport.Complete = tempComp / 100;
                }
                else {
                    tempReport.Complete = 0;
                }
                
                if (workPackageReport != null) { 
                    tempReport.Comment = workPackageReport.Comments;
                }
                projectSumReports.Add(tempReport);
            }


            TempData["PMCostTotal"] = PMCostTotal;
            TempData["PMHourTotal"] = PMHourTotal;
            TempData["RECostTotal"] = RECostTotal;
            TempData["REHourTotal"] = REHourTotal;
            TempData["ACostTotal"] = ACostTotal;
            TempData["AHourTotal"] = AHourTotal;

            if (PMHourTotal != 0)
            {
                double VarianceTotal = (int)((REHourTotal - PMHourTotal) / PMHourTotal * 10000);
                TempData["VarianceTotal"] = VarianceTotal / 100;
            }
            else {
                TempData["VarianceTotal"] = 0;
            }
            return View(projectSumReports);
        }

        private bool ProjectExists(int id)
        {
            return _context.Projects.Any(e => e.ProjectId == id);
        }

    }

}
