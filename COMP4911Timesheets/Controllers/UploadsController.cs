using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using COMP4911Timesheets.Data;
using COMP4911Timesheets.Models;
using CsvHelper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace COMP4911Timesheets.Controllers
{
    public class UploadsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IFileProvider fileProvider;
        private readonly IHostingEnvironment hostingEnvironment;
        private String fileContents;

        public UploadsController(
            ApplicationDbContext context, 
            IFileProvider fileProvider, 
            IHostingEnvironment environment)
        {
            this._context = context;
            this.fileProvider = fileProvider;
            this.hostingEnvironment = environment;
        }
        public IActionResult Index()
        {
            ViewData["fileContents"] = fileContents ?? "none";
            return View();
        }

        
        [HttpPost]
        public IActionResult UploadFile(IFormFile file)
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

            using (var stream = new FileStream(path, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            using (var stream = new StreamReader(path))
            {
                using (var csv = new CsvReader(stream))
                {
                    //var temporaryPackage = new WorkPackage();
                    //var records = csv.EnumerateRecords(temporaryPackage);
                    //foreach(var package in records)
                    //{
                    //    if (package.ParentWorkPackageId == Null)
                    //    {

                    //    }

                    //}
                    csv.Configuration.MissingFieldFound = null;
                    csv.Read();
                    csv.ReadHeader();
                    while(csv.Read())
                    {
                        
                        var id = csv.GetField<int>("Id");
                        var field = csv.GetField("Name") ?? "none";


                    }
                    
                }
            }

            //var workPackages = parseCsvFromString( (String) ViewData["fileContents"]);
            return View("Success");
        }

        //public Object parseCsvFromString(String csvData)
        //{

        //    return null;
        //}

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
                {".xlsx",
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" },
                {".png", "image/png"},
                {".jpg", "image/jpeg"},
                {".jpeg", "image/jpeg"},
                {".gif", "image/gif"},
                {".csv", "text/csv"}
            };
        }
    }
}

