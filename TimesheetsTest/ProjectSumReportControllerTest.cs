using COMP4911Timesheets.Controllers;
using COMP4911Timesheets.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Xunit;

namespace TimesheetsTest
{
    public class ProjectSumReportControllerTest
    {
        //[Fact]
        //public async Task IndexReturnNotNull()
        //{
        //    var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        //    optionsBuilder.UseInMemoryDatabase(databaseName: "db1");
        //    var _dbContext = new ApplicationDbContext(optionsBuilder.Options);

        //    var controller = new ProjectSumReportController(_dbContext);
        //    var result = await controller.Index();
        //    Assert.IsType<ViewResult>(result);
        //    Assert.NotNull(result);
        //}

        //[Fact]
        //public async Task IndexReturnTypeTest()
        //{
        //    var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        //    optionsBuilder.UseInMemoryDatabase(databaseName: "db1");
        //    var _dbContext = new ApplicationDbContext(optionsBuilder.Options);

        //    var controller = new ProjectSumReportController(_dbContext);
        //    var result = await controller.Index();
        //    Assert.IsType<ViewResult>(result);
        //}

        //[Fact]
        //public async Task ReportZeroShouldReturnNotFound()
        //{
        //    var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        //    optionsBuilder.UseInMemoryDatabase(databaseName: "db1");
        //    var _dbContext = new ApplicationDbContext(optionsBuilder.Options);

        //    var controller = new ProjectSumReportController(_dbContext);
        //    var result = await controller.Report(0);
        //    Assert.IsType<NotFoundResult>(result);
        //}

        ////[Fact]
        ////public async Task EditZeroShouldReturnNotFound()
        ////{
        ////    var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        ////    optionsBuilder.UseInMemoryDatabase(databaseName: "db1");
        ////    var _dbContext = new ApplicationDbContext(optionsBuilder.Options);

        ////    var controller = new ProjectSumReportController(_dbContext);
        ////    var result = await controller.Report(1);
        ////    Assert.IsType<NotFoundResult>(result);
        ////}
    }
}
