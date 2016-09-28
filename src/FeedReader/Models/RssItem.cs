using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FeedReader.Models
{
    public class RssItem
    {
        public int RssItemId { get; set; }
        public string Title { get; set; }
        public string Link { get; set; }
        public string Description { get; set; }
        public DateTimeOffset PubDate { get; set; }
        public string ImageUrl { get; set; }
    }
}