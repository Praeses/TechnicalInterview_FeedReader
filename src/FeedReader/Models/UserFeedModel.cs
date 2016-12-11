using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace FeedReader.Models
{
    [Table("UserFeed")]
    public class UserFeedModel
    {
        [Key]
        public int UserFeedID { get; set; }
       
        [HiddenInput(DisplayValue = false)]
        public string UserID { get; set; }

        [Required]
        [MaxLength(255)]
        [Display(Name = "URL")]
        public string Url { get; set; }

        public string Title { get; set; }

        public virtual IEnumerable<Rss> FeedList { get; set; }
    }

    public class UserFeedContext : DbContext
    {
        public UserFeedContext() 
            : base("DefaultConnection")
        {
        }
        public DbSet<UserFeedModel> UserFeedModel { get; set; }
    }
}