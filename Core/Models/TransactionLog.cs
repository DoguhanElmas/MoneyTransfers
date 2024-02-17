using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class TransactionLog
    {
        public int Id { get; set; }
        public int SenderUserId { get; set; }
        public int ReceiverUserId { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string Status { get;set; }
        public decimal SenderBalance { get; set; }
        public decimal ReceiverBalance { get; set; }
    }
}
