using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using COMP4911Timesheets.Data;
using COMP4911Timesheets.Models;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace COMP4911Timesheets.Controllers
{
    [Authorize(Roles ="AD")]
    public sealed class WorkPackagesMap : ClassMap<WorkPackage>
    {
        public WorkPackagesMap()
        {
            Map(m => m.Name).Name("Name");
            Map(m => m.Description).Name("Description");
            Map(m => m.Contractor).Name("Contractor");
            Map(m => m.Purpose).Name("Purpose");
            Map(m => m.Input).Name("Input");
            Map(m => m.Output).Name("Output");
            Map(m => m.Activity).Name("Activity");
            Map(m => m.Status).Name("Status");
            Map(m => m.ParentWorkPackageId).Name("ParentWorkPackage");

        }
    }
    public class UploadsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IFileProvider fileProvider;
        private readonly IHostingEnvironment hostingEnvironment;
        private String fileContents;
        private readonly UserManager<Employee> _userManager;

        public UploadsController(
            ApplicationDbContext context,
            IFileProvider fileProvider,
            IHostingEnvironment environment,
            UserManager<Employee> userManager)
        {
            this._context = context;
            this.fileProvider = fileProvider;
            this.hostingEnvironment = environment;
            this._userManager = userManager;
        }
        public IActionResult Index()
        {
            ViewData["fileContents"] = fileContents ?? "none";
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                ViewData["FailureMessage"] = "Unrecognized Mime Type";
                return View("Failure");
            }
            //TODO remove these
            Guid guid = Guid.NewGuid();      // created guid
            var uploadedFileName =
                guid + Path.GetFileName(file.FileName);    // added guid to filename
            var path = Path.Combine(
                  hostingEnvironment.WebRootPath, "workpackages.csv");
            var failedList = new List<String>();
            var failedPackages = new List<Dictionary<String, String>>();
            var failedEmployees = new Dictionary<String, String>();
            var WPcontroller = new WorkPackagesController(_context, _userManager);
            using (var stream = new FileStream(path, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            using (var stream = new StreamReader(path))
            {
                using (var csv = new CsvReader(stream))
                {
                    //csvreader will substitute a null value instead of throwing an exception
                    csv.Configuration.MissingFieldFound = null;
                    csv.Read();
                    csv.ReadHeader();
                    while (csv.Read())
                    {
                        String[] fieldArray = new String[]
                                             { "ProjectCode", "Name", "Description", "Contractor",
                                                 "Purpose", "Input", "Output", "Activity",
                                                 "ParentWorkPackageCode", "Employees"};
                        var workPackageData =
                            new Dictionary<String, String>();
                        foreach (var field in fieldArray)
                        {
                            workPackageData.Add(field, csv.GetField(field) ?? "");
                        }
                        //If whoever's code reviewing know's a cleaner way to do this, please let me know
                        if (workPackageData["ProjectCode"] != null
                            && _context.Projects.Any(m =>
                                m.ProjectCode.Equals(workPackageData["ProjectCode"])))
                        {
                            var parentProject = await _context.Projects.Include(w => w.WorkPackages)
                                .FirstOrDefaultAsync(m =>
                                                m.ProjectCode == workPackageData["ProjectCode"]);

                            var package = new WorkPackage()
                            {
                                Name = workPackageData["Name"] ?? "",
                                Description = workPackageData["Description"] ?? "",
                                Contractor = workPackageData["Contractor"] ?? "",
                                Purpose = workPackageData["Description"] ?? "",
                                Input = workPackageData["Input"] ?? "",
                                Output = workPackageData["Output"] ?? "",
                                Activity = workPackageData["Activity"] ?? "",
                                Status = WorkPackage.OPENED,
                                ProjectId = parentProject.ProjectId
                            };
                            if (workPackageData["ParentWorkPackageCode"] != null && !workPackageData["ParentWorkPackageCode"].Equals(""))
                            {
                                try
                                {
                                    var parentWorkPackage = parentProject.WorkPackages
                                        .Where(m =>
                                            m.WorkPackageCode.Equals(workPackageData["ParentWorkPackageCode"]))
                                        .FirstOrDefault();
                                    package.ParentWorkPackage = parentWorkPackage;
                                    package.ParentWorkPackageId = parentWorkPackage.WorkPackageId;
                                    package.WorkPackageCode = await generateChildWorkPackageCode(parentWorkPackage);
                                }
                                catch (NullReferenceException e)
                                {
                                    Console.WriteLine(e.Message);
                                    failedPackages.Add(workPackageData);
                                    continue;
                                }
                            }
                            else
                            {
                                package.WorkPackageCode = await generateNonChildWorkPackageCode(parentProject);
                            }
                            if (ModelState.IsValid)
                            {
                                _context.Add(package);
                                await _context.SaveChangesAsync();


                                if (workPackageData["Employees"] != "")
                                {
                                    var emails = workPackageData["Employees"].Split(';');

                                    foreach (var email in emails)
                                    {
                                        Employee employee = await _userManager.Users.FirstOrDefaultAsync(m =>
                                                m.Email == email);
                                        if (employee != null)
                                        {
                                            var projectEmployee = new ProjectEmployee()
                                            {
                                                WorkPackageId = package.WorkPackageId,
                                                EmployeeId = employee.Id,
                                                ProjectId = parentProject.ProjectId
                                            };

                                            _context.Add(projectEmployee);
                                            await _context.SaveChangesAsync();

                                        }
                                        else
                                        {
                                            var packageData = string.Join(";", workPackageData.Select(x => x.Key + "=" + x.Value).ToArray());
                                            try
                                            {
                                                failedEmployees[packageData] += email;
                                            }
                                            catch (KeyNotFoundException ex)
                                            {
                                                Console.WriteLine(ex.Message);
                                                failedEmployees.Add(packageData, email);
                                            }
                                        }
                                    }

                                }
                            }
                            else
                            {
                                failedList.Add(workPackageData["Name"]);
                                failedPackages.Add(workPackageData);
                            }
                        }
                        else
                        {
                            failedList.Add(workPackageData["Name"]);
                            failedPackages.Add(workPackageData);
                        }
                    }
                }
            }
            //ViewData["failedPackages"] = String.Join(",", failedList);
            ViewData["failedPackages"] = "";
            ViewData["failedEmployees"] = "";
            foreach (var record in failedPackages)
            {
                ViewData["failedPackages"] += string.Join(";", record.Select(x => x.Key + "=" + x.Value).ToArray()) + "\n";
            }
            foreach (var record in failedEmployees)
            {
                ViewData["failedEmployees"] += record.Key + "\n" + record.Value + "\n";
            }

            return View("Success");
        }

        public IActionResult Files()
        {
            return View();
        }

        private string GetContentType(string path)
        {
            var types = GetMimeTypes();
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types[ext];
        }

        private Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
                {".txt", "text/plain"},
                {".pdf", "application/pdf"},
                {".doc", "application/vnd.ms-word"},
                {".docx", "application/vnd.ms-word"},
                {".xls", "application/vnd.ms-excel"},
                {".xlsx","application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" },
                {".png", "image/png"},
                {".jpg", "image/jpeg"},
                {".jpeg", "image/jpeg"},
                {".gif", "image/gif"},
                {".csv", "text/csv"}
            };
        }
        private async Task<String> generateChildWorkPackageCode(WorkPackage parentWorkPackage)
        {
            int[] workpackageCode = new int[10];
            for (int i = 0; i < 10; i++)
            {
                workpackageCode[i] = -1;
            }
            var workPackages = await _context.WorkPackages.Where(u => u.ProjectId == parentWorkPackage.ProjectId).ToListAsync();
            int workPackageLength = 0;

            foreach (WorkPackage tempWorkPackage in workPackages)
            {

                if (tempWorkPackage.ParentWorkPackageId == parentWorkPackage.WorkPackageId)
                {
                    workPackageLength = tempWorkPackage.WorkPackageCode.Length;
                    // Debug.WriteLine("tempWorkPackage.WorkPackageCode----------" + tempWorkPackage.WorkPackageCode);
                    workpackageCode[Int32.Parse(tempWorkPackage.WorkPackageCode.Substring(parentWorkPackage.WorkPackageCode.Length, 1))] = Int32.Parse(tempWorkPackage.WorkPackageCode);
                }
            }

            string theWorkpackageCode = null;

            for (int i = 0; i < 10; i++)
            {
                if (workpackageCode[i] == -1 && i != 0)
                {
                    theWorkpackageCode = ((Math.Pow(10, workPackageLength)) + workpackageCode[i - 1] + 1).ToString();
                    theWorkpackageCode = theWorkpackageCode.Substring(1);
                    break;
                }

                if (workpackageCode[i] == -1 && i == 0)
                {
                    theWorkpackageCode = parentWorkPackage.WorkPackageCode + "0";
                    break;
                }
            }

            return theWorkpackageCode;
        }
        private async Task<String> generateNonChildWorkPackageCode(Project parentProject)
        {
            int[] workpackageCode = new int[10];
            for (int i = 0; i < 10; i++)
            {
                workpackageCode[i] = -1;
            }
            int workPackageLength = 0;
            var projectPackages = parentProject.WorkPackages;
            foreach (WorkPackage tempWorkPackage in projectPackages)
            {

                if (tempWorkPackage.WorkPackageCode.Length == WorkPackagesController.PROJECT_CODE_LENGTH + 1)
                {
                    workPackageLength = tempWorkPackage.WorkPackageCode.Length;
                    workpackageCode[Int32.Parse(tempWorkPackage.WorkPackageCode.Substring(WorkPackagesController.PROJECT_CODE_LENGTH, 1))] = Int32.Parse(tempWorkPackage.WorkPackageCode);
                }
            }
            string theWorkpackageCode = null;

            for (int i = 0; i < 10; i++)
            {
                if (workpackageCode[i] == -1 && i != 0)
                {
                    theWorkpackageCode = ((Math.Pow(10, workPackageLength)) + workpackageCode[i - 1] + 1).ToString();
                    theWorkpackageCode = theWorkpackageCode.Substring(1);
                    break;
                }

                if (workpackageCode[i] == -1 && i == 0)
                {
                    theWorkpackageCode = parentProject.ProjectCode + "0";
                    break;
                }
            }

            return theWorkpackageCode;
        }
    }
}
