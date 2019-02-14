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

            List<ProjectSumReport> reportList = new List<ProjectSumReport>();

            var workPackage = await  _context.WorkPackages.Where(u => u.ProjectId == id).ToListAsync();

            foreach (WorkPackage wp in workPackage)
            {
                ProjectSumReport tempReport = new ProjectSumReport();
                double aHour = 0;
                double eHour = 0;
                var budget = await _context.Budgets.Where(u => u.WorkPackageId == wp.WorkPackageId).ToListAsync();
                foreach (Budget bt in budget)
                {
                    if (bt.Type == 20) {
                        aHour += bt.Hour;
                    } else if (bt.Type == 10) {
                        eHour += bt.Hour;
                    }
                }

                var workPackageReport = await _context.WorkPackageReports.FirstOrDefaultAsync(wpk => wpk.WorkPackageId == wp.WorkPackageId);

                tempReport.WorkPackageCode = wp.WorkPackageId;
                tempReport.WorkPackageName = wp.Name;
                tempReport.AHour = aHour;
                tempReport.EHour = eHour;
                tempReport.Variance = (int)(aHour / eHour * 100);
                tempReport.Comment = workPackageReport.Comments;
                reportList.Add(tempReport);
            }

            var projectReprot = reportList;
            return View(projectReprot);
        }

        private bool ProjectExists(string id)
        {
            return _context.Projects.Any(e => e.ProjectId == id);
        }
    }
}
