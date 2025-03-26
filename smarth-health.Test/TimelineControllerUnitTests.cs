using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using smarth_health.WebApi.Controllers;
using smarth_health.WebApi.Models;
using smarth_health.WebApi.Repositories;

namespace smarth_health.Tests
{
    [TestClass]
    public sealed class TimelineControllerTests
    {
        private Mock<ITimelineRepository> _timelineRepository;
        private TimelineController _controller;

        [TestInitialize]
        public void Setup()
        {
            _timelineRepository = new Mock<ITimelineRepository>();
            _controller = new TimelineController(_timelineRepository.Object);
        }

        [TestMethod]
        public async Task CreateTimeline_ValidTimeline_ReturnsCreatedAtActionResult()
        {
            // ARRANGE
            var timeline = new Timeline(); // Add any required properties if needed

            // ACT
            var result = await _controller.CreateTimeline(timeline);

            // ASSERT
            var createdResult = result as CreatedAtActionResult;
            Assert.IsNotNull(createdResult, "Expected CreatedAtActionResult");
            // The action name should be GetTimeline as defined in the controller.
            Assert.AreEqual(nameof(TimelineController.GetTimeline), createdResult.ActionName);

            var returnedTimeline = createdResult.Value as Timeline;
            Assert.IsNotNull(returnedTimeline, "Returned timeline should not be null.");
            // Verify that the controller has assigned a new Guid.
            Assert.AreNotEqual(Guid.Empty, returnedTimeline.ID);

            // Verify that the repository's InsertAsync method was called once with the timeline.
            _timelineRepository.Verify(r => r.InsertAsync(It.Is<Timeline>(t => t.ID == returnedTimeline.ID)), Times.Once);
        }

        [TestMethod]
        public async Task GetTimeline_ExistingTimeline_ReturnsOkObjectResult()
        {
            // ARRANGE
            var timelineId = Guid.NewGuid();
            var timeline = new Timeline { ID = timelineId };
            _timelineRepository.Setup(r => r.ReadAsync(timelineId)).ReturnsAsync(timeline);

            // ACT
            var result = await _controller.GetTimeline(timelineId);

            // ASSERT
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult, "Expected OkObjectResult");
            Assert.AreEqual(timeline, okResult.Value);
        }

        [TestMethod]
        public async Task GetTimeline_NonExistingTimeline_ReturnsNotFoundResult()
        {
            // ARRANGE
            var timelineId = Guid.NewGuid();
            _timelineRepository.Setup(r => r.ReadAsync(timelineId)).ReturnsAsync((Timeline)null);

            // ACT
            var result = await _controller.GetTimeline(timelineId);

            // ASSERT
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task UpdateTimeline_IdMismatch_ReturnsBadRequest()
        {
            // ARRANGE
            var timeline = new Timeline { ID = Guid.NewGuid() };
            var differentId = Guid.NewGuid();

            // ACT
            var result = await _controller.UpdateTimeline(differentId, timeline);

            // ASSERT
            Assert.IsInstanceOfType(result, typeof(BadRequestResult));
        }

        [TestMethod]
        public async Task UpdateTimeline_ValidTimeline_ReturnsNoContentResult()
        {
            // ARRANGE
            var timeline = new Timeline { ID = Guid.NewGuid() };
            _timelineRepository.Setup(r => r.UpdateAsync(timeline)).Returns(Task.CompletedTask);

            // ACT
            var result = await _controller.UpdateTimeline(timeline.ID, timeline);

            // ASSERT
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
            _timelineRepository.Verify(r => r.UpdateAsync(timeline), Times.Once);
        }

        [TestMethod]
        public async Task DeleteTimeline_ReturnsNoContentResult()
        {
            // ARRANGE
            var timelineId = Guid.NewGuid();
            _timelineRepository.Setup(r => r.DeleteAsync(timelineId)).Returns(Task.CompletedTask);

            // ACT
            var result = await _controller.DeleteTimeline(timelineId);

            // ASSERT
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
            _timelineRepository.Verify(r => r.DeleteAsync(timelineId), Times.Once);
        }
    }
}
