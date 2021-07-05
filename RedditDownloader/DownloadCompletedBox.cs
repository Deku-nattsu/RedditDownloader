using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace RedditDownloader
{
    public partial class DownloadCompletedBox : Form
    {
        private readonly Form1 frm;
        private readonly FileInfo fi;
        public DownloadCompletedBox(Form1 frm)
        {
            InitializeComponent();
            this.frm = frm;
            fi = new FileInfo(frm.textBox2.Text);
        }

        private void CustomMessageBox_Load(object sender, EventArgs e)
        {
            textBox1.Text = fi.Name;
            label3.Text = string.Format("{0:0.##} MB", (fi.Length / 1024f) / 1024f);
        }

        private void Button1_Click(object sender, EventArgs e)
        {

            Process.Start(fi.Directory.FullName);
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            Process.Start(frm.textBox2.Text);
        }

        private void Button4_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            Dispose();
        }

        private void CustomMessageBox_FormClosed(object sender, FormClosedEventArgs e)
        {
            frm.Clear_Form();
        }
    }
}
