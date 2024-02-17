using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dto.Response
{
    public class TransactionDetailResponse
    {
        public List<Transaction> Transactions { get; set; }
    }
}
