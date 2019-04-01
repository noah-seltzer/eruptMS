using COMP4911Timesheets.Controllers;
using COMP4911Timesheets.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Xunit;

namespace TimesheetsTest
{
    public class BudgetsComtrollerTest
    {
        [Fact]
        public async Task IndexReturnNotNull()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseInMemoryDatabase(databaseName: "db1");
            var _dbContext = new ApplicationDbContext(optionsBuilder.Options);

            var controller = new BudgetsController(_dbContext);
            var result = await controller.Index();
            Assert.IsType<ViewResult>(result);
            Assert.NotNull(result);
        }

        [Fact]
        public async Task IndexReturnTypeTest()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseInMemoryDatabase(databaseName: "db1");
            var _dbContext = new ApplicationDbContext(optionsBuilder.Options);

            var controller = new BudgetsController(_dbContext);
            var result = await controller.Index();
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task DetailsNullInputTest()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseInMemoryDatabase(databaseName: "db1");
            var _dbContext = new ApplicationDbContext(optionsBuilder.Options);

            var controller = new BudgetsController(_dbContext);
            var result = await controller.Details(null);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DetailsViewReturnTest()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseInMemoryDatabase(databaseName: "db1");
            var _dbContext = new ApplicationDbContext(optionsBuilder.Options);

            var controller = new BudgetsController(_dbContext);
            var result = await controller.Details(1);
            Assert.NotNull(result);
        }

        //[Fact]
        //public async Task CreateViewReturnTest()
        //{
        //    var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        //    optionsBuilder.UseInMemoryDatabase(databaseName: "db1");
        //    var _dbContext = new ApplicationDbContext(optionsBuilder.Options);

        //    var controller = new BudgetsController(_dbContext);
        //    var result = await controller.Create(1);
        //    Assert.NotNull(result);
        //}

        [Fact]
        public async Task EditNullInputTest()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseInMemoryDatabase(databaseName: "db1");
            var _dbContext = new ApplicationDbContext(optionsBuilder.Options);

            var controller = new BudgetsController(_dbContext);
            var result = await controller.Edit(null);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task EditValidInputTest()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseInMemoryDatabase(databaseName: "db1");
            var _dbContext = new ApplicationDbContext(optionsBuilder.Options);

            var controller = new BudgetsController(_dbContext);
            var result = await controller.Edit(1);
            Assert.NotNull(result);
        }

        [Fact]
        public async Task DeleteNullInputTest()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseInMemoryDatabase(databaseName: "db1");
            var _dbContext = new ApplicationDbContext(optionsBuilder.Options);

            var controller = new BudgetsController(_dbContext);
            var result = await controller.Delete(null);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteValidInputTest()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseInMemoryDatabase(databaseName: "db1");
            var _dbContext = new ApplicationDbContext(optionsBuilder.Options);

            var controller = new BudgetsController(_dbContext);
            var result = await controller.Delete(1);
            Assert.NotNull(result);
        }
    }
}
