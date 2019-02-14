using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using COMP4911Timesheets.Data;
using COMP4911Timesheets.Models;

namespace COMP4911Timesheets.Controllers
{
    public class ProjectsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProjectsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Projects
        public async Task<IActionResult> Index()
        {
            return View(await _context.Projects.ToListAsync());
        }

        // GET: Projects/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Projects
                .FirstOrDefaultAsync(m => m.ProjectId == id);
            if (project == null)
            {
                return NotFound();
            }
            return View(project);
        }

        // GET: Projects/Report/5
        public async Task<IActionResult> Report(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Projects
                .FirstOrDefaultAsync(m => m.ProjectId == id);

            if (project == null)
            {
                return NotFound();
            }

            ViewData["ProjectName"] = project.Name;

            List<ProjectSumReport> projectSumReports = new List<ProjectSumReport>();

            var workPackages = await  _context.WorkPackages.Where(u => u.ProjectId == id).ToListAsync();

            foreach (WorkPackage tempWorkPackage in workPackages)
            {   
                ProjectSumReport tempReport = new ProjectSumReport();
                double aHour = 0;
                double eHour = 0;
                double aCost = 0;
                double eCost = 0;
                var budgets = await _context.Budgets.Where(u => u.WorkPackageId == tempWorkPackage.WorkPackageId).ToListAsync();
                foreach (Budget tempBudget in budgets)
                {
                    var payGrade = await _context.PayGrades.Where(p => p.PayGradeId == tempBudget.PayGradeId).FirstOrDefaultAsync();
                    if (tempBudget.Type == Budget.ACTUAL) {
                        aHour += tempBudget.Hour;
                        aCost += payGrade.Cost * aHour;
                    } else if (tempBudget.Type == Budget.ESTIMATE) {
                        eHour += tempBudget.Hour;
                        eCost += payGrade.Cost * eHour;
                    }                    

                }

                var workPackageReport = await _context.WorkPackageReports.FirstOrDefaultAsync(wpk => wpk.WorkPackageId == tempWorkPackage.WorkPackageId);

                tempReport.WorkPackageCode = tempWorkPackage.WorkPackageId;
                tempReport.WorkPackageName = tempWorkPackage.Name;
                tempReport.ACost = aCost;
                tempReport.ECost = eCost;
                tempReport.AHour = aHour;
                tempReport.EHour = eHour;
                tempReport.Variance = (int)(aHour / eHour * 100);
                tempReport.Comment = workPackageReport.Comments;
                projectSumReports.Add(tempReport);
            }

            return View(projectSumReports);
        }

        private bool ProjectExists(string id)
        {
            return _context.Projects.Any(e => e.ProjectId == id);
        }
    }
}
