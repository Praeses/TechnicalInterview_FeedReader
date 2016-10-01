using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FeedReader.Models.Helpers
{
    public class UserDataHelper
    {
        public ApplicationUser retireveUserFromDb(string userId)
        {
            ApplicationUser applicationUser = null;

            using (var db = new FeedReaderContext())
            {
               applicationUser = db.Users.Find(userId);
            }

            return applicationUser;
        }
    }
}