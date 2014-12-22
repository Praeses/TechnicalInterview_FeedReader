using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FeedReader.Models
{
    //FIXME: Might not need
    public class RSSFeed
    {
        public Channel channel { get; set; }
       
    }

    public class Channel
    {
        public string title { get; set; }
        public string description { get; set; }
        //TODO: Image
        public string language { get; set; }
        public DateTime lastBuildDate { get; set; }
        public string copyright { get; set; }

    }
}