using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using smarth_health.WebApi.Controllers;
using smarth_health.WebApi.Models;
using smarth_health.WebApi.Repositories;

namespace smarth_health.Tests
{
    [TestClass]
    public sealed class DairyControllerTests
    {
        private Mock<IDairyRepository> _dairyRepository;
        private DairyController _controller;

        [TestInitialize]
        public void Setup()
        {
            _dairyRepository = new Mock<IDairyRepository>();
            _controller = new DairyController(_dairyRepository.Object);
        }

        [TestMethod]
        public async Task GetAllDairies_ReturnsOkObjectResult_WithListOfDairies()
        {
            // ARRANGE
            var dairies = new List<Dairy>
            {
                new Dairy { Id = Guid.NewGuid() },
                new Dairy { Id = Guid.NewGuid() }
            };
            _dairyRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(dairies);

            // ACT
            var result = await _controller.GetAllDairies();

            // ASSERT
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult, "Expected OkObjectResult");
            Assert.AreEqual(StatusCodes.Status200OK, okResult.StatusCode);
            Assert.AreEqual(dairies, okResult.Value);
        }

        [TestMethod]
        public async Task GetDairy_ExistingDairy_ReturnsOkObjectResult()
        {
            // ARRANGE
            var id = Guid.NewGuid();
            var user = new Dairy { UserId = id };
            _dairyRepository.Setup(repo => repo.GetByUserIdAsync(id)).ReturnsAsync(user);

            // ACT
            var result = await _controller.GetDairyByUserId(id);

            // ASSERT
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult, "Expected OkObjectResult");
            Assert.AreEqual(StatusCodes.Status200OK, okResult.StatusCode);
            Assert.AreEqual(user, okResult.Value);
        }

        [TestMethod]
        public async Task GetDairy_NonExistingDairy_ReturnsNotFoundResult()
        {
            // ARRANGE
            var id = Guid.NewGuid();
            _dairyRepository.Setup(repo => repo.GetByUserIdAsync(id)).ReturnsAsync((Dairy)null);

            // ACT
            var result = await _controller.GetDairyByUserId(id);

            // ASSERT
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task CreateDairy_ValidDairy_ReturnsCreatedAtActionResult()
        {
            // ARRANGE
            var dairy = new Dairy(); // Initially no Id assigned.
            _dairyRepository.Setup(repo => repo.CreateAsync(It.IsAny<Dairy>()))
                           .Returns(Task.CompletedTask);

            // ACT
            var result = await _controller.CreateDairy(dairy);

            // ASSERT
            var createdResult = result as CreatedAtActionResult;
            Assert.IsNotNull(createdResult, "Expected CreatedAtActionResult");
            Assert.AreEqual(nameof(_controller.GetDairyByUserId), createdResult.ActionName);

            var returnedDairy = createdResult.Value as Dairy;
            Assert.IsNotNull(returnedDairy, "Returned dairy should not be null.");
            Assert.AreNotEqual(Guid.Empty, returnedDairy.Id, "Expected dairy Id to be assigned a new Guid.");

            _dairyRepository.Verify(repo => repo.CreateAsync(It.IsAny<Dairy>()), Times.Once);
        }

        [TestMethod]
        public async Task CreateDairy_InvalidModelState_ReturnsBadRequest()
        {
            // ARRANGE
            var dairy = new Dairy();
            _controller.ModelState.AddModelError("error", "Invalid dairy");

            // ACT
            var result = await _controller.CreateDairy(dairy);

            // ASSERT
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task UpdateDairy_IdMismatch_ReturnsBadRequest()
        {
            // ARRANGE
            var id = Guid.NewGuid();
            var dairy = new Dairy { Id = Guid.NewGuid() };

            // ACT
            var result = await _controller.UpdateDairy(id, dairy);

            // ASSERT
            Assert.IsInstanceOfType(result, typeof(BadRequestResult));
        }

        [TestMethod]
        public async Task UpdateDairy_NonExistingDairy_ReturnsNotFoundResult()
        {
            // ARRANGE
            var id = Guid.NewGuid();
            var dairy = new Dairy { Id = id };

            _dairyRepository.Setup(repo => repo.GetByDairyIdAsync(id))
                           .ReturnsAsync((Dairy)null);

            // ACT
            var result = await _controller.UpdateDairy(id, dairy);

            // ASSERT
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task UpdateDairy_ValidDairy_ReturnsNoContentResult()
        {
            // ARRANGE
            var id = Guid.NewGuid();
            var dairy = new Dairy { Id = id };

            _dairyRepository.Setup(repo => repo.GetByDairyIdAsync(id))
                           .ReturnsAsync(dairy);
            _dairyRepository.Setup(repo => repo.UpdateAsync(dairy))
                           .Returns(Task.CompletedTask);

            // ACT
            var result = await _controller.UpdateDairy(id, dairy);

            // ASSERT
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
            _dairyRepository.Verify(repo => repo.UpdateAsync(dairy), Times.Once);
        }

        [TestMethod]
        public async Task DeleteDairy_NonExistingDairy_ReturnsNotFoundResult()
        {
            // ARRANGE
            var id = Guid.NewGuid();
            _dairyRepository.Setup(repo => repo.GetByDairyIdAsync(id))
                           .ReturnsAsync((Dairy)null);

            // ACT
            var result = await _controller.DeleteDairy(id);

            // ASSERT
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task DeleteDairy_ExistingDairy_ReturnsNoContentResult()
        {
            // ARRANGE
            var id = Guid.NewGuid();
            var dairy = new Dairy { Id = id };

            _dairyRepository.Setup(repo => repo.GetByDairyIdAsync(id))
                           .ReturnsAsync(dairy);
            _dairyRepository.Setup(repo => repo.DeleteAsync(id))
                           .Returns(Task.CompletedTask);

            // ACT
            var result = await _controller.DeleteDairy(id);

            // ASSERT
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
            _dairyRepository.Verify(repo => repo.DeleteAsync(id), Times.Once);
        }
    }
}
