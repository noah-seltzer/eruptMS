using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using COMP4911Timesheets.Data;
using COMP4911Timesheets.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using System.Collections;
using System.Security.Claims;
using COMP4911Timesheets.ViewModels;

namespace COMP4911Timesheets.Controllers
{
    [Authorize(Roles = "AD,HR")]
    public class PayGradesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PayGradesController(
            ApplicationDbContext context)
        {
            _context = context;
        }

        // GET
        public async Task<IActionResult> Index()
        {
            var payGrades = await _context.PayGrades
                .Include(pg => pg.EmployeePays)
                .Where(pg => pg.Year == DateTime.Now.Year)
                .OrderBy(pg => pg.PayGradeId).ToListAsync();
            var payGradeManagements = new List<PayGradeManagement>();
            foreach (var payGrade in payGrades)
            {
                payGradeManagements.Add
                (
                    new PayGradeManagement
                    {
                        PayGrade = payGrade,
                        EmployeePay = await _context.EmployeePays.Where(ep => ep.PayGradeId == payGrade.PayGradeId).Where(ep => ep.Status == EmployeePay.VALID).Include(ep => ep.Employee).FirstOrDefaultAsync()
                    }
                );
            }

            return View(payGradeManagements);
        }

        // GET: Employees/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PayGradeManagement payGradeManagement)
        {
            if (ModelState.IsValid)
            {
                if (await _context.PayGrades.Where(pg => pg.PayLevel == payGradeManagement.PayGrade.PayLevel).FirstOrDefaultAsync() != null)
                {
                    return RedirectToAction(nameof(Index));
                }
                payGradeManagement.PayGrade.Year = DateTime.Now.Year;
                await _context.AddAsync(payGradeManagement.PayGrade);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(payGradeManagement);
        }

        // GET
        public async Task<IActionResult> Edit(int id)
        {
            if (id <= 0)
            {
                return NotFound();
            }

            var payGrade = await _context.PayGrades.FindAsync(id);
            if (payGrade == null)
            {
                return NotFound();
            }

            var payGradeManagement = new PayGradeManagement
            {
                PayGrade = payGrade
            };

            return View(payGradeManagement);
        }

        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PayGradeManagement payGradeManagment)
        {
            if (id != payGradeManagment.PayGrade.PayGradeId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    payGradeManagment.PayGrade.Year = DateTime.Now.Year;
                    _context.PayGrades.Update(payGradeManagment.PayGrade);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PayGradeExists(payGradeManagment.PayGrade.PayGradeId))
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

            return View(payGradeManagment);
        }
        
        private bool PayGradeExists(int id)
        {
            return _context.PayGrades.Any(pg => pg.PayGradeId == id);
        }
    }
}
