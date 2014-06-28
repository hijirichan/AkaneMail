using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AkaneMail
{
    /// <summary>
    /// ListViewの項目の並び替えに使用するクラス
    /// </summary>
    public class ListViewItemComparer : System.Collections.IComparer
    {
        static ListViewItemComparer()
        {
            ColumnModes = new[] { ComparerMode.String, ComparerMode.String, ComparerMode.DateTime, ComparerMode.Integer };
        }

        /// <summary>
        /// 比較する方法
        /// </summary>
        public enum ComparerMode
        {
            String,
            Integer,
            DateTime
        };

        private int _column;
        /// <summary>
        /// 並び替えるListView列の番号
        /// </summary>
        public int Column
        {
            get { return _column; }
            set
            {
                if (_column == value) {
                    Order = Order.Invert();
                }
                _column = value;
            }
        }

        public static ListViewItemComparer Default { get { return new ListViewItemComparer(2, SortOrder.Descending); } }

        /// <summary>
        /// 昇順か降順か
        /// </summary>
        private SortOrder Order { get; set; }

        /// <summary>
        /// 列ごとの並び替えの方法
        /// </summary>
        private static ComparerMode[] ColumnModes { get; set; }

        /// <summary>
        /// ListViewItemComparerクラスのコンストラクタ
        /// </summary>
        /// <param name="col">並び替える列番号</param>
        /// <param name="ord">昇順か降順か</param>
        /// <param name="cmod">並び替えの方法</param>
        public ListViewItemComparer(int col, SortOrder ord)
        {
            Column = col;
            Order = ord;
        }

        public ListViewItemComparer() : this(0, SortOrder.Ascending) { }

        // xがyより小さいときはマイナスの数、大きいときはプラスの数、
        // 同じときは0を返す
        public int Compare(object x, object y)
        {
            if (ColumnModes == null || ColumnModes.Length <= Column || Order == SortOrder.None) return 0;

            var result = Compare(((ListViewItem)x).SubItems[Column].Text, ((ListViewItem)y).SubItems[Column].Text);

            // 降順の時は結果を+-逆にする
            if (Order == SortOrder.Descending)
                return -result;

            return result;
        }

        private int Compare(string itemx, string itemy)
        {
            switch (ColumnModes[Column]) {
                case ComparerMode.String:
                    return string.Compare(itemx, itemy);
                case ComparerMode.Integer:
                    return int.Parse(itemx) - int.Parse(itemy);
                case ComparerMode.DateTime:
                    return DateTime.Compare(DateTime.Parse(itemx), DateTime.Parse(itemy));
                default:
                    return 0;
            }
        }
    }
}
