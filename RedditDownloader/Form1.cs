using MediaToolkit;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows.Forms;

namespace RedditDownloader
{
    public partial class Form1 : Form
    {
        private Rvideo vid;
        private string filesize;
        public Form1()
        {
            InitializeComponent();
            var radioButtons = groupBox2.Controls.OfType<RadioButton>();

            foreach (RadioButton item in radioButtons)
            {
                item.CheckedChanged += QualityBox_CheckedChanged;
            }
        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(textBox1.Text) && ((textBox1.Text.StartsWith("https://") || textBox1.Text.StartsWith("www."))))
            {
                button1.Enabled = true;
            }
            else
                button1.Enabled = false;

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            richTextBox1.Text += "Selected " + get_checked_text() + "p video quality.\n";
            button1.Enabled = false;
            button2.Enabled = false;
            richTextBox1.Text += string.Format("Downloading video file....");
            download_video(get_checked_text());
        }

        private void download_video(string quality)
        {
            using (WebClient wc = new WebClient())
            {
                wc.DownloadProgressChanged += (s, ex) =>
                {
                    progressBar1.Value = ex.ProgressPercentage;

                };
                wc.DownloadFileCompleted += (s, e) =>
                {
                    richTextBox1.Text += string.Format("Done.\n");
                    if (vid.with_audio)
                        download_audio();
                    else
                        button2.Enabled = true;
                };
                wc.DownloadFileAsync(new Uri(vid.media_url + "DASH_" + quality + ".mp4"), vid.video_path);
            }
        }
        private void download_audio()
        {
            richTextBox1.Text += string.Format("Downloading audio file....");
            using (WebClient wc = new WebClient())
            {
                wc.DownloadProgressChanged += (s, ex) =>
                {
                    progressBar1.Value = 100 + ex.ProgressPercentage;
                };
                wc.DownloadFileCompleted += (s, e) =>
                {
                    richTextBox1.Text += string.Format("Done.\n");
                    button1.Enabled = true;
                    button2.Enabled = true;
                    richTextBox1.Text += "Combining audio and video files\n";
                    backgroundWorker4.RunWorkerAsync();
                };
                wc.DownloadFileAsync(new Uri(vid.media_url + "DASH_audio.mp4"), vid.audio_path);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            richTextBox1.Text = "Fetching data from site\n";
            backgroundWorker2.RunWorkerAsync();
        }
        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            vid = new Rvideo(textBox1.Text);
            filesize = GetFileSize(vid.media_url + "DASH_" + get_checked_text() + ".mp4").ToString();


        }
        private void backgroundWorker2_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Radiomanager();
            label2.Text = vid.title;
            label2.Enabled = true;
            TimeSpan time = TimeSpan.FromSeconds(vid.duration);
            Duration.Text = time.ToString(@"mm\:ss");
            button2.Enabled = true;
            Size_label.Text = filesize;

            string download_dir = Path.Combine(Directory.GetCurrentDirectory(), "Downloads");
            if (!Directory.Exists(download_dir)) Directory.CreateDirectory(download_dir);
            textBox2.Text = Path.Combine(download_dir, vid.title + ".mp4");
            saveFileDialog1.FileName = textBox2.Text;
            textBox2.Enabled = true;
            if (vid.with_audio)
            {
                progressBar1.Maximum = 250;
                richTextBox1.Text += "Audio file found...\n";
            }
            else
            {
                richTextBox1.Text += "Audio file isn't available.\n";
            }

        }
        public void Radiomanager()
        {
            foreach (string item in vid.locked_settings)
            {
                richTextBox1.Text += item + "p isn't available....\n";
            }
            foreach (string item in vid.available_quality)
            {
                var buttons = groupBox2.Controls.OfType<RadioButton>()
                    .FirstOrDefault(n => n.Text == item);
                buttons.Enabled = true;
            }
        }
        private string get_checked_text()
        {
            var buttons = groupBox2.Controls.OfType<RadioButton>()
                .FirstOrDefault(n => n.Checked);
            return buttons.Text;
        }
        public void QualityBox_CheckedChanged(object sender, EventArgs e)
        {
            string checkedText = ((RadioButton)sender).Text;

            Size_label.Text = GetFileSize(vid.media_url + "DASH_" + checkedText + ".mp4").ToString();

        }
        public string GetFileSize(string url)
        {
            long result = -1;

            System.Net.WebRequest req = System.Net.WebRequest.Create(url);
            req.Method = "HEAD";
            using (System.Net.WebResponse resp = req.GetResponse())
            {
                if (long.TryParse(resp.Headers.Get("Content-Length"), out long ContentLength))
                {
                    result = ContentLength;
                }
            }

            return string.Format("{0:0.###} MB", (result / 1024f) / 1024f);
        }

        public void Convert(string path)
        {
            using (var engine = new Engine())
            {
                engine.CustomCommand(string.Format("-i {0} -i {1} -vcodec copy -acodec copy \"{2}\"", vid.video_path, vid.audio_path, path));

            }
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            richTextBox1.SelectionStart = richTextBox1.Text.Length;
            // scroll it automatically
            richTextBox1.ScrollToCaret();
        }
        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.saveFileDialog1.ShowDialog();
        }

        private void saveFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            this.textBox2.Text = this.saveFileDialog1.FileName;
        }

        private void backgroundWorker4_DoWork(object sender, DoWorkEventArgs e)
        {
            Convert(textBox2.Text);
        }

        private void backgroundWorker4_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            progressBar1.Value += 50;
            richTextBox1.Text += "Done.\n";
            CustomMessageBox c = new CustomMessageBox(this);
            Directory.Delete(vid.temp_dir,true);
            c.Show();
        }
        public void Clear_Form()
        {
            radioButton1.Checked = true;
            radioButton1.Enabled = false;
            radioButton2.Enabled = false;
            radioButton3.Enabled = false;
            radioButton4.Enabled = false;
            radioButton5.Enabled = false;

            textBox2.Clear();
            label2.Text = "";
            Duration.Text = "";
            Size_label.Text = "";

            richTextBox1.Clear();

            button2.Enabled = false;
            progressBar1.Value = 0;
        }

        private void button2_EnabledChanged(object sender, EventArgs e)
        {
            button3.Enabled = button2.Enabled;
        }
    }
}
