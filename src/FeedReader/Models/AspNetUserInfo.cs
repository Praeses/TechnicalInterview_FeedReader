using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace FeedReader.Models
{
    public class AspNetUserInfo
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Display(Name = "Account ID")]
        public string UserId { get; set; }

        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name="Last Name")]
        public string LastName { get; set; }
    }

    public class AspNetUserFeed
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Display(Name = "Account ID")]
        public string AccountId { get; set; }

        [Display(Name = "Feed ID")]
        public string FeedId { get; set; }

        [Display(Name = "Feed Name")]
        public string FeedName { get; set; }

        [Display(Name = "Title")]
        public string Title { get; set; }

        [Display(Name="Info")]
        public string Info { get; set; }
    }

    public class AspNetFeed
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Display(Name = "Name")]
        public string Name { get; set; }
        [Display(Name = "URL")]
        public string FeedUrl { get; set; }
        [Display(Name = "Post Id")]
        public string RecentPostId { get; set; }
        [DataType(DataType.Date)]
        [Display(Name="Date Posted")]
        public string DatePosted { get; set; }
        [Display(Name = "Title")]
        public string Title { get; set; }
        [Display(Name = "Info")]
        public string Info { get; set; }
    }

    public class AspNetPost
    {
        [Key]
        [Required]
        public int Id { set; get; }
        [Display(Name = "Feed ID")]
        public string FeedId { get; set; }
        [Display(Name = "Author")]
        public string Author { get; set; }
        [Display(Name = "Title")]
        public string Title { get; set; }
        [Display(Name = "Body")]
        public string Body { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Date")]
        public string Date { get; set; }
    }
}