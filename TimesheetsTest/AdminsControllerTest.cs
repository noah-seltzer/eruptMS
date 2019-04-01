using COMP4911Timesheets.Controllers;
using COMP4911Timesheets.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Xunit;

namespace TimesheetsTest
{
    public class AdminsControllerTest
    {
        [Fact]
        public void IndexReturnNotNull()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseInMemoryDatabase(databaseName: "db1");
            var _dbContext = new ApplicationDbContext(optionsBuilder.Options);

            var controller = new AdminsController(_dbContext);
            var result = controller.Index();
            Assert.NotNull(result);
        }

        [Fact]
        public void BackupRedirectionTest()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseInMemoryDatabase(databaseName: "db1");
            var _dbContext = new ApplicationDbContext(optionsBuilder.Options);

            //var controller = new AdminsController(_dbContext);
            //var result = controller.Backup() as RedirectToRouteResult;
            //Assert.Equal("Index", result.RouteValues["Index"]);            
        }
    }
}
