namespace Model.Persistence.Extension
{
    using System;

    using Model.Persistence.Interface;

    public static class UserServiceExtension
    {
        #region Public Methods and Operators

        public static void AddChannel(this IUserService @this, Guid userGuid, IChannel channel)
        {
            bool existed;
            @this.AddChannel(userGuid, channel, out existed);
        }

        public static void AddToken(this IUserService @this, Guid userGuid, IToken token)
        {
            bool existed;
            @this.AddToken(userGuid, token, out existed);
        }

        public static IUser GetUser(this IUserService @this, Guid tokenGuid)
        {
            IToken token;
            return @this.GetUser(tokenGuid, out token);
        }

        public static void PutUser(this IUserService @this, IUser user)
        {
            bool existed;
            @this.PutUser(user, out existed);
        }

        public static void PutUserItem(this IUserService @this, Guid userGuid, Guid itemGuid, bool read)
        {
            bool existed;
            @this.PutUserItem(userGuid, itemGuid, read, out existed);
        }

        #endregion
    }
}