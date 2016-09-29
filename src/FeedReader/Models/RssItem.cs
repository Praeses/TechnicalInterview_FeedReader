using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FeedReader.Models
{
    public class RssItem
    {
        public RssChannel Channel { get; set; }

        public int RssChannelId { get; set; }

        [Key]
        [MaxLength(400)]
        public string Title { get; set; }

        [MaxLength(2083)] //max length of url in ie
        public string Link { get; set; }

        [MaxLength]
        public string Description { get; set; }
        public DateTimeOffset PubDate { get; set; }
        public string ImageUrl { get; set; }
    }
}