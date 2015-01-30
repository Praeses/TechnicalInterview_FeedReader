using Dapper;
using FeedReader.Models.Interfaces;
using FeedReader.Models.Views;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeedReader.Data
{
    public class SqlFeedRepository : IFeedRepository
    {
        public List<FeedUrl> GetFeedUrlsByUserId(string id)
        {
            using (var cn = new SqlConnection(Settings.GetConnectionString()))
            {
                var p = new DynamicParameters();
                p.Add("@AspNetId", id);
                return cn.Query<FeedUrl>("FeedGetAllByUserId", p, commandType: CommandType.StoredProcedure).ToList();
            }
        }

        public List<Feed> SearchFeeds(string id, string searchEntry)
        {
            using (var cn = new SqlConnection(Settings.GetConnectionString()))
            {
                var p = new DynamicParameters();
                p.Add("@AspNetID", id);
                p.Add("@SearchEntry", searchEntry);
                return cn.Query<Feed>("FeedSearch", p, commandType: CommandType.StoredProcedure).ToList();
            }
        }


        public int AddFeed(Feed feed)
        {
            using (var cn = new SqlConnection(Settings.GetConnectionString()))
            {
                var p = new DynamicParameters();
                p.Add("@Url", feed.RssUrl);
                p.Add("@Description", feed.Description);
                p.Add("@LastBuildDate", feed.LastBuildDate);
                p.Add("@Subscribers", feed.Subscribers);
                p.Add("@Title", feed.Title);
                p.Add("@SiteUrl", feed.Url);
                return cn.Query<int>("FeedInsert", p, commandType: CommandType.StoredProcedure).First();
            }
        }

        public void UpdateUserInfoFeeds(string userId, int feedId)
        {
            using (var cn = new SqlConnection(Settings.GetConnectionString()))
            {
                var p = new DynamicParameters();
                p.Add("@AspNetId", userId);
                p.Add("@FeedID", feedId);
                
                cn.Execute("UpdateUserInfoFeeds", p, commandType: CommandType.StoredProcedure);
            }
        }


        public void RemovedUserInfoFeeds(string userId, int feedId)
        {
            using (var cn = new SqlConnection(Settings.GetConnectionString()))
            {
                var p = new DynamicParameters();
                p.Add("@AspNetId", userId);
                p.Add("@FeedID", feedId);

                cn.Execute("RemoveUserInfoFeeds", p, commandType: CommandType.StoredProcedure);
            }
        }
    }
}
