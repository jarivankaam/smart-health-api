using Microsoft.AspNetCore.Mvc;
using Moq;
using smarth_health.WebApi.Controllers;
using smarth_health.WebApi.Models;
using smarth_health.WebApi.Repositories;
using smarth_health.WebApi.Services;

namespace smarth_health.Tests
{
    [TestClass]
    public sealed class UserControllerTests
    {
        private Mock<IIdentityService> _identityService;
        private Mock<IUserRepository> _userRepository;
        private UserController _controller;

        [TestInitialize]
        public void Setup()
        {
            _identityService = new Mock<IIdentityService>();
            _userRepository = new Mock<IUserRepository>();
            _controller = new UserController(_userRepository.Object, _identityService.Object);
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
            var userDto = new UserDto { ID = Guid.NewGuid(), IdentityUserID = identityUserId.ToString() };

            _userRepository.Setup(repo => repo.ReadAsync(identityUserId))
                           .ReturnsAsync(userDto); 

            var expectedUser = new User
            {
                ID = userDto.ID,
                IdentityUserID = Guid.Parse(userDto.IdentityUserID)
            };

            // ACT
            var result = await _controller.GetUser(identityUserId);

            // ASSERT
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult, "Expected OkObjectResult");

            var returnedUser = okResult.Value as User;
            Assert.IsNotNull(returnedUser, "Expected value to be of type User");

            Assert.AreEqual(expectedUser.ID, returnedUser.ID);
            Assert.AreEqual(expectedUser.IdentityUserID, returnedUser.IdentityUserID);
        }

        [TestMethod]
        public async Task GetUser_UserDoesNotExist_ReturnsNotFoundResult()
        {
            // ARRANGE
            var identityUserId = Guid.NewGuid();
            _userRepository.Setup(repo => repo.ReadAsync(identityUserId))
                           .ReturnsAsync((UserDto)null);

            // ACT
            var result = await _controller.GetUser(identityUserId);

            // ASSERT
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
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
