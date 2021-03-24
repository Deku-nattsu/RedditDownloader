using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RedditDownloader
{
    public partial class CustomMessageBox : Form
    {
        private readonly Form1 frm;
        private readonly FileInfo fi;
        public CustomMessageBox(Form1 frm)
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

        private void button1_Click(object sender, EventArgs e)
        {

            Process.Start(fi.Directory.FullName);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Process.Start(frm.textBox2.Text);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Close();
            frm.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void CustomMessageBox_FormClosed(object sender, FormClosedEventArgs e)
        {
            frm.Clear_Form();
        }
    }
}
