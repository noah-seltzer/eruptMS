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

            for (int i = 0; i < budgets.Count; i++) {
                budgets[i].PayGrade = await _context.PayGrades.FirstOrDefaultAsync(m => m.PayGradeId == budgets[i].PayGradeId);
            }

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
        public async Task<IActionResult> Create([Bind("WorkPackageId,WorkPackageCode,Name,Description,Contractor,Purpose,Input,Output,Activity,Status,ProjectId,ParentWorkPackageId")] WorkPackage workPackage)
        {
            int[] workpackageCode = new int[10];
            for (int i = 0; i < 10; i++) {
                workpackageCode[i] = -1;
            }
            workPackage.ProjectId = projectId;
            workPackage.Status = 2;
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
        public async Task<IActionResult> CreateChildWorkPackage([Bind("WorkPackageId,WorkPackageCode,Name,Description,Contractor,Purpose,Input,Output,Activity,Status,ProjectId,ParentWorkPackageId")] WorkPackage workPackage)
        {
            int[] workpackageCode = new int[10];
            for (int i = 0; i < 10; i++)
            {
                workpackageCode[i] = -1;
            }
            workPackage.ProjectId = projectId;
            workPackage.Status = 2;
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


        // GET: WorkPackages/CreateWorkPackage/6
        public async Task<IActionResult> CreateWorkPackageReport(int? id)
        {
            var workPackages = await _context.WorkPackages.FirstOrDefaultAsync(m => m.ParentWorkPackageId == id);
            if (workPackages == null)
            {
                var theWorkPackage = await _context.WorkPackages.FindAsync(id);
                WorkPackageReport workPackageReport = new WorkPackageReport
                {
                    WorkPackage = theWorkPackage,
                    WeekNumber = Utility.GetWeekNumberByDate(DateTime.Today)
                };
                return View(workPackageReport);
            }

            TempData["info"] = "Workpackage report only can be created on leaf workpackages";
            var wpTemp = await _context.WorkPackages.FirstOrDefaultAsync(m => m.WorkPackageId == id);
            return RedirectToAction("ProjectWorkPackges", "WorkPackages", new { id = wpTemp.ProjectId });
     
        }

        // POST: WorkPackageReports/CreateWorkPackageReport
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateWorkPackageReport([Bind("WorkPackageReportId,WeekNumber,Status,Comments,StartingPercentage,CompletedPercentage,CostStarted,CostFinished,WorkAccomplished,WorkAccomplishedNP,Problem,ProblemAnticipated,WorkPackageId")] WorkPackageReport workPackageReport)
        {
            workPackageReport.Status = 1;
            if (ModelState.IsValid)
            {
                _context.Add(workPackageReport);
                await _context.SaveChangesAsync();
                //return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(ProjectWorkPackges), new { id = projectId });
        }


        // GET: WorkPackages/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workPackage = await _context.WorkPackages.FindAsync(id);

            if (workPackage.Status == 3)
            {
                TempData["info"] = "Workpackage already closed you can not change  work package info";
                return RedirectToAction("ProjectWorkPackges", "WorkPackages", new { id = projectId });
            }

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
        public async Task<IActionResult> Edit(int id, [Bind("WorkPackageId,WorkPackageCode,Name,Description,Contractor,Purpose,Input,Output,Activity,Status,ProjectId,ParentWorkPackageId")] WorkPackage workPackage)
        {
            if (id != workPackage.WorkPackageId)
            {
                return NotFound();
            }


            if (ModelState.IsValid)
            {
                try
                {
                    if (workPackage.Status == 3 || workPackage.Status == 4)
                    {
                        var workPackages = await _context.WorkPackages.Where(u => u.WorkPackageCode.StartsWith(workPackage.WorkPackageCode)).ToListAsync();
                        foreach (WorkPackage tempWorkPackage in workPackages)
                        {
                            if (tempWorkPackage.Status != 3)
                            {
                                tempWorkPackage.Status = workPackage.Status;
                                _context.Update(tempWorkPackage);
                                await _context.SaveChangesAsync();
                            }
                        }
                    }
                    else {
                        _context.Update(workPackage);
                        await _context.SaveChangesAsync();
                    }
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
                return RedirectToAction("ProjectWorkPackges", "WorkPackages", new { id = projectId });
            }
            ViewData["ParentWorkPackageId"] = new SelectList(_context.WorkPackages, "WorkPackageId", "WorkPackageId", workPackage.ParentWorkPackageId);
            ViewData["ProjectId"] = new SelectList(_context.Projects, "ProjectId", "ProjectId", workPackage.ProjectId);
            return View(workPackage);
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
             .Where(u => u.ProjectId == id && u.Status != 3).ToListAsync();
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

        //GET: ProjectWorkPackges/ClosedWorkPackageInfo/5
        public async Task<IActionResult> ClosedWorkPackageInfo()
        {
            var project = await _context.Projects.FirstOrDefaultAsync(m => m.ProjectId == projectId);

            ViewData["projectCode"] = project.ProjectCode;
            ViewData["projectName"] = project.Name;
            ViewData["projectId"] = projectId;
            if (projectId == null)
            {
                return NotFound();
            }


            var workPackages = await _context.WorkPackages
             .Where(u => u.ProjectId == projectId && u.Status == 3).ToListAsync();

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
