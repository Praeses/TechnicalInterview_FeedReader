using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FeedReader.Models
{
    public class RssFeed
    {
        public string UserId{ get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Link { get; set; }

        public virtual ApplicationUser User { get; set; }
    }
}