﻿using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System;
using System.Collections;
using System.Collections.Generic;

namespace FeedReader.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class Feed
    {
        [Required]
        public int FeedId { get; set; }
        
        public string UserId { get; set; }

        [Required]
        [Url(ErrorMessage = "The url does not appear to be valid")]
        public string URL { get; set; }
        
        [Url(ErrorMessage = "The url does not appear to be valid")]
        public string Image { get; set; }

        public string Title { get; set; }

        public virtual ICollection<FeedItem> FeedItems { get; set; }
    }

    public class FeedItem
    {
        public int FeedId { get; set; }
        public virtual Feed Feed { get; set; }

        public int FeedItemId { get; set; }

        [Url]
        public string Image { get; set; }
        public string Description { get; set; }
        public string Title { get; set; }
        public DateTime? PublishedDate { get; set; }

        [Url]
        public string URL { get; set; }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public DbSet<Feed> Feeds { get; set; }
        public DbSet<FeedItem> FeedItems { get; set; }
    }
}