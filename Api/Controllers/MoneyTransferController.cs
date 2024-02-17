using Core.Dto.Request;
using Core.Dto.Response;
using Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoneyTransferController : ControllerBase
    {
        private readonly IMoneyTransferService _moneyTransferService;
        private readonly IUserService _userService;
        public MoneyTransferController(IMoneyTransferService moneyTransferService, IUserService userService)
        {
            this._moneyTransferService = moneyTransferService;
            this._userService = userService;
        }

        [HttpPost("transferMoney")]
        public async Task<MoneyTransferResponse> TransferMoney(MoneyTransferRequest moneyTransferRequest)
        {
            return await _moneyTransferService.TransferMoney(moneyTransferRequest);
        }

        [HttpPost("getUserBalance")]
        public async Task<UserDetailResponse> GetUserBalance(UserDetailRequest userDetailRequest)
        {
            return await _userService.GetUserDetail(userDetailRequest);
        }

        [HttpPost("getAllTransactions")]
        public async Task<TransactionDetailResponse> GetAllTransactions()
        {
            return await _moneyTransferService.GetTransactions();
        }

        [HttpPost("getTransactionById")]
        public async Task<TransactionDetailResponse> GetTransactionById(TransactionDetailRequest transactionDetailRequest)
        {
            return await _moneyTransferService.GetTransactionById(transactionDetailRequest);
        }
    }
}
