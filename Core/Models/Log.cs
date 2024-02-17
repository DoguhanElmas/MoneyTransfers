using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class Log
    {
        public int Id { get; set; }
        public string FunctionName { get; set; }    
        public string Request { get; set; }
        public string Message { get; set; }
        public string StackTrace { get; set; }
        public DateTime DateTime { get; set; }

    }
}
