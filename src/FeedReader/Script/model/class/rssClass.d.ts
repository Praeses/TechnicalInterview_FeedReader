declare module Model.Class {

    interface IRssClass {
        channels: IChannelClass[];

        addChannel(rss: string): Model.Api.IDtoPromise<IChannelClass>;
        refresh(): Model.Api.IDtoPromise<void>;
        removeChannel(channel: IChannelClass): void;
        sort(): void;
    }
}