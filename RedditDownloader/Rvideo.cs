using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace RedditDownloader
{
    class Rvideo
    {
        private string media_url;
        private string dash_url, mdp;
        private bool with_audio;
        private int duration;
        private string title;
        private readonly string url;
        private readonly string video_path;
        private readonly string audio_path;
        private readonly string[] vid_q = { "240", "360", "480", "720", "1080" };
        private readonly List<string> available_quality = new List<string>();
        public int Duration { get => duration;}
        public bool With_audio { get => with_audio;}
        public string Title { get => title;}
        public string Video_path { get => video_path;}
        public string Audio_path { get => audio_path;}
        public string Media_url { get => media_url;}
        public List<string> Available_quality { get => available_quality;}
        public string[] Vid_q { get => vid_q;}

        public Rvideo(string _url,string temp_dir)
        {
            with_audio = false;
            video_path = Path.Combine(temp_dir, "DASH_video.mp4");
            audio_path = Path.Combine(temp_dir, "DASH_audio.mp4");
            url = _url.Contains("?")  ? _url.Substring(0, _url.IndexOf('?')) + ".json"  : _url + ".json";
            Parse_data();
            Generate_quality();
        }

        private void Parse_data()
        {
                var json = new WebClient().DownloadString(url);
                List<Child_parent> post = JsonConvert.DeserializeObject<List<Child_parent>>(json);
                title = post[0].data.children[0].data.title;
                title = Regex.Replace(title, @"[^\w\s]", string.Empty);
                dash_url = post[0].data.children[0].data.secure_media.reddit_video.dash_url;
                media_url = post[0].data.children[0].data.url_overridden_by_dest + "/";
                duration = post[0].data.children[0].data.secure_media.reddit_video.duration;
        }

        private void Generate_quality()
        {
            mdp = new WebClient().DownloadString(dash_url);
            for (int i = 0; i < Vid_q.Length; i++)
            {
                if (mdp.Contains(Vid_q[i])) available_quality.Add(Vid_q[i]);
            }
            if (mdp.Contains("DASH_audio.mp4")) with_audio = true;

        }

    }
}
