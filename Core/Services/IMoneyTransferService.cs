using Core.Dto.Request;
using Core.Dto.Response;
using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Services
{
    public interface IMoneyTransferService
    {
        Task<MoneyTransferResponse> TransferMoney(MoneyTransferRequest moneyTransferRequest);
        Task<TransactionDetailResponse> GetTransactionById(TransactionDetailRequest transactionDetailRequest);
        Task<TransactionDetailResponse> GetTransactions();
        Task<int> InsertLog(TransactionLogRequest transactionLogRequest);
    }
}
