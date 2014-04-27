using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AkaneMail.Views
{
    public class MailEventArgs : EventArgs
    {
        public Mail SelectedMail { get; private set; }
        public MailEventArgs() : base() { }
        public MailEventArgs(Mail mail)
            : this()
        {
            SelectedMail = mail;
        }
    }
}
