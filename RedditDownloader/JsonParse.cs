using System.Collections.Generic;

namespace RedditDownloader
{
    public class MediaEmbed
    {
    }

    public class SecureMediaEmbed
    {
    }
    public class RedditVideo
    {
        public int bitrate_kbps { get; set; }
        public string fallback_url { get; set; }
        public int height { get; set; }
        public int width { get; set; }
        public string scrubber_media_url { get; set; }
        public string dash_url { get; set; }
        public int duration { get; set; }
        public string hls_url { get; set; }
        public bool is_gif { get; set; }
        public string transcoding_status { get; set; }
    }
    public class SecureMedia
    {
        public RedditVideo reddit_video { get; set; }
    }

    public class Data_parent
    {
        public IList<Child_child> children { get; set; }
    }
    public class Child_parent
    {
        public Data_parent data { get; set; }
    }
    public class Child_child
    {
        public Data_child data { get; set; }
    }
    public class Data_child
    {
        public string subreddit { get; set; }
        public string title { get; set; }
        public string name { get; set; }
        public MediaEmbed media_embed { get; set; }
        public string author_fullname { get; set; }
        public SecureMedia secure_media { get; set; }
        public bool is_reddit_media_domain { get; set; }
        public SecureMediaEmbed secure_media_embed { get; set; }
        public string domain { get; set; }
        public string url_overridden_by_dest { get; set; }
        public string id { get; set; }
        public string author { get; set; }
        public string permalink { get; set; }
        public string url { get; set; }
        public bool is_video { get; set; }
    }
}
