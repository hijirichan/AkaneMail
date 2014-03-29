using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AkaneMail
{
    public class ApplicationMessage
    {
        public string Message { get; set; }
        public LogLevel LogLevel { get; set; }
        public DateTime SentAt { get; set; }

        public override string ToString()
        {
            return string.Format("{0}@{1}:{2}", LogLevel, SentAt, Message);
        }
    }
}
