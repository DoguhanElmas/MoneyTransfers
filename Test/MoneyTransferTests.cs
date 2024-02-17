using Core;
using Core.Dto.Request;
using Core.Models;
using Core.Repositories;
using Moq;
using Services.Services;

namespace Test
{
    public class MoneyTransferTests
    {
        [Fact]
        public async Task GetTransactionById_ExistingTransaction_ReturnsTransactionDetailResponseWithTransaction()
        {
            // Arrange
            var transactionId = 1;
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var transaction = new Transaction { Id = transactionId };
            unitOfWorkMock.Setup(uow => uow.Transactions.GetByIdAsync(transactionId)).ReturnsAsync(transaction);
            var service = new MoneyTransferService(unitOfWorkMock.Object);
            var request = new TransactionDetailRequest { Id = transactionId };

            // Act
            var result = await service.GetTransactionById(request);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Transactions);
            Assert.Single(result.Transactions);
            Assert.Equal(transactionId, result.Transactions[0].Id);
        }

        [Fact]
        public async Task GetTransactionById_NonExistingTransaction_ReturnsEmptyTransactionDetailResponse()
        {
            // Arrange
            var transactionId = 1;
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.Setup(uow => uow.Transactions.GetByIdAsync(transactionId)).ReturnsAsync((Transaction)null);

            var logRepositoryMock = new Mock<ILogRepository>();
            unitOfWorkMock.Setup(uow => uow.Logs).Returns(logRepositoryMock.Object);

            var service = new MoneyTransferService(unitOfWorkMock.Object);
            var request = new TransactionDetailRequest { Id = transactionId };

            // Act
            var result = await service.GetTransactionById(request);

            // Assert
            Assert.NotNull(result);
            Assert.Null(result.Transactions);            
        }

        [Fact]
        public async Task GetTransactionById_Exception_ReturnsEmptyTransactionDetailResponseAndLogsError()
        {
            // Arrange
            var exceptionMessage = "Test Exception";
            var transactionId = 1;
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.Setup(uow => uow.Transactions.GetByIdAsync(transactionId)).ThrowsAsync(new Exception(exceptionMessage));

            var logRepositoryMock = new Mock<ILogRepository>();
            unitOfWorkMock.Setup(uow => uow.Logs).Returns(logRepositoryMock.Object);

            var service = new MoneyTransferService(unitOfWorkMock.Object);
            var request = new TransactionDetailRequest { Id = transactionId };

            // Act
            var result = await service.GetTransactionById(request);

            // Assert
            Assert.NotNull(result);
            Assert.Null(result.Transactions);

            unitOfWorkMock.Verify(uow => uow.Logs.AddAsync(It.IsAny<Log>()), Times.Once);
        }

        [Fact]
        public async Task GetTransactions_ReturnsAllTransactions()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var transactions = new List<Transaction>
            {
                new Transaction { Id = 1, Amount = 100 },
                new Transaction { Id = 2, Amount = 200 }
            };
            unitOfWorkMock.Setup(uow => uow.Transactions.GetAllAsync()).ReturnsAsync(transactions);
            var service = new MoneyTransferService(unitOfWorkMock.Object);

