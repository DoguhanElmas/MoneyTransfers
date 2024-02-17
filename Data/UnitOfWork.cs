using Core;
using Core.Repositories;
using Data.Repositories;

namespace Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly MoneyTransferDbContext _context;

        private LogRepository _logRepository;
        private UserRepository _userRepository;
        private TransactionLogRepository _transactionLogRepository;
        private TransactionRepository _transactionRepository;


        public UnitOfWork(MoneyTransferDbContext context)
        {
            this._context = context;
        }

        public ILogRepository Logs => _logRepository = _logRepository ?? new LogRepository(_context);
        public IUserRepository Users => _userRepository = _userRepository ?? new UserRepository(_context);
        public ITransactionRepository Transactions => _transactionRepository = _transactionRepository ?? new TransactionRepository(_context);
        public ITransactionLogRepository TransactionLogs => _transactionLogRepository = _transactionLogRepository ?? new TransactionLogRepository(_context);


        public async Task<int> CommitAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
