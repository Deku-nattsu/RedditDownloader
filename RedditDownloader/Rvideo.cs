using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace RedditDownloader
{
    class Rvideo
    {
        public string title, temp_dir, video_path, audio_path, media_url;
        public List<string> available_quality = new List<string>() { "240", "360", "480", "720", "1080" };
        private string dash_url;
        private string url, mdp;
        public bool with_audio;
        public int duration;
        public List<string> locked_settings = new List<string>();
        public Rvideo(string _url)
        {
            Setup_dirs();
            if (_url.Contains("?"))
                url = _url.Substring(0, _url.IndexOf('?')) + ".json";
            else
                url = _url + ".json";

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

        private void Setup_dirs()
        {
            temp_dir = Path.Combine(Path.GetTempPath(), "RVID");
            video_path = Path.Combine(temp_dir, "DASH_video.mp4");
            audio_path = Path.Combine(temp_dir, "DASH_audio.mp4");
            if (Directory.Exists(temp_dir)) Directory.Delete(temp_dir, true);
            Directory.CreateDirectory(temp_dir);

        }

        private void Generate_quality()
        {
            mdp = new WebClient().DownloadString(dash_url);
            List<string> place_holder = new List<string>(available_quality);
            for (int i = 0; i < place_holder.Count; i++)
            {

                string item = place_holder[i];
                if (!mdp.Contains(item))
                {
                    available_quality.Remove(item);
                    locked_settings.Add(item);
                }

                if (mdp.Contains("DASH_audio.mp4")) with_audio = true;
                else with_audio = false;

            }
        }

    }
}
