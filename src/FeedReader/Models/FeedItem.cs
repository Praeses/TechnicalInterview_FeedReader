using System.Data.Entity;

namespace FeedReader.Models
{
    public class FeedItem
    {
        public string Title { get; set; }
        public string address { get; set; }
        public string siteTitle { get; set; }
        public string baseUri { get; set; }
    }


}