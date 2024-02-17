using Core;
using Core.Dto.Request;
using Core.Models;
using Core.Repositories;
using Moq;
using Services.Services;

namespace Test
{
    public class UserServiceTests
    {
        [Fact]
        public async Task GetUserDetail_ValidRequest_ReturnsUserDetailResponse()
        {
            // Arrange
            var userId = 1;
            var user = new User { Id = userId, Balance = 100 };
            var userDetailRequest = new UserDetailRequest { UserId = userId };

            var userRepositoryMock = new Mock<IUserRepository>();
            userRepositoryMock.Setup(repo => repo.GetByIdAsync(userId)).ReturnsAsync(user);

            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.Setup(uow => uow.Users).Returns(userRepositoryMock.Object);

            var userService = new UserService(unitOfWorkMock.Object);

            // Act
            var result = await userService.GetUserDetail(userDetailRequest);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(user.Balance, result.Balance);
        }

        [Fact]
        public async Task GetUserDetail_ExceptionThrown_LogsExceptionAndReturnsDefaultResponse()
        {
            // Arrange
            var userId = 1;
            var userDetailRequest = new UserDetailRequest { UserId = userId };
            var exceptionMessage = "Test Exception";

            var userRepositoryMock = new Mock<IUserRepository>();
            userRepositoryMock.Setup(repo => repo.GetByIdAsync(userId)).ThrowsAsync(new Exception(exceptionMessage));

            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.Setup(uow => uow.Users).Returns(userRepositoryMock.Object);

            var logRepositoryMock = new Mock<ILogRepository>();
            unitOfWorkMock.Setup(uow => uow.Logs).Returns(logRepositoryMock.Object);


            var userService = new UserService(unitOfWorkMock.Object);

            // Act
            var result = await userService.GetUserDetail(userDetailRequest);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(0, result.Balance); 

            unitOfWorkMock.Verify(uow => uow.Logs.AddAsync(It.IsAny<Log>()), Times.Once);
        }
    }
}