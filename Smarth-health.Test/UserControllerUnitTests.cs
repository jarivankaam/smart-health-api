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
    public sealed class UserControllerTests
    {
        private Mock<IUserRepository> _userRepository;
        private UserController _controller;

        [TestInitialize]
        public void Setup()
        {
            _userRepository = new Mock<IUserRepository>();
            _controller = new UserController(_userRepository.Object);
        }

        [TestMethod]
        public async Task CreateUser_ValidUser_ReturnsCreatedAtActionResult()
        {
            // ARRANGE
            var user = new User { IdentityUserID = Guid.NewGuid() };
            _userRepository.Setup(repo => repo.InsertAsync(It.IsAny<User>()))
                           .Returns(Task.CompletedTask);

            // ACT
            var result = await _controller.CreateUser(user);

            // ASSERT
            var createdResult = result as CreatedAtActionResult;
            Assert.IsNotNull(createdResult, "Expected CreatedAtActionResult");
            Assert.AreEqual("GetUser", createdResult.ActionName);
            Assert.IsNotNull(createdResult.RouteValues);
            Assert.IsTrue(createdResult.RouteValues.ContainsKey("identityUserId"));
            Assert.AreEqual(user.IdentityUserID, createdResult.RouteValues["identityUserId"]);

            var returnedUser = createdResult.Value as User;
            Assert.IsNotNull(returnedUser);
            Assert.AreNotEqual(Guid.Empty, returnedUser.ID, "User ID should be assigned a new Guid.");

            _userRepository.Verify(repo => repo.InsertAsync(It.IsAny<User>()), Times.Once);
        }

        [TestMethod]
        public async Task GetUser_UserExists_ReturnsOkObjectResult()
        {
            // ARRANGE
            var identityUserId = Guid.NewGuid();
            var user = new User { ID = Guid.NewGuid(), IdentityUserID = identityUserId };
            _userRepository.Setup(repo => repo.ReadAsync(identityUserId))
                           .ReturnsAsync(user);

            // ACT
            var result = await _controller.GetUser(identityUserId);

            // ASSERT
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult, "Expected OkObjectResult");
            Assert.AreEqual(user, okResult.Value);
        }

        [TestMethod]
        public async Task GetUser_UserDoesNotExist_ReturnsNotFoundResult()
        {
            // ARRANGE
            var identityUserId = Guid.NewGuid();
            _userRepository.Setup(repo => repo.ReadAsync(identityUserId))
                           .ReturnsAsync((User)null);

            // ACT
            var result = await _controller.GetUser(identityUserId);

            // ASSERT
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task UpdateUser_IdMismatch_ReturnsBadRequest()
        {
            // ARRANGE
            var userId = Guid.NewGuid();
            var user = new User { ID = Guid.NewGuid() };

            // ACT
            var result = await _controller.UpdateUser(userId, user);

            // ASSERT
            Assert.IsInstanceOfType(result, typeof(BadRequestResult));
        }

        [TestMethod]
        public async Task UpdateUser_ValidUser_ReturnsNoContentResult()
        {
            // ARRANGE
            var userId = Guid.NewGuid();
            var user = new User { ID = userId };

            _userRepository.Setup(repo => repo.UpdateAsync(user))
                           .Returns(Task.CompletedTask);

            // ACT
            var result = await _controller.UpdateUser(userId, user);

            // ASSERT
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
            _userRepository.Verify(repo => repo.UpdateAsync(user), Times.Once);
        }

        [TestMethod]
        public async Task DeleteUser_ReturnsNoContentResult()
        {
            // ARRANGE
            var userId = Guid.NewGuid();
            _userRepository.Setup(repo => repo.DeleteAsync(userId))
                           .Returns(Task.CompletedTask);

            // ACT
            var result = await _controller.DeleteUser(userId);

            // ASSERT
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
            _userRepository.Verify(repo => repo.DeleteAsync(userId), Times.Once);
        }
    }
}
