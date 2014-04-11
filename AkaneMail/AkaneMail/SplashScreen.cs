using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace AkaneMail
{
    public partial class SplashScreen : Form
    {
        public SplashScreen()
        {
            InitializeComponent();
            labelVersion.Text = "Version " + Assembly.GetExecutingAssembly().GetName().Version.ToString(3);
            labelProgress.Text = "";
        }

        public void Initialize()
        {
            ProgressMesssage = "メールクライアントの初期化中です";
            if (File.Exists(@"akanemail.png")) {
                try {
                    var image = Image.FromFile(@"akanemail.png");
                    //画像幅が極端に広かったり狭かったりしたらここで例外投げればよろし(今回はしない)
                    BackgroundImage = image;
                }
                catch {
                    // 読み込めないときは通常画像を表示するため処理なし
                }
            }
            this.Height = BackgroundImage.Height;
            this.Width = BackgroundImage.Width;
           Show();
           Refresh();
        }

        public string ProgressMesssage
        {
            set { labelProgress.Text = value; }
        }
    }
}