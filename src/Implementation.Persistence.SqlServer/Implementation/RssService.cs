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
    using Model.Persistence.Interface;

    using Shared.Exceptions;
    using Shared.Extension;

    public class RssService : IRssService
    {
        #region Fields

        private readonly SqlHelper sqlHelper;

        #endregion

        #region Constructors and Destructors

        public RssService(SqlHelper sqlHelper)
        {
            this.sqlHelper = sqlHelper;
        }

        #endregion

        #region Public Methods and Operators

        public void AddItem(Guid channelGuid, IItem item, out bool existed)
        {
            try
            {
                SqlParameter channelGuidParameter = SqlHelper.Parameter(() => channelGuid);
                SqlParameter descriptionParameter = SqlHelper.Parameter(() => item.Description);
                SqlParameter linkParameter = SqlHelper.Parameter("@link", item.Link.AbsoluteUri, typeof(string));
                SqlParameter titleParameter = SqlHelper.Parameter(() => item.Title);
                SqlParameter itemGuidParameter = SqlHelper.Parameter(
                    "@itemGuid",
                    item.ItemGuid,
                    typeof(Guid?),
                    ParameterDirection.InputOutput);
                SqlParameter sequenceParameter = SqlHelper.Parameter<int>("@sequence");
                SqlParameter existedParameter = SqlHelper.Parameter<bool>("@existed");
                int error = this.sqlHelper.ExecuteStoredProcedure(
                    "addItem",
                    channelGuidParameter,
                    descriptionParameter,
                    linkParameter,
                    titleParameter,
                    itemGuidParameter,
                    sequenceParameter,
                    existedParameter);
                switch (error)
                {
                    case 0:
                        item.ItemGuid = itemGuidParameter.CastTo<Guid>();
                        item.Sequence = sequenceParameter.CastTo<int>();
                        existed = existedParameter.CastTo<bool>();
                        return;

                    case 1:
                        throw new NotFoundException("channelGuid");

                    default:
                        throw new ExternalException().AddDumpObject(() => error);
                }
            }
            catch (Exception e)
            {
                e.AddDumpObject(() => item);
                throw;
            }
        }

        public void DeleteChannel(Guid channelGuid)
        {
            try
            {
                SqlParameter channelGuidParameter = SqlHelper.Parameter(() => channelGuid);
                int error = this.sqlHelper.ExecuteStoredProcedure("deleteChannel", channelGuidParameter);
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
                e.AddDumpObject(() => channelGuid);
                throw;
            }
        }

        public IEnumerable<IItem> EnumerateItems(Guid channelGuid, int limit)
        {
            try
            {
                IEnumerable<IItem> items;
                SqlParameter channelGuidParameter = SqlHelper.Parameter(() => channelGuid);
                SqlParameter limitParameter = SqlHelper.Parameter(() => limit);
                int error = this.sqlHelper.ExecuteStoredProcedureItems(
                    "enumerateItems",
                    out items,
                    channelGuidParameter,
                    limitParameter);
                switch (error)
                {
                    case 0:
                        return items;

                    case 1:
                        throw new NotFoundException("channelGuid");

                    default:
                        throw new ExternalException().AddDumpObject(() => error);
                }
            }
            catch (Exception e)
            {
                e.AddDumpObject(() => channelGuid).AddDumpObject(() => limit);
                throw;
            }
        }

        public IChannel GetChannel(Guid channelGuid)
        {
            try
            {
                SqlParameter channelGuidParameter = SqlHelper.Parameter(() => channelGuid);
                SqlParameter lastCheckedParameter = SqlHelper.Parameter<string>("@lastChecked");
                SqlParameter linkParameter = SqlHelper.Parameter<string>("@link");
                SqlParameter rssParameter = SqlHelper.Parameter<string>("@rss");
                SqlParameter titleParameter = SqlHelper.Parameter<string>("@title");
                int error = this.sqlHelper.ExecuteStoredProcedure(
                    "getChannel",
                    channelGuidParameter,
                    lastCheckedParameter,
                    linkParameter,
                    rssParameter,
                    titleParameter);
                switch (error)
                {
                    case 0:
                        return new Channel(
                            channelGuid,
                            new TimeSpan(lastCheckedParameter.CastTo<int>() * TimeSpan.TicksPerSecond),
                            new Uri(linkParameter.CastTo<string>()),
                            new Uri(rssParameter.CastTo<string>()),
                            titleParameter.CastTo<string>());

                    case 1:
                        return default(IChannel);

                    default:
                        throw new ExternalException().AddDumpObject(() => error);
                }
            }
            catch (Exception e)
            {
                e.AddDumpObject(() => channelGuid);
                throw;
            }
        }

        public IItem GetItem(Guid itemGuid)
        {
            try
            {
                SqlParameter itemGuidParameter = SqlHelper.Parameter(() => itemGuid);
                SqlParameter descriptionParameter = SqlHelper.Parameter<string>("@description");
                SqlParameter linkParameter = SqlHelper.Parameter<string>("@link");
                SqlParameter sequenceParameter = SqlHelper.Parameter<int>("@sequence");
                SqlParameter titleParameter = SqlHelper.Parameter<string>("@title");
                int error = this.sqlHelper.ExecuteStoredProcedure(
                    "getItem",
                    itemGuidParameter,
                    descriptionParameter,
                    linkParameter,
                    sequenceParameter,
                    titleParameter);
                switch (error)
                {
                    case 0:
                        return new Item(
                            itemGuid,
                            descriptionParameter.CastTo<string>(),
                            new Uri(linkParameter.CastTo<string>()),
                            sequenceParameter.CastTo<int>(),
                            titleParameter.CastTo<string>());

                    case 1:
                        return default(IItem);

                    default:
                        throw new ExternalException().AddDumpObject(() => error);
                }
            }
            catch (Exception e)
            {
                e.AddDumpObject(() => itemGuid);
                throw;
            }
        }

        public void PutChannel(IChannel channel, out bool existed)
        {
            try
            {
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
                    "putChannel",
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

                    default:
                        throw new ExternalException().AddDumpObject(() => error);
                }
            }
            catch (Exception e)
            {
                e.AddDumpObject(() => channel);
                throw;
            }
        }

        public void RemoveItem(Guid itemGuid)
        {
            try
            {
                SqlParameter itemGuidParameter = SqlHelper.Parameter(() => itemGuid);
                int error = this.sqlHelper.ExecuteStoredProcedure("removeItem", itemGuidParameter);
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
                e.AddDumpObject(() => itemGuid);
                throw;
            }
        }

        #endregion
    }
}