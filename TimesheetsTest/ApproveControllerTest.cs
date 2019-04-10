using COMP4911Timesheets.Controllers;
using COMP4911Timesheets.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Xunit;

namespace TimesheetsTest
{
    public class ApproveControllerTest
    {
//         [Fact]
//         public async Task IndexReturnNotNull()
//         {
//             var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
//             optionsBuilder.UseInMemoryDatabase(databaseName: "db1");
//             var _dbContext = new ApplicationDbContext(optionsBuilder.Options);

//             var controller = new ApproveController(_dbContext);
//             var result = await controller.Index();           
//             Assert.NotNull(result);
//         }

//         [Fact]
//         public async Task IndexReturnTypeTest()
//         {
//             var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
//             optionsBuilder.UseInMemoryDatabase(databaseName: "db1");
//             var _dbContext = new ApplicationDbContext(optionsBuilder.Options);

//             var controller = new ApproveController(_dbContext);
//             var result = await controller.Index();
//             Assert.IsType<ViewResult>(result);
//         }

//         [Fact]
//         public async Task DetailsTest()
//         {
//             var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
//             optionsBuilder.UseInMemoryDatabase(databaseName: "db1");
//             var _dbContext = new ApplicationDbContext(optionsBuilder.Options);

//             var controller = new ApproveController(_dbContext);
//             var result = await controller.Details("1");
//             Assert.NotNull(result);
//         }


//         [Fact]
//         public async Task TimesheetView1Test()
//         {
//             var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
//             optionsBuilder.UseInMemoryDatabase(databaseName: "db1");
//             var _dbContext = new ApplicationDbContext(optionsBuilder.Options);

//             var controller = new ApproveController(_dbContext);
//             var result = await controller.TimesheetView(1);
//             Assert.NotNull(result);
//         }

//         [Fact]
//         public async Task TimesheetViewInput0ReturnNull()
//         {
//             var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
//             optionsBuilder.UseInMemoryDatabase(databaseName: "db1");
//             var _dbContext = new ApplicationDbContext(optionsBuilder.Options);

//             var controller = new ApproveController(_dbContext);
//             var result = await controller.TimesheetView(0);
//             Assert.IsType<NotFoundResult>(result);
//         }

//         [Fact]
//         public async Task TimesheetViewInputNegNumberReturnNull()
//         {
//             var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
//             optionsBuilder.UseInMemoryDatabase(databaseName: "db1");
//             var _dbContext = new ApplicationDbContext(optionsBuilder.Options);

//             var controller = new ApproveController(_dbContext);
//             var result = await controller.TimesheetView(-2);
//             Assert.IsType<NotFoundResult>(result);
//         }

//         [Fact]
//         public async Task ApprovalIdZeroReturnIndex()
//         {
//             var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
//             optionsBuilder.UseInMemoryDatabase(databaseName: "db1");
//             var _dbContext = new ApplicationDbContext(optionsBuilder.Options);

//             var controller = new ApproveController(_dbContext);
//             var result = await controller.Approval(0) as RedirectToRouteResult;
//             var result1 = await controller.Approval(0);
//             //Assert.True(result.RouteValues.ContainsKey("action"));
//             Assert.Null(result);
//         }

//         [Fact]
//         public void ApprovalRedirectionTest()
//         {
//             var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
//             optionsBuilder.UseInMemoryDatabase(databaseName: "db1");
//             var _dbContext = new ApplicationDbContext(optionsBuilder.Options);

//             var controller = new ApproveController(_dbContext);
//             var result = controller.Approval(1);
//             Assert.NotNull(result);
//             //Assert.IsType<RedirectToActionResult>(result);
//         }

//         [Fact]
//         public async Task RejectTimesheetIdZeroRedirectionTest()
//         {
//             var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
//             optionsBuilder.UseInMemoryDatabase(databaseName: "db1");
//             var _dbContext = new ApplicationDbContext(optionsBuilder.Options);

//             var controller = new ApproveController(_dbContext);
//             var result = await controller.Reject(0);
//             Assert.IsType<NotFoundResult>(result);
//         }

//         [Fact]
//         public void RejectRedirectionTest()
//         {
//             var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
//             optionsBuilder.UseInMemoryDatabase(databaseName: "db1");
//             var _dbContext = new ApplicationDbContext(optionsBuilder.Options);

//             var controller = new ApproveController(_dbContext);
//             var result = controller.Reject(1);
//             Assert.NotNull(result);
//         }
    }
}
