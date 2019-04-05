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
        private readonly UserManager<Employee> _usermgr;

        public ProjectSumReportController(ApplicationDbContext context, UserManager<Employee> mgr)
        {
            _context = context;
            _usermgr = mgr;
        }

        // GET: ProjectSumReport
        public async Task<IActionResult> Index()
        {
            var uid = (await _usermgr.GetUserAsync(User)).Id;
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

            var project = await _context.Projects
                .FirstOrDefaultAsync(m => m.ProjectId == id);

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
            var uid = (await _usermgr.GetUserAsync(User)).Id;
            if (!User.IsInRole("AD") && manager.EmployeeId != uid && assistant != null && assistant.EmployeeId != uid)
                return RedirectToAction(nameof(Index));

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

                double tempVar = (int)(aHour / REHour * 10000);
                tempReport.Variance = tempVar / 100;

                if(workPackageReport != null) { 
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
