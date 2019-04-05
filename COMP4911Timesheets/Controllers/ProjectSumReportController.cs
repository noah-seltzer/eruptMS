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

namespace COMP4911Timesheets.Controllers
{
    //[Authorize]
    public class ProjectSumReportController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Employee> _userManager;

        public ProjectSumReportController(ApplicationDbContext context, UserManager<Employee> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: ProjectSumReport
        public async Task<IActionResult> Index()
        {
            return View(await _context.Projects.ToListAsync());
        }


        // GET: ProjectSumReport/Report/5
        public async Task<IActionResult> Report(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }

            var project = _context.Projects.Where(m => m.ProjectId == id).FirstOrDefault();
            var users = _userManager.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
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
