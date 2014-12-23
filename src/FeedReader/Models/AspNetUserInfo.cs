using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FeedReader.Models
{
    public class AspNetUserInfo
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }
    }

    public class AspNetUserFeed
    {
        public int Id { get; set; }

        public string UserId { get; set; }
        public virtual AspNetUserInfo AspNetUserInfo { get; set; }

        public string FeedId { get; set; }
        public virtual AspNetFeed AspNetFeed{ get; set; }
    }

    public class AspNetFeed
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public string FeedUrl { get; set; }
        public string Date { get; set; }
    }
}