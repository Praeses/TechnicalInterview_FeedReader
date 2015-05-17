using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FeedReader.Models
{
    public class Feed
    {
        public int id { get; set; }
        public String user_id { get; set; }
        public string channel { get; set; }
        public String link { get; set;}     
        public String desc { get; set; }

    }
}