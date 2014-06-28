// コンテキストメニューのイベントハンドラーとそれに付随する処理をまとめる
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AkaneMail.Views
{
    public partial class MailList
    {
        private void OnDeleteMenuClick(object sender, EventArgs e)
        {
            RequestRemoveMails();
        }

        private void OnReplyMenuClick(object sender, EventArgs e)
        {
           
        }

        protected void OnForwadMenuClick(object sender, EventArgs e)
        {
            
        }

        private void ReadMenuClick(object sender, EventArgs e)
        {
            ChangeSelectedMailReadStatus(false);
        }

        /// <summary>
        /// 既読メールを未読にする
        /// </summary>
        private void UnreadMenuClick(object sender, EventArgs e)
        {
            ChangeSelectedMailReadStatus(true);
        }

        private void ChangeSelectedMailReadStatus(bool unread)
        {
            if (SelectedItems.Count == 0) return;
            BeginUpdate();
            foreach (var item in SelectedItems.Cast<MailListItem>()) {
                Invoke(() => item.IsRead = unread);
            }
            
            SelectedIndices.Clear();
            Items[0].EnsureVisible();
            EndUpdate();
        }
    }
}
