using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dto.Request
{
    public class MoneyTransferRequest
    {
        public int SenderUserId { get; set; }
        public int ReceiverUserId { get; set; }
        public decimal Amount { get; set; }
    }
}
