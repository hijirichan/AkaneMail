using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AkaneMail.Views
{
    class MailListItem : ListViewItem
    {
        public MailListItem() : base() { }
        public MailListItem(Mail mail) : this()
        {
            this.Tag = mail;
            this.SubItems.AddRange(new[] { mail.Address, mail.Subject, mail.Date, mail.Size });
        }

        public bool IsRead
        {
            get { return Tag.NotReadYet; }
            set
            {
                if (Tag == null) return;
                Tag.NotReadYet = value;
                Update();
            }
        }

        private Mail _Tag;
        public new Mail Tag
        {
            get { return _Tag; }
            private set 
            {
                if (value == _Tag) return;
                _Tag = value;
                if (value != null) Update();
            }
        }

        public void Update()
        {
            SubItems.Clear();
            SubItems.Add(string.IsNullOrWhiteSpace(Tag.Subject) ? "(no subject)" : Tag.Subject);

            SubItems.Add(Tag.Date);
            SubItems.Add(Tag.Size);

            var style = Tag.NotReadYet ? FontStyle.Bold : FontStyle.Regular;
            Font = new Font(this.Font, style);
            ForeColor = MailPriority.GetPriorityColor(Tag);
        }
    }
   
}
