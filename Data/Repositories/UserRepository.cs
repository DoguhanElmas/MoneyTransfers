using Core.Models;
using Core.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repositories
{
    public class UserRepository :Repository<User>, IUserRepository
    {
        private MoneyTransferDbContext MoneyTransferDbContext
        {
            get { return Context as MoneyTransferDbContext; }
        }
        public UserRepository(MoneyTransferDbContext context) : base(context)
        {
        }
    }
}
