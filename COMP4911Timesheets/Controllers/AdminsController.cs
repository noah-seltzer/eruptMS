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
using System.Data.SqlClient;

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
            return View();
        }

        public IActionResult Backup()
        {
            Utility.BackupDatabase();
            TempData["Message"] = "Database backed up successfully";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Restore()
        {
            Utility.RestoreDatabase();
            try
            {
                await _userManager.GetUserAsync(User);
            }
            catch (SqlException e)
            {
                TempData["ErrorMessage"] = "Database restore unsuccessful";
                Console.WriteLine(e.ToString());
                return RedirectToAction(nameof(Index));
            }
            TempData["Message"] = "Database restored successfully";
            return RedirectToAction(nameof(Index));
        }
    }
}
