using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

using Microsoft.AspNet.Identity;


namespace FeedReader.Utils
{
    public interface IUserContext
    {
        string UserId { get; }
    }

    public class UserContext : IUserContext
    {
        public string UserId
        {
            get
            {
                return HttpContext.Current.User.Identity.GetUserId();
            }
        }
    }
}
