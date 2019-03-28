using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using COMP4911Timesheets.Data;
using COMP4911Timesheets.Models;
using System.Diagnostics;

namespace COMP4911Timesheets.Controllers
{
    public class WorkPackagesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private static int? projectId;
        private static int? parentWorkPKId;
        public static int PROJECT_CODE_LENGTH = 4;

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

            var budgets = await _context.Budgets.Where(a => a.WorkPackageId == id).ToListAsync();

            workPackage.Budgets = budgets;

            if (workPackage == null)
            {
                return NotFound();
            }

            return View(workPackage);
        }

        // GET: WorkPackages/CreateWorkPackage
        public IActionResult CreateWorkPackage()
        {
            TempData["projectId"] = projectId;
            return View();
        }

        // POST: WorkPackages/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("WorkPackageId,WorkPackageCode,Name,Description,Contractor,Purpose,Input,Output,Activity,ProjectId,ParentWorkPackageId")] WorkPackage workPackage)
        {
            int[] workpackageCode = new int[10];
            for (int i = 0; i < 10; i++) {
                workpackageCode[i] = -1;
            }
            workPackage.ProjectId = projectId;
            var workPackages = await _context.WorkPackages.Where(u => u.ProjectId == projectId).ToListAsync();
            var project = await _context.Projects.FirstOrDefaultAsync(m => m.ProjectId == projectId);

            foreach (WorkPackage tempWorkPackage in workPackages)
            {
                
                if (tempWorkPackage.WorkPackageCode.Length == PROJECT_CODE_LENGTH + 1) {
                    workpackageCode[Int32.Parse(tempWorkPackage.WorkPackageCode.Substring(PROJECT_CODE_LENGTH, 1))] = Int32.Parse(tempWorkPackage.WorkPackageCode);
                }
            }

            string theWorkpackageCode = null;

            for (int i = 0; i < 10; i++) {
                if (workpackageCode[i] == -1 && i != 0) {
                    theWorkpackageCode = (workpackageCode[i - 1] + 1).ToString();
                    break;
                }

                if (workpackageCode[i] == -1 && i == 0) {
                    theWorkpackageCode = project.ProjectCode + "0";
                    break;
                }
            }

            workPackage.WorkPackageCode = theWorkpackageCode;
            if (ModelState.IsValid)
            {
                _context.Add(workPackage);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(ProjectWorkPackges), new { id = projectId });
            }
            //ViewData["ParentWorkPackageId"] = new SelectList(_context.WorkPackages, "WorkPackageId", "WorkPackageId", workPackage.ParentWorkPackageId);
            //ViewData["ProjectId"] = new SelectList(_context.Projects, "ProjectId", "ProjectId", workPackage.ProjectId);
            return View(workPackage);
        }

        // GET: WorkPackages/CreateChildWorkPackage/6
        public IActionResult CreateChildWorkPackage(int? id)
        {
            parentWorkPKId = id;
            TempData["projectId"] = projectId;
            return View();
        }

        // POST: WorkPackages/CreateChildWorkPackage
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateChildWorkPackage([Bind("WorkPackageId,WorkPackageCode,Name,Description,Contractor,Purpose,Input,Output,Activity,ProjectId,ParentWorkPackageId")] WorkPackage workPackage)
        {
            int[] workpackageCode = new int[10];
            for (int i = 0; i < 10; i++)
            {
                workpackageCode[i] = -1;
            }
            workPackage.ProjectId = projectId;
            var workPackages = await _context.WorkPackages.Where(u => u.ProjectId == projectId).ToListAsync();
            var parentWorkPackage = await _context.WorkPackages.FindAsync(parentWorkPKId);
            var project = await _context.Projects.FirstOrDefaultAsync(m => m.ProjectId == projectId);
            foreach (WorkPackage tempWorkPackage in workPackages)
            {

                if (tempWorkPackage.ParentWorkPackageId == parentWorkPackage.WorkPackageId)
                {
                   // Debug.WriteLine("tempWorkPackage.WorkPackageCode----------" + tempWorkPackage.WorkPackageCode);
                    workpackageCode[Int32.Parse(tempWorkPackage.WorkPackageCode.Substring(parentWorkPackage.WorkPackageCode.Length, 1))] = Int32.Parse(tempWorkPackage.WorkPackageCode);
                }
            }

            string theWorkpackageCode = null;

            for (int i = 0; i < 10; i++)
            {
                if (workpackageCode[i] == -1 && i != 0)
                {
                    theWorkpackageCode = (workpackageCode[i - 1] + 1).ToString();
                    break;
                }

                if (workpackageCode[i] == -1 && i == 0)
                {
                    theWorkpackageCode = parentWorkPackage.WorkPackageCode + "0";
                    break;
                }
            }
             
            workPackage.ParentWorkPackageId = parentWorkPKId;
            workPackage.WorkPackageCode = theWorkpackageCode;
            if (ModelState.IsValid)
            {
                _context.Add(workPackage);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(ProjectWorkPackges), new { id = projectId });
            }
            //ViewData["ParentWorkPackageId"] = new SelectList(_context.WorkPackages, "WorkPackageId", "WorkPackageId", workPackage.ParentWorkPackageId);
            //ViewData["ProjectId"] = new SelectList(_context.Projects, "ProjectId", "ProjectId", workPackage.ProjectId);
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
            projectId = id;
            var project = await _context.Projects.FirstOrDefaultAsync(m => m.ProjectId == projectId);

            ViewData["projectCode"] = project.ProjectCode;
            ViewData["projectName"] = project.Name;

            if (id == null)
            {
                return NotFound();
            }


            var workPackages = await _context.WorkPackages
             .Where(u => u.ProjectId == id).ToListAsync();
            //order the workpackages
            workPackages = workPackages.OrderBy(u => u.WorkPackageCode).ToList();

            int maxWorkPackageCodeLength = 0;
            
            //get the max length of the workpackage code
            foreach (WorkPackage tempWorkPackage in workPackages) {
                if (tempWorkPackage.WorkPackageCode.Length > maxWorkPackageCodeLength) {
                    maxWorkPackageCodeLength = tempWorkPackage.WorkPackageCode.Length;
                }
            }

            var tempWorkPackages = new List<WorkPackage>();

            //add the root workpackages to tempWorkPackages
            foreach (WorkPackage tempWorkPackage in workPackages)
            {
                if (tempWorkPackage.WorkPackageCode.Length == (PROJECT_CODE_LENGTH + 1))
                {
                    tempWorkPackages.Add(tempWorkPackage);
                }
            }

            //put children workpackages under parents workpackages          
            for (int i = 0; i < maxWorkPackageCodeLength - PROJECT_CODE_LENGTH - 1; i++) {

                for (int n = 0; n < workPackages.Count; n++)
                {
                    //checkout the children workpackages
                    if (workPackages[n].WorkPackageCode.Length == PROJECT_CODE_LENGTH + i + 2) {
                        //compare children workpackagecode with other possible parents workpackagecode
                        for (int m = 0; m < tempWorkPackages.Count; m++) {
                            string tempCode1 = tempWorkPackages[m].WorkPackageCode;
                            string tempCode2 = workPackages[n].WorkPackageCode.Substring(0, PROJECT_CODE_LENGTH + i + 1);
                           
                            if (tempCode1.Equals(tempCode2)) {
                                tempWorkPackages.Insert(m + 1, workPackages[n]);
                            }                           
                        }
                    }
                }
            }
            
            workPackages = tempWorkPackages;

            //ViewData["NestedLevel"] = maxWorkPackageCodeLength - PROJECT_CODE_LENGTH;

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
