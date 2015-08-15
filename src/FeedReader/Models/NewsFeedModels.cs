using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace FeedReader.Models
{
    public class NewsFeed : IComparable
    {
        public NewsFeed()
        {
            this.Items = new List<NewsFeedItem>();
        }

        public int NewsFeedID { get; set; }

        [Required]
        public string Category { get; set; }
       
        public virtual ICollection<NewsFeedItem> Items { get; set; }

        public virtual ICollection<ApplicationUser> ApplicationUsers { get; set; }

        public int CompareTo(object obj)
        {
            if (obj is NewsFeed)
            {
                NewsFeed newsFeedToCompare = (NewsFeed)obj;

                return this.Category.CompareTo(newsFeedToCompare.Category);

            }
            else
            {
                throw new ArgumentException("Object is not a NewsFeed");
            }
        }
    }

    public class NewsFeedItem : IComparable
    {
        public int NewsFeedItemID { get; set; }        

        [Required]
        [Display(Name = "Category")]
        public int NewsFeedID { get; set; }
        public virtual NewsFeed NewsFeed { get; set; }

        [Required]
        [StringLength(144, ErrorMessage = "Title cannot be longer than 144 characters.")]
        public string Title { get; set; }

        [Required]
        [StringLength(1024, ErrorMessage = "Fitzgerald Flash cannot be longer than 1024 characters.")]
        [Display(Name = "Fitzgerald Flash")]
        public string Description { get; set; }
        
        [DataType(DataType.DateTime)]
        [Display(Name="Date Added")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DateAdded { get; set; }

        [Display(Name="Created By")]
        [StringLength(144)]
        public string CreatedBy { get; set; }

        public int CompareTo(object obj)
        {
            if (obj is NewsFeedItem)
            {
                NewsFeedItem newsFeedItemToCompare = (NewsFeedItem)obj;

                return this.DateAdded.CompareTo(newsFeedItemToCompare.DateAdded);

            }
            else
            {
                throw new ArgumentException("Object is not a NewsFeedItem");
            }
        }
    }
}