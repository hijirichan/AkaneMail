using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AkaneMail.Views
{

    public partial class MailList
    {
        /// <summary>
        /// MailListの表示モードを表します。
        /// </summary>
        public enum DisplayMode
        {
            /// <summary>アカウント情報のリストであることを表します。これが規定値です。</summary>
            AccountList,
            /// <summary>受信箱であることを表します。</summary>
            ReceiveBox,
            /// <summary>送信箱であることを表します。</summary>
            SendBox,
            /// <summary>ごみ箱であることを表します。</summary>
            TrashBox
        }

        public MailList()
            : base()
        {
            InitializeComponent();
            Columns.Initialize();
            this.ListViewItemSorter = Columns[2] as MailColumnHeader;
            ((MailColumnHeader)this.ListViewItemSorter).Reset(SortOrder.Descending);
        }

        #region Events
        /// <summary>選択されているメールが変更されたときに発生します。</summary>
        public event EventHandler<MailEventArgs> MailSelected;
        private void RaiseMailSelected(Mail mail)
        {
            if (MailSelected != null) MailSelected(this, new MailEventArgs(mail));
        }
        #endregion

        #region Properties
        public DisplayMode Mode { get; private set; }

        public string ListName { get { return Folder.Name; } }

        public IEnumerable<Mail> SelectedMails
        {
            get { return SelectedItems.Cast<ListViewItem>().Select(i => i.Tag as Mail); }
        }
        
        #region Folder変更通知プロパティ
        private MailFolder _Folder;

        public MailFolder Folder
        {
            get { return _Folder; }
            set
            {
                if (_Folder == value) return;
                _Folder.CollectionChanged -= OnMailFolderChanged;
                _Folder = value;
                _Folder.CollectionChanged += OnMailFolderChanged;
                OnFolderChanged(_Folder);
                RaisePropertyChanged();
            }
        }
        #endregion
        #endregion

        #region Methods
        public void Initialize(params string[] contents)
        {
            BeginUpdate();
            Items.Clear();
            if (contents == null || contents.Length == 0) {
                Items.Add(new ListViewItem(contents));
            }
            else {
                Items.Add(new ListViewItem(new [] {"", "データ未作成", "0"}));
            }
            EndUpdate();
        }

        private void UpdateItems()
        {
            BeginUpdate();
            Items.Clear();
            Items.AddRange(Folder.Select(m => new MailListItem(m)).ToArray());
            EndUpdate();
        }

        private void OnMailFolderChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Invoke(() => OnFolderChanged(sender as MailFolder));
        }

        #region Invoke overloads
        void Invoke(Action action)
        {
            Invoke((Delegate)action);
        }
        T Invoke<T>(Func<T> func)
        {
            return (T)Invoke((Delegate)func);
        }
        #endregion
        #endregion

        #region Implements
        #region IEnumerable<Mail> メンバー
        public IEnumerator<Mail> GetEnumerator()
        {
            return Folder.GetEnumerator();
        }
        #endregion

        #region IEnumerable メンバー
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return Folder.GetEnumerator();
        }
        #endregion

        #region INotifyPropertyChanged メンバー
        public event PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
        
        public event NotifyCollectionChangedEventHandler RemoveMailsRequested;
        protected void RequestRemoveMails()
        {
            if (RemoveMailsRequested != null) RemoveMailsRequested(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, SelectedMails.ToList()));
        }
        #endregion

        #region Overrides
        private void OnFolderChanged(MailFolder folder)
        {
            Columns.SetHeader((DisplayMode)Enum.Parse(typeof(DisplayMode), folder.Name));
            UpdateItems();
        }

        protected override void OnItemSelectionChanged(ListViewItemSelectionChangedEventArgs e)
        {
            base.OnItemSelectionChanged(e);
            if (e.IsSelected) RaiseMailSelected(e.Item.Tag as Mail);
        }

        protected override void OnColumnClick(ColumnClickEventArgs e)
        {
            base.OnColumnClick(e);
            var col = this.Columns[e.Column] as MailColumnHeader;
            if (ListViewItemSorter == col) {
                col.FlipOrder();
            }
            else {
                col.Reset();
                ListViewItemSorter = col;
            }
            Sort();
        }
        #endregion

    }
}