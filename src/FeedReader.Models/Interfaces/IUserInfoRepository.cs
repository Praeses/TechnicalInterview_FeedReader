using FeedReader.Models.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeedReader.Models.Interfaces
{
    public interface IUserInfoRepository
    {
        void CreateUserInfo(UserInfo model);
    }
}
