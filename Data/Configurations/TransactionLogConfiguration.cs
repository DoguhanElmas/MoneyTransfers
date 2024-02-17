using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Models;

namespace Data.Configurations
{
    public class TransactionLogConfiguration : IEntityTypeConfiguration<TransactionLog>
    {
        public void Configure(EntityTypeBuilder<TransactionLog> builder)
        {
            builder
                .HasKey(m => m.Id);

            builder
                .Property(m => m.Id)
                .UseMySqlIdentityColumn();

            builder
                .ToTable("TransactionLogs");
        }
    }
}
