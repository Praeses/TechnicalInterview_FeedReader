using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeedReader.Models.Views
{
    public class Feed
    {
        public int FeedID { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }
        public int Subscribers { get; set; }
        public string RssUrl { get; set; }
        public DateTime? LastBuildDate { get; set; }
        public Image Image { get; set; }
        public List<FeedItem> FeedItems { get; set; }

        public Feed()
        {
            Image = new Image();
            FeedItems = new List<FeedItem>();
        }        
    }
}
