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
    public class BudgetsController : Controller
    {
        private static int? workpackageId;
        private readonly ApplicationDbContext _context;

        public BudgetsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Budgets
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Budgets.Include(b => b.PayGrade).Include(b => b.WorkPackage);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Budgets/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var budget = await _context.Budgets
                .Include(b => b.PayGrade)
                .Include(b => b.WorkPackage)
                .FirstOrDefaultAsync(m => m.BudgetId == id);
            if (budget == null)
            {
                return NotFound();
            }
            return View(budget);

        }

        // GET: Budgets/Create/5
        public async Task<IActionResult> Create(int? id)
        {
            workpackageId = id;
            var workPackages = await _context.WorkPackages.FirstOrDefaultAsync(m => m.ParentWorkPackageId == id);
            if (workPackages == null) {
                ViewData["PayGradeId"] = new SelectList(_context.PayGrades, "PayGradeId", "PayGradeId");
                ViewData["WorkPackageId"] = new SelectList(_context.WorkPackages, "WorkPackageId", "WorkPackageId");
                return View();
            }

            TempData["info"] = "Budget plan only can be added into leaf workpackages";
            var wpTemp = await _context.WorkPackages.FirstOrDefaultAsync(m => m.WorkPackageId == workpackageId); ;
            return RedirectToAction("ProjectWorkPackges", "WorkPackages", new { id = wpTemp.ProjectId });

        }

        // POST: Budgets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BudgetId,Hour,Status,WeekNumber,Type,WorkPackageId,PayGradeId")] Budget budget)
        {
            budget.WorkPackageId = workpackageId;

            if (ModelState.IsValid)
            {
                _context.Add(budget);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PayGradeId"] = new SelectList(_context.PayGrades, "PayGradeId", "PayGradeId", budget.PayGradeId);
            ViewData["WorkPackageId"] = new SelectList(_context.WorkPackages, "WorkPackageId", "WorkPackageId", budget.WorkPackageId);
            return View(budget);
        }

        // GET: Budgets/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var budget = await _context.Budgets.FindAsync(id);
            if (budget == null)
            {
                return NotFound();
            }
            ViewData["PayGradeId"] = new SelectList(_context.PayGrades, "PayGradeId", "PayGradeId", budget.PayGradeId);
            ViewData["WorkPackageId"] = new SelectList(_context.WorkPackages, "WorkPackageId", "WorkPackageId", budget.WorkPackageId);
            return View(budget);
        }

        // POST: Budgets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BudgetId,Hour,Status,WeekNumber,Type,WorkPackageId,PayGradeId")] Budget budget)
        {
            if (id != budget.BudgetId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(budget);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BudgetExists(budget.BudgetId))
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
            ViewData["PayGradeId"] = new SelectList(_context.PayGrades, "PayGradeId", "PayGradeId", budget.PayGradeId);
            ViewData["WorkPackageId"] = new SelectList(_context.WorkPackages, "WorkPackageId", "WorkPackageId", budget.WorkPackageId);
            return View(budget);
        }

        // GET: Budgets/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var budget = await _context.Budgets
                .Include(b => b.PayGrade)
                .Include(b => b.WorkPackage)
                .FirstOrDefaultAsync(m => m.BudgetId == id);
            if (budget == null)
            {
                return NotFound();
            }

            return View(budget);
        }

        // POST: Budgets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var budget = await _context.Budgets.FindAsync(id);
            _context.Budgets.Remove(budget);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BudgetExists(int id)
        {
            return _context.Budgets.Any(e => e.BudgetId == id);
        }
    }
}
