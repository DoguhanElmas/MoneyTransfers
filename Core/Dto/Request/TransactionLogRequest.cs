using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dto.Request
{
    public class TransactionLogRequest
    {
        public string Status { get; set; }
        public User SenderUser { get; set; }
        public User ReceiverUser { get; set; }
        public decimal Amount { get; set; }
    }
}