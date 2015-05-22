using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FeedReader.Models
{
    public class FeedArticle
    {
        public String title { get; set; }
        public String desc { get; set; }
        public String link { get; set; }
        public String authors { get; set; }
        public String publishDate { get; set; }
    }
}