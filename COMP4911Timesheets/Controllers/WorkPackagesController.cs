﻿using System;
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
    public class WorkPackagesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public WorkPackagesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: WorkPackages
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.WorkPackages.Include(w => w.ParentWorkPackage).Include(w => w.Project);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: WorkPackages/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workPackage = await _context.WorkPackages
                .Include(w => w.ParentWorkPackage)
                .Include(w => w.Project)
                .FirstOrDefaultAsync(m => m.WorkPackageId == id);
            if (workPackage == null)
            {
                return NotFound();
            }

            return View(workPackage);
        }

        // GET: WorkPackages/Create
        public IActionResult Create()
        {
            ViewData["ParentWorkPackageId"] = new SelectList(_context.WorkPackages, "WorkPackageId", "WorkPackageId");
            ViewData["ProjectId"] = new SelectList(_context.Projects, "ProjectId", "ProjectId");
            return View();
        }

        // POST: WorkPackages/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("WorkPackageId,WorkPackageCode,Name,Description,Contractor,Purpose,Input,Output,Activity,ProjectId,ParentWorkPackageId")] WorkPackage workPackage)
        {
            if (ModelState.IsValid)
            {
                _context.Add(workPackage);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ParentWorkPackageId"] = new SelectList(_context.WorkPackages, "WorkPackageId", "WorkPackageId", workPackage.ParentWorkPackageId);
            ViewData["ProjectId"] = new SelectList(_context.Projects, "ProjectId", "ProjectId", workPackage.ProjectId);
            return View(workPackage);
        }

        // GET: WorkPackages/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workPackage = await _context.WorkPackages.FindAsync(id);
            if (workPackage == null)
            {
                return NotFound();
            }
            ViewData["ParentWorkPackageId"] = new SelectList(_context.WorkPackages, "WorkPackageId", "WorkPackageId", workPackage.ParentWorkPackageId);
            ViewData["ProjectId"] = new SelectList(_context.Projects, "ProjectId", "ProjectId", workPackage.ProjectId);
            return View(workPackage);
        }

        // POST: WorkPackages/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("WorkPackageId,WorkPackageCode,Name,Description,Contractor,Purpose,Input,Output,Activity,ProjectId,ParentWorkPackageId")] WorkPackage workPackage)
        {
            if (id != workPackage.WorkPackageId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(workPackage);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WorkPackageExists(workPackage.WorkPackageId))
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
            ViewData["ParentWorkPackageId"] = new SelectList(_context.WorkPackages, "WorkPackageId", "WorkPackageId", workPackage.ParentWorkPackageId);
            ViewData["ProjectId"] = new SelectList(_context.Projects, "ProjectId", "ProjectId", workPackage.ProjectId);
            return View(workPackage);
        }

        // GET: WorkPackages/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workPackage = await _context.WorkPackages
                .Include(w => w.ParentWorkPackage)
                .Include(w => w.Project)
                .FirstOrDefaultAsync(m => m.WorkPackageId == id);
            if (workPackage == null)
            {
                return NotFound();
            }

            return View(workPackage);
        }

        // POST: WorkPackages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var workPackage = await _context.WorkPackages.FindAsync(id);
            _context.WorkPackages.Remove(workPackage);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        
        //GET: ProjectWorkPackges/WorkPackages/5
        public async Task<IActionResult> ProjectWorkPackges(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }


            var workPackages = await _context.WorkPackages
             .Where(u => u.ProjectId == id).ToListAsync();

            if (workPackages == null)
            {
                return NotFound();
            }
            return View(workPackages);
        }

        private bool WorkPackageExists(int id)
        {
            return _context.WorkPackages.Any(e => e.WorkPackageId == id);
        }
    }
}
