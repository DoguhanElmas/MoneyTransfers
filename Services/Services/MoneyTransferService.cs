using Core;
using Core.Constants;
using Core.Dto.Request;
using Core.Dto.Response;
using Core.Models;
using Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Services.Services
{
    public class MoneyTransferService : IMoneyTransferService
    {
        private readonly IUnitOfWork _unitOfWork;

        public MoneyTransferService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<TransactionDetailResponse> GetTransactionById(TransactionDetailRequest transactionDetailRequest)
        {
            try
            {
                var transaction = await _unitOfWork.Transactions.GetByIdAsync(transactionDetailRequest.Id);

                var response = new TransactionDetailResponse();

                if (transaction != null)
                {
                    response.Transactions = new List<Transaction>();
                    response.Transactions.Add(transaction);
                }              


                return response;

            }
            catch (Exception e)
            {

                var log = new Log();
                log.StackTrace = e.StackTrace;
                log.Message = e.Message;
                log.Request = JsonSerializer.Serialize(transactionDetailRequest);
                log.FunctionName = "Service-GetTransactionById";
                log.DateTime = DateTime.Now;
                await _unitOfWork.Logs.AddAsync(log);
                await _unitOfWork.Logs.SaveChangesAsync();
                return new TransactionDetailResponse();
            }
        }

        public async Task<TransactionDetailResponse> GetTransactions()
        {
            try
            {
                var transaction = await _unitOfWork.Transactions.GetAllAsync();

                var response = new TransactionDetailResponse();
                response.Transactions = transaction.ToList();

                return response;
            }
            catch (Exception e)
            {

                var log = new Log();
                log.StackTrace = e.StackTrace;
                log.Message = e.Message;
                log.Request = "";
                log.FunctionName = "Service-GetTransactions";
                log.DateTime = DateTime.Now;
                await _unitOfWork.Logs.AddAsync(log);
                await _unitOfWork.Logs.SaveChangesAsync();
                return new TransactionDetailResponse();
            }
        }

        public async Task<int> InsertLog(TransactionLogRequest transactionLogRequest)
        {
            var transactionLog = new TransactionLog
            {
                Status = transactionLogRequest.Status,
                Amount = transactionLogRequest.Amount,
                SenderBalance = transactionLogRequest.SenderUser.Balance,
                SenderUserId = transactionLogRequest.SenderUser.Id,
                ReceiverUserId = transactionLogRequest.ReceiverUser.Id,
                ReceiverBalance = transactionLogRequest.ReceiverUser.Balance,
                Date = DateTime.Now
            };
            await _unitOfWork.TransactionLogs.AddAsync(transactionLog);
           return await _unitOfWork.TransactionLogs.SaveChangesAsync();
        }

        public async Task<MoneyTransferResponse> TransferMoney(MoneyTransferRequest moneyTransferRequest)
        {
            try
            {
                var response = new MoneyTransferResponse();

                if (moneyTransferRequest.SenderUserId == moneyTransferRequest.ReceiverUserId)
                {
                    response.Message = "Same User!";
                    return response;
                }

               
                var senderUser = await _unitOfWork.Users.GetByIdAsync(moneyTransferRequest.SenderUserId);
                var receiverUser = await _unitOfWork.Users.GetByIdAsync(moneyTransferRequest.ReceiverUserId);
                if (senderUser != null && receiverUser != null && senderUser.Balance >= moneyTransferRequest.Amount)
                {
                    var transactionLogRequest = new TransactionLogRequest
                    {
                        ReceiverUser = receiverUser,
                        Amount = moneyTransferRequest.Amount,
                        SenderUser = senderUser,
                        Status = TransactionConstants.Begin
                    };

                    await InsertLog(transactionLogRequest);

                    senderUser.Balance -= moneyTransferRequest.Amount;

                    await _unitOfWork.Users.SaveChangesAsync();

                    transactionLogRequest.Status = TransactionConstants.InProgress;
                    transactionLogRequest.SenderUser = senderUser;

                    await InsertLog(transactionLogRequest);

                    receiverUser.Balance += moneyTransferRequest.Amount;

                    await _unitOfWork.Users.SaveChangesAsync();

                    transactionLogRequest.Status = TransactionConstants.Completed;
                    transactionLogRequest.ReceiverUser = receiverUser;

                    await InsertLog(transactionLogRequest);

                    var transaction = new Transaction
                    {
                        Amount = moneyTransferRequest.Amount,
                        Date = DateTime.Now,
                        ReceiverUserId = receiverUser.Id,
                        SenderUserId = senderUser.Id,
                    };

                    await _unitOfWork.Transactions.AddAsync(transaction);
                    await _unitOfWork.Transactions.SaveChangesAsync();

                    response.Success = true;
                    response.Message = "Success!";

                    return response;

                }
                else
                {
                    if (senderUser == null)
                    {
                        response.Message = "Sender Undefined!";
                    }
                    else if (receiverUser == null)
                    {
                        response.Message = "Receiver Undefined!";
                    }
                    else
                    {
                        response.Message = "Not Enough Balance!";
                    }
                    return response;
                }
            }
            catch (Exception e)
            {

                var log = new Log();
                log.StackTrace = e.StackTrace;
                log.Message = e.Message;
                log.Request = JsonSerializer.Serialize(moneyTransferRequest);
                log.FunctionName = "Service-TransferMoney";
                log.DateTime = DateTime.Now;
                await _unitOfWork.Logs.AddAsync(log);
                await _unitOfWork.Logs.SaveChangesAsync();
                return new MoneyTransferResponse();
            }
        }
    }
}
