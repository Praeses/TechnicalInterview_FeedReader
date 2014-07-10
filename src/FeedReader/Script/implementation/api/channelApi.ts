/// <reference path='../../model/api/authenticationApi.d.ts' />
import UrlModule = require('implementation/base/url');

export module Implementation.Api {
    export class ChannelApi implements Model.Api.IChannelApi {

        constructor(private dto: Model.Api.IDto) {}

        addChannel(rss: string): JQueryPromise<Model.Api.IChannelApiChannel> {
            return this.dto.send<IChannelApiAddChannel, Model.Api.IChannelApiChannel>(
                    new ChannelApiAddChannelRequest(rss))
                .then((response) => {
                    return response.data;
                });
        }

        enumerateChannels(): JQueryPromise<Model.Api.IChannelApiChannel[]> {
            return this.dto.send<void, Model.Api.IChannelApiChannel[]>(
                    new ChannelApiEnumerateChannelsRequest())
                .then((response) => {
                    return response.data;
                });
        }

        enumerateUserItemsAfter(channelGuid: string, limit: number, itemGuid: string): JQueryPromise<Model.Api.IChannelApiUserItem[]> {
            return this.dto.send<IChannelApiEnumerateUserItemsAfter, Model.Api.IChannelApiUserItem[]>(
                    new ChannelApiEnumerateUserItemsAfterRequest(channelGuid, limit, itemGuid))
                .then((response) => {
                    return response.data;
                });
        }

        removeChannel(channelGuid: string): JQueryPromise<void> {
            return this.dto.send<IChannelApiRemoveChannel, void>(
                    new ChannelApiRemoveChannelRequest(channelGuid))
                .then(() => {});
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

    class ChannelApiEnumerateUserItemsAfterRequest implements Model.Api.IDtoRequest<IChannelApiEnumerateUserItemsAfter, Model.Api.IChannelApiUserItem[]> {
        constructor(channelGuid: string, limit: number, itemGuid: string) {
            this.data = {
                channelGuid: channelGuid,
                limit: limit,
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