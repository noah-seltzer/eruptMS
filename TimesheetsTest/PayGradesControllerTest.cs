using COMP4911Timesheets.Controllers;
using COMP4911Timesheets.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Xunit;

namespace TimesheetsTest
{
    public class PayGradesControllerTest
    {
        [Fact]
        public async Task IndexReturnNotNull()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseInMemoryDatabase(databaseName: "db1");
            var _dbContext = new ApplicationDbContext(optionsBuilder.Options);

            var controller = new PayGradesController(_dbContext);
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

            var controller = new PayGradesController(_dbContext);
            var result = await controller.Index();
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void CreateReturnTypeTest()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseInMemoryDatabase(databaseName: "db1");
            var _dbContext = new ApplicationDbContext(optionsBuilder.Options);

            var controller = new PayGradesController(_dbContext);
            var result = controller.Create();
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task EditZeroShouldReturnNotFound()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseInMemoryDatabase(databaseName: "db1");
            var _dbContext = new ApplicationDbContext(optionsBuilder.Options);

            var controller = new PayGradesController(_dbContext);
            var result = await controller.Edit(0);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task EditInvalidInputTest()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseInMemoryDatabase(databaseName: "db1");
            var _dbContext = new ApplicationDbContext(optionsBuilder.Options);

            var controller = new PayGradesController(_dbContext);
            var result = await controller.Edit(-3);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task EditValidInputTest()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseInMemoryDatabase(databaseName: "db1");
            var _dbContext = new ApplicationDbContext(optionsBuilder.Options);

            var controller = new PayGradesController(_dbContext);
            var result = await controller.Edit(1);
            Assert.NotNull(result);
        }

        
    }
}
