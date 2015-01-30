using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace FeedReader.UI.Migrations
{
    using FeedReader.Data;
    using FeedReader.Models.Interfaces;
    using FeedReader.Models.Tables;
    using FeedReader.UI.Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<ApplicationDbContext>
    {
        private static IUserInfoRepository _userInfoRepository = new SqlUserInfoRepository();

        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(ApplicationDbContext context)
        {
            var mgr = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            CreateUsers(mgr);
        }

        private void CreateUsers(UserManager<ApplicationUser> mgr)
        {
            var user = new ApplicationUser()
            {
                UserName = "Fry",
                Email = "frenchfry@email.com"
            };

            mgr.Create(user, "testing123");
            var userInfo = new UserInfo()
            {
                AspNetID = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                FirstName = "Fry",
                LastName = "Something"
            };

            _userInfoRepository.CreateUserInfo(userInfo);

            var user2 = new ApplicationUser()
            {
                UserName = "Leela",
                Email = "cyclops@email.com"
            };

            mgr.Create(user2, "testing123");
            var userInfo2 = new UserInfo()
            {
                AspNetID = user2.Id,
                UserName = user2.UserName,
                Email = user2.Email,
                FirstName = "Leela",
                LastName = "Cyclops"
            };

            _userInfoRepository.CreateUserInfo(userInfo2);

            var user3 = new ApplicationUser()
            {
                UserName = "Zoidberg",
                Email = "lobster@email.com"
            };

            mgr.Create(user3, "testing123");
            var userInfo3 = new UserInfo()
            {
                AspNetID = user3.Id,
                UserName = user3.UserName,
                Email = user3.Email,
                FirstName = "Zoidberg",
                LastName = "Lobster"
            };

            _userInfoRepository.CreateUserInfo(userInfo3);

            var user4 = new ApplicationUser()
            {
                UserName = "Amy",
                Email = "amy@email.com"
            };

            mgr.Create(user4, "testing123");
            var userInfo4 = new UserInfo()
            {
                AspNetID = user4.Id,
                UserName = user4.UserName,
                Email = user4.Email,
                FirstName = "Amy",
                LastName = "Chan"
            };

            _userInfoRepository.CreateUserInfo(userInfo4);

            var user5 = new ApplicationUser()
            {
                UserName = "Bender",
                Email = "robot@email.com"
            };

            mgr.Create(user5, "testing123");
            var userInfo5 = new UserInfo()
            {
                AspNetID = user5.Id,
                UserName = user5.UserName,
                Email = user5.Email,
                FirstName = "Bender"
            };

            _userInfoRepository.CreateUserInfo(userInfo5);

            var user6 = new ApplicationUser()
            {
                UserName = "Professor",
                Email = "oldguy@email.com"
            };

            mgr.Create(user6, "testing123");
            var userInfo6 = new UserInfo()
            {
                AspNetID = user6.Id,
                UserName = user6.UserName,
                Email = user6.Email,
                FirstName = "Professor",
                LastName = "OldGuy"
            };

            _userInfoRepository.CreateUserInfo(userInfo6);

            var user7 = new ApplicationUser()
            {
                UserName = "Nibbler",
                Email = "nibbler@email.com"
            };

            mgr.Create(user7, "testing123");
            var userInfo7 = new UserInfo()
            {
                AspNetID = user7.Id,
                UserName = user7.UserName,
                Email = user7.Email,
                FirstName = "Nibbler"
            };

            _userInfoRepository.CreateUserInfo(userInfo7);

            var user8 = new ApplicationUser()
            {
                UserName = "Zap",
                Email = "captain@email.com"
            };

            mgr.Create(user8, "testing123");
            var userInfo8 = new UserInfo()
            {
                AspNetID = user8.Id,
                UserName = user8.UserName,
                Email = user8.Email,
                FirstName = "Zap",
                LastName = "Brannigan"
            };

            _userInfoRepository.CreateUserInfo(userInfo8);
        }
    }
}
