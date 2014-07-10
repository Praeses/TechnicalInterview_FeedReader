declare module Model.Api {

    interface IChannelApi {
        addChannel(rss: string): JQueryPromise<IChannelApiChannel>;
        enumerateChannels(): JQueryPromise<IChannelApiChannel[]>;
        enumerateUserItemsAfter(channelGuid: string, limit: number, itemGuid: string): JQueryPromise<IChannelApiUserItem[]>;
        removeChannel(channelGuid: string): JQueryPromise<void>;
    }

    interface IChannelApiChannel {
        channelGuid: string;
        link: string;
        rss: string;
        title: string;
    }

    export interface IChannelApiUserItem {
        description: string;
        itemGuid: string;
        link: string;
        read: boolean;
        sequence: number;
        title: string;
    }

}