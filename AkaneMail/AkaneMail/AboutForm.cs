using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Reflection;

namespace AkaneMail
{
    public partial class AboutForm : Form
    {
        // アドレスを直書きから変数に変更
        string strHomeUrl = "http://www.angel-teatime.com/";

        public AboutForm()
        {
            InitializeComponent();  
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void AboutForm_Load(object sender, EventArgs e)
        {
            labelNmailVersion.Text = "nMail.dll Version " + nMail.Options.Version;
            labelVersion.Text = "Akane Mail Version " + Assembly.GetExecutingAssembly().GetName().Version.ToString(3);
            var copy = GetAssemblyAttributes<AssemblyCopyrightAttribute>(Assembly.GetExecutingAssembly());
            labelCopyright.Text = copy.Copyright;
        }

        private void linkHomePage_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            //リンク先に移動したことにする
            linkHomePage.LinkVisited = true;

            //ブラウザで開く
            System.Diagnostics.Process.Start(strHomeUrl);
        }

        static T GetAssemblyAttributes<T>(Assembly target) where T : class
        {
            var attributes = target.GetCustomAttributes(typeof(T), false);
            if (attributes != null && attributes.Any()) return attributes[0] as T;
            return null;
        }
    }
}