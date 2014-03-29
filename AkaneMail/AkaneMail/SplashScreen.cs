using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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

        public string ProgressMsg
        {
            set
            {
                labelProgress.Text = value;
            }
        }
    }
}