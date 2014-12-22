using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FeedReader.Models
{
    //FIXME: Might not need
    public class RSSFeed
    {
        public string id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        //TODO: Image
        public Uri link { get; set; }
        public string category { get; set; }
        public DateTime publishDate { get; set; }
    }
}