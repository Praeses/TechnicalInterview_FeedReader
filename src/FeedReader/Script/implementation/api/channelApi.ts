/// <reference path='../../model/api/authenticationApi.d.ts' />
import UrlModule = require('implementation/base/url');

export module Implementation.Api {
    export class ChannelApi implements Model.Api.IChannelApi {

        constructor(private dto: Model.Api.IDto) {}

        addChannel(rss: string): Model.Api.IDtoPromise<Model.Api.IChannelApiChannel> {
            return this.dto.send<IChannelApiAddChannel, Model.Api.IChannelApiChannel>(
                new ChannelApiAddChannelRequest(rss));
        }

        enumerateChannels(): Model.Api.IDtoPromise<Model.Api.IChannelApiChannel[]> {
            return this.dto.send<void, Model.Api.IChannelApiChannel[]>(
                new ChannelApiEnumerateChannelsRequest());
        }

        enumerateUserItems(channelGuid: string, limit: number, before: boolean, itemGuid: string): Model.Api.IDtoPromise<Model.Api.IChannelApiUserItem[]> {
            return this.dto.send<IChannelApiEnumerateUserItemsAfter, Model.Api.IChannelApiUserItem[]>(
                new ChannelApiEnumerateUserItemsRequest(channelGuid, limit, before, itemGuid));
        }

        removeChannel(channelGuid: string): Model.Api.IDtoPromise<void> {
            return this.dto.send<IChannelApiRemoveChannel, void>(
                new ChannelApiRemoveChannelRequest(channelGuid));
        }

    }

    // Channel Dtos
    class ChannelApiAddChannelRequest implements Model.Api.IDtoRequest<IChannelApiAddChannel, Model.Api.IChannelApiChannel> {
        constructor(private rss: string) {
            this.data = {
                rss: rss,
            };
            this.method = "POST";
            this.url = new UrlModule.Implementation.Base.Url('/api/channel');
        }

        data: IChannelApiAddChannel;
        method: string;
        url: Model.Base.IUrl;
    }

    class ChannelApiEnumerateChannelsRequest implements Model.Api.IDtoRequest<void, Model.Api.IChannelApiChannel[]> {
        constructor() {
            this.method = "GET";
            this.url = new UrlModule.Implementation.Base.Url('/api/channel');
        }

        data: void;
        method: string;
        url: Model.Base.IUrl;
    }

    class ChannelApiEnumerateUserItemsRequest implements Model.Api.IDtoRequest<IChannelApiEnumerateUserItemsAfter, Model.Api.IChannelApiUserItem[]> {
        constructor(channelGuid: string, limit: number, before: boolean, itemGuid: string) {
            this.data = {
                channelGuid: channelGuid,
                limit: limit,
                before: before,
                itemGuid: itemGuid
            };
            this.method = "GET";
            this.url = new UrlModule.Implementation.Base.Url('/api/channel/{channelGuid}');
        }

        data: IChannelApiEnumerateUserItemsAfter;
        method: string;
        url: Model.Base.IUrl;
    }

    class ChannelApiRemoveChannelRequest implements Model.Api.IDtoRequest<IChannelApiRemoveChannel, void> {
        constructor(channelGuid: string) {
            this.data = {
                channelGuid: channelGuid
            };
            this.method = "DELETE";
            this.url = new UrlModule.Implementation.Base.Url('/api/channel/{channelGuid}');
        }

        data: IChannelApiRemoveChannel;
        method: string;
        url: Model.Base.IUrl;
    }

    interface IChannelApiAddChannel {
        rss: string;
    }

    interface IChannelApiEnumerateUserItemsAfter {
        channelGuid: string;
        limit: number;
        itemGuid: string;
    }

    interface IChannelApiRemoveChannel {
        channelGuid: string;
    }

}