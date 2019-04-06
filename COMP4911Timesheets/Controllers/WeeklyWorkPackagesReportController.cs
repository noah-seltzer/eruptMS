using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using COMP4911Timesheets.Data;
using COMP4911Timesheets.Models;

namespace COMP4911Timesheets.Controllers
{
    public class WeeklyWorkPackagesReportController : Controller
    {
        private readonly ApplicationDbContext _context;

        public WeeklyWorkPackagesReportController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: WeeklyWorkPackagesReport
        public async Task<IActionResult> Index()
        {
            var WorkPackage = _context.WorkPackages
                .Include(w => w.ParentWorkPackage)
                .Include(w => w.Project)
                .Include(w => w.ChildWorkPackages)
                .Include(w => w.Budgets)
                .Include(w => w.WorkPackageEmployees)
                .Include(w => w.TimesheetRows);

            return View(await WorkPackage.ToListAsync());
        }

        // GET: WeeklyWorkPackagesReport/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var WorkPackage = await _context.WorkPackages
                .Include(w => w.ParentWorkPackage)
                .Include(w => w.Project)
                .Include(w => w.ChildWorkPackages)
                .Include(w => w.Budgets)
                .Include(w => w.WorkPackageEmployees)
                .Include(w => w.TimesheetRows)
                .FirstOrDefaultAsync(m => m.WorkPackageId == id);
            if (WorkPackage == null)
            {
                return NotFound();
            }

            return View(WorkPackage);
        }

        // GET: WeeklyWorkPackagesReport/Create
        

        private bool WorkPackageExists(int id)
        {
            return _context.WorkPackages.Any(e => e.WorkPackageId == id);
        }
    }
}
