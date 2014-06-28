using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AkaneMail.Views
{

    public class MailColumnHeader : ColumnHeader, IComparer<ListViewItem>, System.Collections.IComparer
    {
        private Func<string, string, int> compareFunc;
        private SortOrder order;
        public MailColumnHeader(string header, Func<string , string, int> comparer)
        {
            Text = header;
            compareFunc = comparer;
        }

        public void FlipOrder()
        {
            switch (order) { 
                case SortOrder.Ascending:
                    order = SortOrder.Descending;
                    break;
                case SortOrder.Descending:
                case SortOrder.None:
                    order = SortOrder.Ascending;
                    break;
            }
        }

        public void Reset(SortOrder order = SortOrder.None)
        {
            this.order = order;
        }

        public new string Text
        {
            get { return base.Text; }
            set
            {
                if (base.Text == value) return;
                base.Text = value;
                Reset();
            }
        }

        #region IComparer メンバー
        public int Compare(object x, object y)
        {
            return ((IComparer<ListViewItem>)this).Compare(x as ListViewItem, y as ListViewItem);
        }
        #endregion

        #region IComparer<ListViewItem> メンバー
        int IComparer<ListViewItem>.Compare(ListViewItem x, ListViewItem y)
        {
            if (order == SortOrder.None) return 0;
            return compareFunc(x.SubItems[Index].Text, y.SubItems[Index].Text) * (order == SortOrder.Ascending ? 1 : -1);
        }
        #endregion
    }

    static class MailListHeaderSelector
    {
        private static Dictionary<MailList.DisplayMode, string> headers = new Dictionary<MailList.DisplayMode, string> 
        {
            { MailList.DisplayMode.AccountList, "名前,メールアドレス,最終データ更新日,データサイズ" },
            { MailList.DisplayMode.ReceiveBox, "差出人,件名,受信日時,サイズ" },
            { MailList.DisplayMode.SendBox, "宛先,件名,送信日時,サイズ" },
            { MailList.DisplayMode.TrashBox, "差出人または宛先,件名,受信日時または送信日時,サイズ" }
        };

        private static Dictionary<string, Func<string, string, int>> compares = new Dictionary<string, Func<string, string, int>>
        {
            { "string", (x, y) => string.Compare(x, y) },
            { "int", (x, y) => int.Parse(x) - int.Parse(y) },
            { "datetime", (x, y) => DateTime.Compare(DateTime.Parse(x), DateTime.Parse(y)) }
        };
        
        private static string[] ToHeader(this string headerTexts)
        {
            return headerTexts.Split(',');
        }

        public static void Initialize(this ListView.ColumnHeaderCollection columns)
        {
            var header = headers[MailList.DisplayMode.AccountList].ToHeader();
            var func = "string,string,datetime,int".Split(',').Select(k => compares[k]);
            columns.AddRange(Enumerable.Range(0, 4).Select(i => new MailColumnHeader(header[i], func.ElementAt(i))).ToArray());
        }

        public static void SetHeader(this ListView.ColumnHeaderCollection columns, MailList.DisplayMode displayMode)
        {
            var h = headers[displayMode].ToHeader();
            foreach (var i in Enumerable.Range(0, columns.Count)) { columns[i].Text = h[i]; }
        }
    }
}