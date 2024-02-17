using Core.Models;
using Core.Repositories;

namespace Data.Repositories
{
    public class LogRepository : Repository<Log>, ILogRepository
    {
        private MoneyTransferDbContext MoneyTransferDbContext
        {
            get { return Context as MoneyTransferDbContext; }
        }
        public LogRepository(MoneyTransferDbContext context) : base(context)
        {
        }
    }
}
