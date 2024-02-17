using Core.Models;
using Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repositories
{
    public class TransactionRepository : Repository<Transaction>, ITransactionRepository
    {
        private MoneyTransferDbContext MoneyTransferDbContext
        {
            get { return Context as MoneyTransferDbContext; }
        }
        public TransactionRepository(MoneyTransferDbContext context) : base(context)
        {
        }
    }
}
