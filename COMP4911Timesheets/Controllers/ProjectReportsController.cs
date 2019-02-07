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
    public class ProjectReportsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProjectReportsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ProjectReports
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.ProjectReports.Include(p => p.Project);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: ProjectReports/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var projectReport = await _context.ProjectReports
                .Include(p => p.Project)
                .FirstOrDefaultAsync(m => m.ProjectReportId == id);
            if (projectReport == null)
            {
                return NotFound();
            }

            return View(projectReport);
        }

        // GET: ProjectReports/Create
        public IActionResult Create()
        {
            ViewData["ProjectId"] = new SelectList(_context.Projects, "ProjectId", "ProjectId");
            return View();
        }

        // POST: ProjectReports/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProjectReportId,StartingPercentage,CompletedPercentage,CreatedTime,Status,ProjectId")] ProjectReport projectReport)
        {
            if (ModelState.IsValid)
            {
                _context.Add(projectReport);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProjectId"] = new SelectList(_context.Projects, "ProjectId", "ProjectId", projectReport.ProjectId);
            return View(projectReport);
        }

        // GET: ProjectReports/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var projectReport = await _context.ProjectReports.FindAsync(id);
            if (projectReport == null)
            {
                return NotFound();
            }
            ViewData["ProjectId"] = new SelectList(_context.Projects, "ProjectId", "ProjectId", projectReport.ProjectId);
            return View(projectReport);
        }

        // POST: ProjectReports/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProjectReportId,StartingPercentage,CompletedPercentage,CreatedTime,Status,ProjectId")] ProjectReport projectReport)
        {
            if (id != projectReport.ProjectReportId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(projectReport);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProjectReportExists(projectReport.ProjectReportId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProjectId"] = new SelectList(_context.Projects, "ProjectId", "ProjectId", projectReport.ProjectId);
            return View(projectReport);
        }

        // GET: ProjectReports/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var projectReport = await _context.ProjectReports
                .Include(p => p.Project)
                .FirstOrDefaultAsync(m => m.ProjectReportId == id);
            if (projectReport == null)
            {
                return NotFound();
            }

            return View(projectReport);
        }

        // POST: ProjectReports/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var projectReport = await _context.ProjectReports.FindAsync(id);
            _context.ProjectReports.Remove(projectReport);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProjectReportExists(int id)
        {
            return _context.ProjectReports.Any(e => e.ProjectReportId == id);
        }
    }
}
