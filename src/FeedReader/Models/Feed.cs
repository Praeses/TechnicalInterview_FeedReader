using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace FeedReader.Models
{
    public class Feed
    {
        public int id { get; set; }
        public String user_id { get; set; }
        [Required]
        public string channel { get; set; }
        [Required]
        public String link { get; set;}     
        public String desc { get; set; }

    }
}