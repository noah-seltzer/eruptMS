using COMP4911Timesheets;
using COMP4911Timesheets.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using COMP4911Timesheets.Models;
using System.Net.Http;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace TimesheetsTest
{
    public class ProjectsControllerTest
    {
        //[Fact]
        //public async Task IndexReturnNotNullAsync()
        //{
        //    var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        //    optionsBuilder.UseInMemoryDatabase(databaseName: "db1");
        //    var _dbContext = new ApplicationDbContext(optionsBuilder.Options);

        //    var controller = new ProjectsController(_dbContext);
        //    var result = await controller.Index("Yipan");
        //    Assert.NotNull(result);
        //}

        //[Fact]
        //public async Task IndexShouldReturnViewResult()
        //{
        //    var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        //    optionsBuilder.UseInMemoryDatabase(databaseName: "db1");
        //    var _dbContext = new ApplicationDbContext(optionsBuilder.Options);

        //    var controller = new ProjectsController(_dbContext);
        //    var result = await controller.Index("Wu");
        //    Assert.IsType<ViewResult> (result);
        //}

        //[Fact]
        //public async Task Project1DetailReturnNotNull()
        //{
        //    var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        //    optionsBuilder.UseInMemoryDatabase(databaseName: "db2");
        //    var _dbContext = new ApplicationDbContext(optionsBuilder.Options);

        //    var controller = new ProjectsController(_dbContext);
        //    var result = await controller.Details(1);
        //    Assert.NotNull(result);
        //}

        //[Fact]
        //public void ProjectIdShouldNotBeNegativeNum()
        //{
        //    var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        //    optionsBuilder.UseInMemoryDatabase(databaseName: "db3");
        //    var _dbContext = new ApplicationDbContext(optionsBuilder.Options);

        //    ProjectsController controller = new ProjectsController(_dbContext);
        //    IActionResult result = controller.Details(-1) as IActionResult;
        //    Assert.Null(result);
        //}

        //[Fact]
        //public void Project0ShouldNotBeNull()
        //{
        //    var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        //    optionsBuilder.UseInMemoryDatabase(databaseName: "db3");
        //    var _dbContext = new ApplicationDbContext(optionsBuilder.Options);

        //    ProjectsController controller = new ProjectsController(_dbContext);
        //    IActionResult result = controller.Details(0) as IActionResult;
        //    Assert.Null(result);
        //}

        ////[Fact]
        ////public async Task EditProject1ViewReturnNotNullAsync()
        ////{
        ////    var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        ////    optionsBuilder.UseInMemoryDatabase(databaseName: "db4");
        ////    var _dbContext = new ApplicationDbContext(optionsBuilder.Options);

        ////    var controller = new ProjectsController(_dbContext);
        ////    var result = await controller.Edit(2);
        ////    Assert.NotNull(result);
        ////}

        //[Fact]
        //public async Task DeleteFuntionTest()
        //{
        //    var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        //    optionsBuilder.UseInMemoryDatabase(databaseName: "db4");
        //    var _dbContext = new ApplicationDbContext(optionsBuilder.Options);

        //    var controller = new ProjectsController(_dbContext);
        //    var result = await controller.Delete(1);
        //    Assert.NotNull(result);
        //}

        ////[Fact]
        ////public async System.Threading.Tasks.Task DeleteConfirmedFuntionTest()
        ////{
        ////    var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        ////    optionsBuilder.UseInMemoryDatabase(databaseName: "db4");
        ////    var _dbContext = new ApplicationDbContext(optionsBuilder.Options);

        ////    var controller = new ProjectsController(_dbContext);
        ////    var result = await controller.DeleteConfirmed(1);
        ////    Assert.NotNull(result);
        ////}

    }
}
