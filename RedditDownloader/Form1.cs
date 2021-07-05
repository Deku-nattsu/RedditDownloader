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
        public string temp_dir = Path.Combine(Path.GetTempPath(), "RVID");
        public Form1()
        {
            InitializeComponent();

        }
        private void TextBox1_TextChanged(object sender, EventArgs e)
        {
            button1.Enabled = false;
            if (!string.IsNullOrEmpty(textBox1.Text) && ((textBox1.Text.StartsWith("https://") || textBox1.Text.StartsWith("www."))))
            button1.Enabled = true;
                
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            var radioButtons = groupBox2.Controls.OfType<RadioButton>();
            foreach (RadioButton item in radioButtons)
            {
                item.CheckedChanged += QualityBox_CheckedChanged;
            }
            if (Directory.Exists(temp_dir)) Directory.Delete(temp_dir, true);
            Directory.CreateDirectory(temp_dir);
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            richTextBox1.Text += "Selected " + GetChecked_text + "p video quality.\n";
            button1.Enabled = false;
            button2.Enabled = false;
            richTextBox1.Text += string.Format("Downloading video file....");
            Download_video(GetChecked_text);
        }

        private void Download_video(string quality)
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
                    if (vid.With_audio) Download_audio();
                    else
                    {
                        button2.Enabled = true;
                        File.Delete(textBox2.Text);
                        File.Move(vid.Video_path, textBox2.Text);
                        backgroundWorker4.RunWorkerAsync();
                    }
                };
                wc.DownloadFileAsync(new Uri(vid.Media_url + "DASH_" + quality + ".mp4"), vid.Video_path);
            }
        }
        private void Download_audio()
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
                wc.DownloadFileAsync(new Uri(vid.Media_url + "DASH_audio.mp4"), vid.Audio_path);
            }
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            richTextBox1.Text = "Fetching data from site\n";
            backgroundWorker2.RunWorkerAsync();
        }
        private void BackgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                vid = new Rvideo(textBox1.Text, temp_dir);
            }
            catch (Exception)
            {
                MessageBox.Show("Failed to parse data from site, make sure it's valid and the subreddit isn't private", "Error");
            }
        }
        private void BackgroundWorker2_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                Radiomanager();
                label2.Text = vid.Title;
                label2.Enabled = true;
                TimeSpan time = TimeSpan.FromSeconds(vid.Duration);
                Duration.Text = time.ToString(@"mm\:ss");
                button2.Enabled = true;
                Size_label.Text = GetFileSize(vid.Media_url + "DASH_" + GetChecked_text + ".mp4").ToString(); ;
                string download_dir = Path.Combine(Directory.GetCurrentDirectory(), "Downloads");
                if (!Directory.Exists(download_dir)) Directory.CreateDirectory(download_dir);
                textBox2.Text = Path.Combine(download_dir, vid.Title + ".mp4");
                if (File.Exists(textBox2.Text)) warning.Text = "Warning! file already exists";
                saveFileDialog1.FileName = textBox2.Text;
                textBox2.Enabled = true;
                if (vid.With_audio)
                {
                    progressBar1.Maximum = 250;
                    richTextBox1.Text += "Audio file found...\n";
                }
                else
                {
                    richTextBox1.Text += "Audio file isn't available.\n";
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }



        }
        public void Radiomanager()
        {
            foreach (string item in vid.Vid_q)
            {
                if(!vid.Available_quality.Contains(item))
                richTextBox1.Text += item + "p isn't available....\n";
            }
            foreach (string item in vid.Available_quality)
            {
                var buttons = groupBox2.Controls.OfType<RadioButton>()
                    .FirstOrDefault(n => n.Text == item);
                buttons.Enabled = true;
            }
        }
        private string GetChecked_text
        {
            get
            {
                var buttons = groupBox2.Controls.OfType<RadioButton>()
                    .FirstOrDefault(n => n.Checked);
                return buttons.Text;
            }
        }

        public void QualityBox_CheckedChanged(object sender, EventArgs e)
        {
            string checkedText = ((RadioButton)sender).Text;
            Size_label.Text = GetFileSize(vid.Media_url + "DASH_" + checkedText + ".mp4").ToString();

        }
        public string GetFileSize(string url)
        {
            long result = -1;

            WebRequest req = WebRequest.Create(url);
            req.Method = "HEAD";
            using (WebResponse resp = req.GetResponse())
            {
                if (long.TryParse(resp.Headers.Get("Content-Length"), out long ContentLength))
                {
                    result = ContentLength;
                }
            }

            return string.Format("{0:0.###} MB", (result / 1024f) / 1024f);
        }

        public void Convert(string output)
        {
            using (var engine = new Engine())
            {
                engine.CustomCommand(string.Format("-i {0} -i {1} -vcodec copy -acodec copy \"{2}\"", vid.Video_path, vid.Audio_path, output));

            }
        }

        private void RichTextBox1_TextChanged(object sender, EventArgs e)
        {
            richTextBox1.SelectionStart = richTextBox1.Text.Length;
            // scroll it automatically
            richTextBox1.ScrollToCaret();
        }
        private void GroupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void Button3_Click(object sender, EventArgs e)
        {
            this.saveFileDialog1.ShowDialog();
        }

        private void SaveFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            this.textBox2.Text = this.saveFileDialog1.FileName;
        }

        private void BackgroundWorker4_DoWork(object sender, DoWorkEventArgs e)
        {
            Convert(textBox2.Text);
        }

        private void BackgroundWorker4_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if(vid.With_audio) progressBar1.Value += 50;
            richTextBox1.Text += "Done.\n";
            DownloadCompletedBox c = new DownloadCompletedBox(this);
            Directory.Delete(temp_dir,true);
            c.ShowDialog();
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
            label2.Text = string.Empty;
            Duration.Text = string.Empty;
            Size_label.Text = string.Empty;

            richTextBox1.Clear();

            button2.Enabled = false;
            progressBar1.Value = 0;
        }

        private void Button2_EnabledChanged(object sender, EventArgs e)
        {
            button3.Enabled = button2.Enabled;
        }

    }
}
