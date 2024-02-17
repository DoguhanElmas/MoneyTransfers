using Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public interface IUnitOfWork : IDisposable
    {
        ILogRepository Logs { get; }
        IUserRepository Users { get; }
        ITransactionLogRepository TransactionLogs { get; }
        ITransactionRepository Transactions { get; }
        Task<int> CommitAsync();
    }
}
