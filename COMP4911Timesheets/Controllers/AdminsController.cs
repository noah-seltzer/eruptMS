using COMP4911Timesheets.Data;
using COMP4911Timesheets.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace COMP4911Timesheets.Controllers
{
    [Authorize(Roles = "AD")]
    public class AdminsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Employee> _userManager;

        public AdminsController(
            ApplicationDbContext context,
            UserManager<Employee> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET
        public IActionResult Index()
        {
            if (TempData["Message"] != null)
            {
                ViewBag.Message = TempData["Message"];
            }
            if (TempData["ErrorMessage"] != null)
            {
                ViewBag.Message = TempData["ErrorMessage"];
            }
            ViewData["DomainName"] = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}";
            return View();
        }

        public IActionResult Backup()
        {
            try
            {
                Utility.BackupDatabase();
            }
            catch (SqlException e)
            {
                TempData["ErrorMessage"] = "Database backup unsuccessful";
                Console.WriteLine(e.ToString());
                return RedirectToAction(nameof(Index));
            }
            TempData["Message"] = "Database backed up successfully";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Restore()
        {
            try
            {
                Utility.RestoreDatabase();
            }
            catch (SqlException e)
            {
                TempData["ErrorMessage"] = "Database restore unsuccessful";
                Console.WriteLine(e.ToString());
                return RedirectToAction(nameof(Index));
            }

            try
            {
                await _userManager.GetUserAsync(User);
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.ToString());
                return RedirectToAction(nameof(Index));
            }
            TempData["Message"] = "Database restored successfully";
            return RedirectToAction(nameof(Index));
        }
    }
}