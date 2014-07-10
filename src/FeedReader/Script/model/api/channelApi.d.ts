declare module Model.Api {

    interface IChannelApi {
        addChannel(rss: string): IDtoPromise<IChannelApiChannel>;
        enumerateChannels(): IDtoPromise<IChannelApiChannel[]>;
        enumerateUserItems(channelGuid: string, limit: number, before: boolean, itemGuid: string): IDtoPromise<IChannelApiUserItem[]>;
        removeChannel(channelGuid: string): IDtoPromise<void>;
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