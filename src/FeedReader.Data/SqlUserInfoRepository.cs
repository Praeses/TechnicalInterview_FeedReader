using Dapper;
using FeedReader.Models.Interfaces;
using FeedReader.Models.Tables;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeedReader.Data
{
    public class SqlUserInfoRepository : IUserInfoRepository
    {
        public void CreateUserInfo(UserInfo model)
        {
            using(var cn = new SqlConnection(Settings.GetConnectionString()))
            {
                var p = new DynamicParameters();

                p.Add("@FirstName", model.FirstName);
                p.Add("@LastName", model.LastName);
                p.Add("@Email", model.Email);
                p.Add("@AspNetID", model.AspNetID);
                p.Add("@UserName", model.UserName);

                cn.Execute("UserInfoInsert", p, commandType: CommandType.StoredProcedure);
            }
        }
    }
}
