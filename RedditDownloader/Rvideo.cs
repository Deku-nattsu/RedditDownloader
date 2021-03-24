using System;
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
        private List<string> available_quality = new List<string>() { "240", "360", "480", "720", "1080" };
        private string dash_url, mdp;
        private readonly string url;
        private readonly bool data_parsed;
        private bool with_audio;
        private int duration;
        private List<string> locked_settings = new List<string>();
        private string title;
        private string temp_dir;
        private string video_path;
        private string audio_path;

        public List<string> Locked_settings { get => locked_settings; set => locked_settings = value; }
        public int Duration { get => duration;}
        public bool With_audio { get => with_audio;}

        public bool Data_parsed => data_parsed;

        public string Title { get => title;}
        public string Temp_dir { get => temp_dir;}
        public string Video_path { get => video_path;}
        public string Audio_path { get => audio_path;}
        public string Media_url { get => media_url;}
        public List<string> Available_quality { get => available_quality;}

        public Rvideo(string _url)
        {
            Setup_dirs();
            if (_url.Contains("?"))
                url = _url.Substring(0, _url.IndexOf('?')) + ".json";
            else
                url = _url + ".json";

            data_parsed = Parse_data();
            if (data_parsed)
            Generate_quality();

        }

        private bool Parse_data()
        {
            try
            {
                var json = new WebClient().DownloadString(url);
                List<Child_parent> post = JsonConvert.DeserializeObject<List<Child_parent>>(json);
                title = post[0].data.children[0].data.title;
                title = Regex.Replace(title, @"[^\w\s]", string.Empty);
                dash_url = post[0].data.children[0].data.secure_media.reddit_video.dash_url;
                media_url = post[0].data.children[0].data.url_overridden_by_dest + "/";
                duration = post[0].data.children[0].data.secure_media.reddit_video.duration;
            } catch (NullReferenceException)
            {
                return false;
            } catch(WebException)
            {
                return false;
            }
            return true;

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
