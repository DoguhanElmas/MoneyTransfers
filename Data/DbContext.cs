using Core.Models;
using Data.Configurations;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public class MoneyTransferDbContext : DbContext
    {
        public DbSet<Log> Logs { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<TransactionLog> TransactionLogs { get; set; }

        public MoneyTransferDbContext(DbContextOptions<MoneyTransferDbContext> options)
            : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder builder)
        {

            builder
                .ApplyConfiguration(new LogConfiguration());
            builder
              .ApplyConfiguration(new UserConfiguration());
            builder
              .ApplyConfiguration(new TransactionConfiguration());
            builder
              .ApplyConfiguration(new TransactionLogConfiguration());

            SeedData(builder);
        }
        private void SeedData(ModelBuilder builder)
        {
            builder.Entity<User>().HasData(
                new User { Id = 1, Balance = 10000 },
                new User { Id = 2, Balance = 20000 },
                new User { Id = 3, Balance = 15000 },
                new User { Id = 4, Balance = 25000 }
            );
        }
    }
}
