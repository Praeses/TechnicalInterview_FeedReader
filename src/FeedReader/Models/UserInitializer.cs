using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace FeedReader.Models
{
    public class UserInitializer : DropCreateDatabaseIfModelChanges<UserContext>
    {
        protected override void Seed(UserContext context)
        {
            var users = new List<AspNetUserInfo>{
                new AspNetUserInfo{UserId = "20ffeddf-1b60-4f4e-bf4e-2c6c95f05778", FirstName = "Test", LastName = "User"},
            };

            foreach (var temp in users)
            {
                context.AspNetUserInfo.Add(temp);
            }
            context.SaveChanges();
        }
        
    }
}