            // Act
            var result = await service.GetTransactions();

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Transactions);
            Assert.Equal(transactions.Count, result.Transactions.Count);
            Assert.Equal(transactions[0].Id, result.Transactions[0].Id);
            Assert.Equal(transactions[1].Id, result.Transactions[1].Id);
        }

        [Fact]
        public async Task GetTransactions_Exception_LogsErrorAndReturnsEmptyResponse()
        {
            // Arrange
            var exceptionMessage = "Test Exception";
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var transactionRepositoryMock = new Mock<ITransactionRepository>();
            unitOfWorkMock.Setup(uow => uow.Transactions).Returns(transactionRepositoryMock.Object);         
            unitOfWorkMock.Setup(uow => uow.Transactions.GetAllAsync()).ThrowsAsync(new Exception(exceptionMessage));   

            var logRepositoryMock = new Mock<ILogRepository>();
            unitOfWorkMock.Setup(uow => uow.Logs).Returns(logRepositoryMock.Object);

            var service = new MoneyTransferService(unitOfWorkMock.Object);

            // Act
            var result = await service.GetTransactions();

            // Assert
            Assert.NotNull(result);
            Assert.Null(result.Transactions);

            unitOfWorkMock.Verify(uow => uow.Logs.AddAsync(It.IsAny<Log>()), Times.Once);
        }
        [Fact]
        public async Task TransferMoney_SuccessfulTransaction_ReturnsSuccessResponseAndLogs()
        {
            // Arrange
            var senderUserId = 1;
            var receiverUserId = 2;
            var amount = 100;
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var senderUser = new User { Id = senderUserId, Balance = amount };
            var receiverUser = new User { Id = receiverUserId, Balance = 0 };
            unitOfWorkMock.Setup(uow => uow.Users.GetByIdAsync(senderUserId)).ReturnsAsync(senderUser);
            unitOfWorkMock.Setup(uow => uow.Users.GetByIdAsync(receiverUserId)).ReturnsAsync(receiverUser);

            var logRepositoryMock = new Mock<ILogRepository>();
            unitOfWorkMock.Setup(uow => uow.Logs).Returns(logRepositoryMock.Object);

            var transactionLogRepositoryMock = new Mock<ITransactionLogRepository>();
            unitOfWorkMock.Setup(uow => uow.TransactionLogs).Returns(transactionLogRepositoryMock.Object);

            var transactionRepositoryMock = new Mock<ITransactionRepository>();
            unitOfWorkMock.Setup(uow => uow.Transactions).Returns(transactionRepositoryMock.Object);

            var service = new MoneyTransferService(unitOfWorkMock.Object);
            var request = new MoneyTransferRequest { SenderUserId = senderUserId, ReceiverUserId = receiverUserId, Amount = amount };

            // Act
            var result = await service.TransferMoney(request);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.Equal("Success!", result.Message);

            unitOfWorkMock.Verify(uow => uow.TransactionLogs.AddAsync(It.IsAny<TransactionLog>()), Times.Exactly(3));
            unitOfWorkMock.Verify(uow => uow.TransactionLogs.SaveChangesAsync(), Times.Exactly(3));
            unitOfWorkMock.Verify(uow => uow.Transactions.AddAsync(It.IsAny<Transaction>()), Times.Once);
            unitOfWorkMock.Verify(uow => uow.Transactions.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task TransferMoney_SenderUserNotFound_ReturnsErrorResponseAndLogs()
        {
            // Arrange
            var senderUserId = 1;
            var receiverUserId = 2;
            var amount = 100;
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.Setup(uow => uow.Users.GetByIdAsync(senderUserId)).ReturnsAsync((User)null);

            var service = new MoneyTransferService(unitOfWorkMock.Object);
            var request = new MoneyTransferRequest { SenderUserId = senderUserId, ReceiverUserId = receiverUserId, Amount = amount };

            // Act
            var result = await service.TransferMoney(request);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.Success);
            Assert.Equal("Sender Undefined!", result.Message);

        }

        [Fact]
        public async Task TransferMoney_ReceiverUserNotFound_ReturnsErrorResponseAndLogs()
        {
            // Arrange
            var senderUserId = 1;
            var receiverUserId = 2;
            var amount = 100;
            var senderUser = new User { Id = senderUserId, Balance = amount };

            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.Setup(uow => uow.Users.GetByIdAsync(senderUserId)).ReturnsAsync(senderUser);
            unitOfWorkMock.Setup(uow => uow.Users.GetByIdAsync(receiverUserId)).ReturnsAsync((User)null);


            var service = new MoneyTransferService(unitOfWorkMock.Object);
            var request = new MoneyTransferRequest { SenderUserId = senderUserId, ReceiverUserId = receiverUserId, Amount = amount };

            // Act
            var result = await service.TransferMoney(request);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.Success);
            Assert.Equal("Receiver Undefined!", result.Message);

        }

        [Fact]
        public async Task TransferMoney_NotEnoughBalance_ReturnsErrorResponse()
        {
            // Arrange
            var senderUserId = 1;
            var receiverUserId = 2;
            var amount = 100;
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var senderUser = new User { Id = senderUserId, Balance = amount - 50 };
            var receiverUser = new User { Id = receiverUserId, Balance = 0 };
            unitOfWorkMock.Setup(uow => uow.Users.GetByIdAsync(senderUserId)).ReturnsAsync(senderUser);
            unitOfWorkMock.Setup(uow => uow.Users.GetByIdAsync(receiverUserId)).ReturnsAsync(receiverUser);          

            var service = new MoneyTransferService(unitOfWorkMock.Object);
            var request = new MoneyTransferRequest { SenderUserId = senderUserId, ReceiverUserId = receiverUserId, Amount = amount };

            // Act
            var result = await service.TransferMoney(request);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.Success);
            Assert.Equal("Not Enough Balance!", result.Message);
        }

        [Fact]
        public async Task TransferMoney_Exception_LogsErrorAndReturnsEmptyResponse()
        {
            // Arrange
            var exceptionMessage = "Test Exception";
            var senderUserId = 1;
            var receiverUserId = 2;
            var amount = 100;
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var senderUser = new User { Id = senderUserId, Balance = amount };
           
            unitOfWorkMock.Setup(uow => uow.Users.GetByIdAsync(senderUserId)).ReturnsAsync(senderUser);
            unitOfWorkMock.Setup(uow => uow.Users.GetByIdAsync(receiverUserId)).ThrowsAsync(new Exception(exceptionMessage));

            var logRepositoryMock = new Mock<ILogRepository>();
            unitOfWorkMock.Setup(uow => uow.Logs).Returns(logRepositoryMock.Object);        

            var service = new MoneyTransferService(unitOfWorkMock.Object);
            var request = new MoneyTransferRequest { SenderUserId = senderUserId, ReceiverUserId = receiverUserId, Amount = amount };

            // Act
            var result = await service.GetTransactions();

            // Assert
            Assert.NotNull(result);
            Assert.Null(result.Transactions);

            unitOfWorkMock.Verify(uow => uow.Logs.AddAsync(It.IsAny<Log>()), Times.Once);
        }
    }
}
