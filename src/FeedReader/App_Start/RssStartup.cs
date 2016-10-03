using FeedReader.Models;
using FeedReader.Providers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace FeedReader.App_Start
{
    public static class RssStartup
    {
        public static void Init()
        {
            
            /*if (false)
            {
                XDocument document = XDocument.Load(new StreamReader(System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/rssowl.opml")));

                var entries = document.Root.Descendants("outline").Where(a => a.Attribute("xmlUrl") != null);

                RssContext dbContext = new RssContext();
                ICollection<ApplicationUser> users = dbContext.Users.ToList();
                dbContext.Dispose();

                foreach (ApplicationUser user in users)
                {
                    foreach (var entry in entries)
                    {
                        string xmlUrl = entry.Attribute("xmlUrl").Value;

                        RssManager manager = new RssManager(user.Id);
                        try
                        {
                            manager.AddSubscription(xmlUrl);
                        }
                        catch (Exception e) { }
                    }
                }
            }*/
           
        }
    }
}