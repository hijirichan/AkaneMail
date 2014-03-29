using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace AkaneMail
{
    class ApplicationMessageCollection : BindingList<ApplicationMessage>, INotifyPropertyChanged
    {
        private ApplicationMessage _LastMessage;
        public ApplicationMessage LastMessage { get { return _LastMessage; }
            private set
            {
                if (_LastMessage != value) {
                    _LastMessage = value;
                    RaisePropertyChanged("LastMessage");
                }
            }
        }

        void Send(LogLevel logLevel, string message, DateTime sentAt = default(DateTime))
        {
            var appMessage = new ApplicationMessage
            {
                LogLevel = logLevel,
                Message = message,
                SentAt = sentAt == default(DateTime) ? DateTime.Now : sentAt
            };
            Add(appMessage);
            LastMessage = appMessage;
        }

        protected void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null) {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #region INotifyPropertyChanged メンバー

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}
