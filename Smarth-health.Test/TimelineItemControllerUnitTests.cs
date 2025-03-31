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
    public sealed class TimeLineItemsControllerTests
    {
        private Mock<ITimeLineItemRepository> _repository;
        private TimeLineItemsController _controller;

        [TestInitialize]
        public void Setup()
        {
            _repository = new Mock<ITimeLineItemRepository>();
            _controller = new TimeLineItemsController(_repository.Object);
        }

        [TestMethod]
        public async Task GetAll_ReturnsOkObjectResult_WithListOfItems()
        {
            // ARRANGE
            var items = new List<TimeLineItem>
            {
                new() { ID = Guid.NewGuid(), Content = "Content1", ToolTipContent = "ToolTip1", Position = "Position1" },
                new() { ID = Guid.NewGuid(), Content = "Content2", ToolTipContent = "ToolTip2", Position = "Position2" }
            };
            _repository.Setup(r => r.GetAllAsync()).ReturnsAsync(items);

            // ACT
            var result = await _controller.GetAll();

            // ASSERT
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult, "Expected an OkObjectResult.");
            Assert.AreEqual(StatusCodes.Status200OK, okResult.StatusCode);
            Assert.AreEqual(items, okResult.Value);
        }

        [TestMethod]
        public async Task GetById_ItemExists_ReturnsOkObjectResult()
        {
            // ARRANGE
            var id = Guid.NewGuid();
            var item = new TimeLineItem { ID = id, Content = "Content", ToolTipContent = "ToolTip", Position = "Position" };
            _repository.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(item);

            // ACT
            var result = await _controller.GetById(id);

            // ASSERT
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult, "Expected an OkObjectResult.");
            Assert.AreEqual(StatusCodes.Status200OK, okResult.StatusCode);
            Assert.AreEqual(item, okResult.Value);
        }

        [TestMethod]
        public async Task GetById_ItemDoesNotExist_ReturnsNotFoundResult()
        {
            // ARRANGE
            var id = Guid.NewGuid();
            _repository.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((TimeLineItem)null);

            // ACT
            var result = await _controller.GetById(id);

            // ASSERT
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Create_ValidItem_ReturnsCreatedAtActionResult()
        {
            // ARRANGE
            var item = new TimeLineItem { Content = "Content", ToolTipContent = "ToolTip", Position = "Position" }; // Initially no ID assigned.
            _repository.Setup(r => r.CreateAsync(It.IsAny<TimeLineItem>()))
                .Returns(Task.CompletedTask);

            // ACT
            var result = await _controller.Create(item);

            // ASSERT
            var createdResult = result as CreatedAtActionResult;
            Assert.IsNotNull(createdResult, "Expected a CreatedAtActionResult.");
            Assert.AreEqual(nameof(TimeLineItemsController.GetById), createdResult.ActionName);

            var returnedItem = createdResult.Value as TimeLineItem;
            Assert.IsNotNull(returnedItem, "Returned item should not be null.");
            Assert.AreNotEqual(Guid.Empty, returnedItem.ID, "A new Guid should be assigned.");

            _repository.Verify(r => r.CreateAsync(It.IsAny<TimeLineItem>()), Times.Once);
        }

        [TestMethod]
        public async Task Update_IdMismatch_ReturnsBadRequest()
        {
            // ARRANGE
            var id = Guid.NewGuid();
            var item = new TimeLineItem { ID = Guid.NewGuid(), Content = "Content", ToolTipContent = "ToolTip", Position = "Position" };

            // ACT
            var result = await _controller.Update(id, item);

            // ASSERT
            Assert.IsInstanceOfType(result, typeof(BadRequestResult));
        }

        [TestMethod]
        public async Task Update_ItemDoesNotExist_ReturnsNotFoundResult()
        {
            // ARRANGE
            var id = Guid.NewGuid();
            var item = new TimeLineItem { ID = id, Content = "Content", ToolTipContent = "ToolTip", Position = "Position" };

            _repository.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((TimeLineItem)null);

            // ACT
            var result = await _controller.Update(id, item);

            // ASSERT
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Update_ValidItem_ReturnsNoContentResult()
        {
            // ARRANGE
            var id = Guid.NewGuid();
            var item = new TimeLineItem { ID = id, Content = "Content", ToolTipContent = "ToolTip", Position = "Position" };

            _repository.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(item);
            _repository.Setup(r => r.UpdateAsync(item)).Returns(Task.CompletedTask);

            // ACT
            var result = await _controller.Update(id, item);

            // ASSERT
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
            _repository.Verify(r => r.UpdateAsync(item), Times.Once);
        }

        [TestMethod]
        public async Task Delete_ItemDoesNotExist_ReturnsNotFoundResult()
        {
            // ARRANGE
            var id = Guid.NewGuid();
            _repository.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((TimeLineItem)null);

            // ACT
            var result = await _controller.Delete(id);

            // ASSERT
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Delete_ExistingItem_ReturnsNoContentResult()
        {
            // ARRANGE
            var id = Guid.NewGuid();
            var item = new TimeLineItem { ID = id, Content = "Content", ToolTipContent = "ToolTip", Position = "Position" };

            _repository.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(item);
            _repository.Setup(r => r.DeleteAsync(id)).Returns(Task.CompletedTask);

            // ACT
            var result = await _controller.Delete(id);

            // ASSERT
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
            _repository.Verify(r => r.DeleteAsync(id), Times.Once);
        }
    }
}
