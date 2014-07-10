namespace Implementation.Persistence.SqlServer.Implementation
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Runtime.InteropServices;

    using global::Implementation.Persistence.SqlServer.Class;
    using global::Implementation.Persistence.SqlServer.Extension;

    using Model.Persistence.Class;
    using Model.Persistence.Enum;
    using Model.Persistence.Interface;

    using Shared.Exceptions;
    using Shared.Extension;

    public class UserService : IUserService
    {
        #region Fields

        private readonly SqlHelper sqlHelper;

        #endregion

        #region Constructors and Destructors

        public UserService(SqlHelper sqlHelper)
        {
            this.sqlHelper = sqlHelper;
        }

        #endregion

        #region Public Methods and Operators

        public void AddChannel(Guid userGuid, IChannel channel, out bool existed)
        {
            try
            {
                SqlParameter userGuidParameter = SqlHelper.Parameter(() => userGuid);
                SqlParameter linkParameter = SqlHelper.Parameter("@link", channel.Link.AbsoluteUri, typeof(string));
                SqlParameter rssParameter = SqlHelper.Parameter("@rss", channel.Rss.AbsoluteUri, typeof(string));
                SqlParameter titleParameter = SqlHelper.Parameter(() => channel.Title);
                SqlParameter channelGuidParameter = SqlHelper.Parameter(
                    "@channelGuid",
                    channel.ChannelGuid,
                    typeof(Guid?),
                    ParameterDirection.InputOutput);
                SqlParameter lastCheckedParameter = SqlHelper.Parameter<string>("@lastChecked");
                SqlParameter existedParameter = SqlHelper.Parameter<bool>("@existed");
                int error = this.sqlHelper.ExecuteStoredProcedure(
                    "addChannel",
                    userGuidParameter,
                    linkParameter,
                    rssParameter,
                    titleParameter,
                    channelGuidParameter,
                    lastCheckedParameter,
                    existedParameter);
                switch (error)
                {
                    case 0:
                        channel.ChannelGuid = channelGuidParameter.CastTo<Guid>();
                        channel.LastChecked = new TimeSpan(lastCheckedParameter.CastTo<int>() * TimeSpan.TicksPerSecond);
                        existed = existedParameter.CastTo<bool>();
                        return;

                    case 1:
                        throw new NotFoundException("userGuid");

                    default:
                        throw new ExternalException().AddDumpObject(() => error);
                }
            }
            catch (Exception e)
            {
                e.AddDumpObject(() => userGuid).AddDumpObject(() => channel);
                throw;
            }
        }

        public void AddToken(Guid userGuid, IToken token, out bool existed)
        {
            try
            {
                SqlParameter userGuidParameter = SqlHelper.Parameter(() => userGuid);
                SqlParameter tokenNameParameter = SqlHelper.Parameter(() => token.TokenName);
                SqlParameter tokenTypeParameter = SqlHelper.Parameter(() => token.TokenType);
                SqlParameter tokenCreatedParameter = SqlHelper.Parameter<DateTimeOffset>("@tokenCreated");
                SqlParameter tokenGuidParameter = SqlHelper.Parameter<Guid>("@tokenGuid");
                SqlParameter existedParameter = SqlHelper.Parameter<bool>("@existed");
                int error = this.sqlHelper.ExecuteStoredProcedure(
                    "addToken",
                    userGuidParameter,
                    tokenNameParameter,
                    tokenTypeParameter,
                    tokenCreatedParameter,
                    tokenGuidParameter,
                    existedParameter);
                switch (error)
                {
                    case 0:
                        token.Created = tokenCreatedParameter.CastTo<DateTimeOffset>();
                        token.TokenGuid = tokenGuidParameter.CastTo<Guid>();
                        existed = existedParameter.CastTo<bool>();
                        return;

                    case 1:
                        throw new NotFoundException("userGuid");

                    default:
                        throw new ExternalException().AddDumpObject(() => error);
                }
            }
            catch (Exception e)
            {
                e.AddDumpObject(() => userGuid).AddDumpObject(() => token);
                throw;
            }
        }

        public void DeleteUser(Guid userGuid)
        {
            try
            {
                SqlParameter userGuidParameter = SqlHelper.Parameter(() => userGuid);
                int error = this.sqlHelper.ExecuteStoredProcedure("deleteUser", userGuidParameter);
                switch (error)
                {
                    case 0:
                        return;

                    default:
                        throw new ExternalException().AddDumpObject(() => error);
                }
            }
            catch (Exception e)
            {
                e.AddDumpObject(() => userGuid);
                throw;
            }
        }

        public IEnumerable<IChannel> EnumerateChannels(Guid userGuid)
        {
            try
            {
                IEnumerable<IChannel> channels;
                SqlParameter userGuidParameter = SqlHelper.Parameter(() => userGuid);
                int error = this.sqlHelper.ExecuteStoredProcedureChannels(
                    "enumerateChannels",
                    out channels,
                    userGuidParameter);
                switch (error)
                {
                    case 0:
                        return channels;

                    case 1:
                        throw new NotFoundException("userGuid");

                    default:
                        throw new ExternalException().AddDumpObject(() => error);
                }
            }
            catch (Exception e)
            {
                e.AddDumpObject(() => userGuid);
                throw;
            }
        }

        public IEnumerable<IUserItem> EnumerateUserItems(
            Guid userGuid,
            Guid channelGuid,
            int limit,
            bool before,
            Guid? itemGuid)
        {
            try
            {
                IEnumerable<IUserItem> userItems;
                SqlParameter userGuidParameter = SqlHelper.Parameter(() => userGuid);
                SqlParameter channelGuidParameter = SqlHelper.Parameter(() => channelGuid);
                SqlParameter limitParameter = SqlHelper.Parameter(() => limit);
                SqlParameter beforeParameter = SqlHelper.Parameter(() => before);
                SqlParameter itemGuidParameter = SqlHelper.Parameter(() => itemGuid);
                int error = this.sqlHelper.ExecuteStoredProcedureUserItems(
                    "enumerateUserItems",
                    out userItems,
                    userGuidParameter,
                    channelGuidParameter,
                    limitParameter,
                    beforeParameter,
                    itemGuidParameter);
                switch (error)
                {
                    case 0:
                        return userItems;

                    case 1:
                        throw new NotFoundException("userGuid");

                    case 2:
                        throw new NotFoundException("channelGuid");

                    case 3:
                        throw new NotFoundException("itemGuid");

                    default:
                        throw new ExternalException().AddDumpObject(() => error);
                }
            }
            catch (Exception e)
            {
                e.AddDumpObject(() => userGuid)
                    .AddDumpObject(() => channelGuid)
                    .AddDumpObject(() => limit)
                    .AddDumpObject(() => before)
                    .AddDumpObject(() => itemGuid);
                throw;
            }
        }

        public IUser GetUser(string userName)
        {
            try
            {
                SqlParameter userNameParameter = SqlHelper.Parameter(() => userName);
                SqlParameter hashedPasswordParameter = SqlHelper.Parameter<string>("@hashedPassword");
                SqlParameter userGuidParameter = SqlHelper.Parameter<Guid>("@userGuid");
                int error = this.sqlHelper.ExecuteStoredProcedure(
                    "getUserByName",
                    userNameParameter,
                    hashedPasswordParameter,
                    userGuidParameter);
                switch (error)
                {
                    case 0:
                        return new User(
                            userGuidParameter.CastTo<Guid>(),
                            userName,
                            hashedPasswordParameter.CastTo<string>());

                    case 1:
                        return default(IUser);

                    default:
                        throw new ExternalException().AddDumpObject(() => error);
                }
            }
            catch (Exception e)
            {
                e.AddDumpObject(() => userName);
                throw;
            }
        }

        public IUser GetUser(Guid tokenGuid, out IToken token)
        {
            try
            {
                SqlParameter tokenGuidParameter = SqlHelper.Parameter(() => tokenGuid);
                SqlParameter hashedPasswordParameter = SqlHelper.Parameter<string>("@hashedPassword");
                SqlParameter userGuidParameter = SqlHelper.Parameter<Guid>("@userGuid");
                SqlParameter userNameParameter = SqlHelper.Parameter<string>("@userName");
                SqlParameter tokenCreatedParameter = SqlHelper.Parameter<DateTimeOffset>("@tokenCreated");
                SqlParameter tokenNameParameter = SqlHelper.Parameter<string>("@tokenName");
                SqlParameter tokenTypeParameter = SqlHelper.Parameter<int>("@tokenType");
                int error = this.sqlHelper.ExecuteStoredProcedure(
                    "getUserByToken",
                    tokenGuidParameter,
                    hashedPasswordParameter,
                    userGuidParameter,
                    userNameParameter,
                    tokenCreatedParameter,
                    tokenNameParameter,
                    tokenTypeParameter);
                switch (error)
                {
                    case 0:
                        token = new Token(
                            tokenCreatedParameter.CastTo<DateTimeOffset>(),
                            tokenGuid,
                            tokenNameParameter.CastTo<string>(),
                            (TokenType)tokenTypeParameter.CastTo<int>());
                        return new User(
                            userGuidParameter.CastTo<Guid>(),
                            userNameParameter.CastTo<string>(),
                            hashedPasswordParameter.CastTo<string>());

                    case 1:
                        token = default(IToken);
                        return default(IUser);

                    default:
                        throw new ExternalException().AddDumpObject(() => error);
                }
            }
            catch (Exception e)
            {
                e.AddDumpObject(() => tokenGuid);
                throw;
            }
        }

        public void PutUser(IUser user, out bool existed)
        {
            try
            {
                SqlParameter hashedPasswordParameter = SqlHelper.Parameter(() => user.HashedPassword);
                SqlParameter userNameParameter = SqlHelper.Parameter(() => user.UserName);
                SqlParameter userGuidParameter = SqlHelper.Parameter(
                    "@userGuid",
                    user.UserGuid,
                    typeof(Guid?),
                    ParameterDirection.InputOutput);
                SqlParameter existedParameter = SqlHelper.Parameter<bool>("@existed");
                int error = this.sqlHelper.ExecuteStoredProcedure(
                    "putUser",
                    hashedPasswordParameter,
                    userNameParameter,
                    userGuidParameter,
                    existedParameter);
                switch (error)
                {
                    case 0:
                        user.UserGuid = userGuidParameter.CastTo<Guid>();
                        existed = existedParameter.CastTo<bool>();
                        return;

                    default:
                        throw new ExternalException().AddDumpObject(() => error);
                }
            }
            catch (Exception e)
            {
                e.AddDumpObject(() => user);
                throw;
            }
        }

        public void PutUserItem(Guid userGuid, Guid itemGuid, bool read, out bool existed)
        {
            try
            {
                SqlParameter userGuidParameter = SqlHelper.Parameter(() => userGuid);
                SqlParameter itemGuidParameter = SqlHelper.Parameter(() => itemGuid);
                SqlParameter readParameter = SqlHelper.Parameter(() => read);
                SqlParameter existedParameter = SqlHelper.Parameter<bool>("@existed");
                int error = this.sqlHelper.ExecuteStoredProcedure(
                    "putUserItem",
                    userGuidParameter,
                    itemGuidParameter,
                    readParameter,
                    existedParameter);
                switch (error)
                {
                    case 0:
                        existed = existedParameter.CastTo<bool>();
                        return;

                    case 1:
                        throw new NotFoundException("userGuid");

                    case 2:
                        throw new NotFoundException("itemGuid");

                    default:
                        throw new ExternalException().AddDumpObject(() => error);
                }
            }
            catch (Exception e)
            {
                e.AddDumpObject(() => userGuid).AddDumpObject(() => itemGuid).AddDumpObject(() => read);
                throw;
            }
        }

        public void RemoveChannel(Guid userGuid, Guid channelGuid)
        {
            try
            {
                SqlParameter userGuidParameter = SqlHelper.Parameter(() => userGuid);
                SqlParameter channelGuidParameter = SqlHelper.Parameter(() => channelGuid);
                int error = this.sqlHelper.ExecuteStoredProcedure(
                    "removeChannel",
                    userGuidParameter,
                    channelGuidParameter);
                switch (error)
                {
                    case 0:
                        return;

                    case 1:
                        throw new NotFoundException("userGuid");

                    default:
                        throw new ExternalException().AddDumpObject(() => error);
                }
            }
            catch (Exception e)
            {
                e.AddDumpObject(() => userGuid).AddDumpObject(() => channelGuid);
                throw;
            }
        }

        public void RemoveToken(Guid tokenGuid)
        {
            try
            {
                SqlParameter tokenGuidParameter = SqlHelper.Parameter(() => tokenGuid);
                int error = this.sqlHelper.ExecuteStoredProcedure("removeToken", tokenGuidParameter);
                switch (error)
                {
                    case 0:
                        return;

                    default:
                        throw new ExternalException().AddDumpObject(() => error);
                }
            }
            catch (Exception e)
            {
                e.AddDumpObject(() => tokenGuid);
                throw;
            }
        }

        #endregion
    }
}