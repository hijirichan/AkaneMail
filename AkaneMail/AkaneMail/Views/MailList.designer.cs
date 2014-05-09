using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AkaneMail.Views
{
    public partial class MailList : ListView, IEnumerable<Mail>, INotifyPropertyChanged
    {
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.ToolStripSeparator separator1;
            System.Windows.Forms.ToolStripSeparator separator2;
            this.MailListMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ReplyMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.ForwardMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.DeleteMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.ReadMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.UnreadMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.SaveAttachmentMenu = new System.Windows.Forms.ToolStripMenuItem();
            separator1 = new System.Windows.Forms.ToolStripSeparator();
            separator2 = new System.Windows.Forms.ToolStripSeparator();
            this.MailListMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // separator1
            // 
            separator1.Name = "separator1";
            separator1.Size = new System.Drawing.Size(267, 6);
            // 
            // separator2
            // 
            separator2.Name = "separator2";
            separator2.Size = new System.Drawing.Size(267, 6);
            // 
            // MailListMenu
            // 
            this.MailListMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ReplyMenu,
            this.ForwardMenu,
            this.DeleteMenu,
            separator1,
            this.ReadMenu,
            this.UnreadMenu,
            separator2,
            this.SaveAttachmentMenu});
            this.MailListMenu.Name = "MailListMenu";
            this.MailListMenu.Size = new System.Drawing.Size(271, 160);
            // 
            // ReplyMenu
            // 
            this.ReplyMenu.Name = "ReplyMenu";
            this.ReplyMenu.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.R)));
            this.ReplyMenu.Size = new System.Drawing.Size(270, 24);
            this.ReplyMenu.Text = "返信";
            // 
            // ForwardMenu
            // 
            this.ForwardMenu.Name = "ForwardMenu";
            this.ForwardMenu.Size = new System.Drawing.Size(270, 24);
            this.ForwardMenu.Text = "転送";
            // 
            // DeleteMenu
            // 
            this.DeleteMenu.Name = "DeleteMenu";
            this.DeleteMenu.ShortcutKeys = System.Windows.Forms.Keys.Delete;
            this.DeleteMenu.Size = new System.Drawing.Size(270, 24);
            this.DeleteMenu.Text = "削除";
            // 
            // ReadMenu
            // 
            this.ReadMenu.Name = "ReadMenu";
            this.ReadMenu.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.O)));
            this.ReadMenu.Size = new System.Drawing.Size(270, 24);
            this.ReadMenu.Text = "既読にする";
            // 
            // UnreadMenu
            // 
            this.UnreadMenu.Name = "UnreadMenu";
            this.UnreadMenu.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.U)));
            this.UnreadMenu.Size = new System.Drawing.Size(270, 24);
            this.UnreadMenu.Text = "未読にする";
            // 
            // SaveAttachmentMenu
            // 
            this.SaveAttachmentMenu.Name = "SaveAttachmentMenu";
            this.SaveAttachmentMenu.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.SaveAttachmentMenu.Size = new System.Drawing.Size(270, 24);
            this.SaveAttachmentMenu.Text = "添付ファイルを取り出す";
            // 
            // MailList
            // 
            this.ContextMenuStrip = this.MailListMenu;
            this.FullRowSelect = true;
            this.View = System.Windows.Forms.View.List;
            this.MailListMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        private ContextMenuStrip MailListMenu;
        private IContainer components;
        private ToolStripMenuItem ReplyMenu;
        private ToolStripMenuItem ForwardMenu;
        private ToolStripMenuItem DeleteMenu;
        private ToolStripMenuItem ReadMenu;
        private ToolStripMenuItem UnreadMenu;
        private ToolStripMenuItem SaveAttachmentMenu;
    }
}
