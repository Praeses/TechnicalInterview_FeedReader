import ChannelClassModule = require('implementation/class/channelClass');

export module Implementation.Class {

    export class RssClass implements Model.Class.IRssClass {
        constructor(
            private channelApi: Model.Api.IChannelApi,
            private userItemApi: Model.Api.IUserItemApi) {
        }

        channels: Model.Class.IChannelClass[] = [];

        addChannel(rss: string): Model.Api.IDtoPromise<Model.Class.IChannelClass> {
            return this.channelApi.addChannel(rss)
                .then((channel) => {
                    var channelClass = new ChannelClassModule.Implementation.Class.ChannelClass(
                        this.channelApi,
                        this.userItemApi,
                        channel);
                    this.channels.push(channelClass);
                    return channelClass;
                });
        }

        refresh(): Model.Api.IDtoPromise<void> {
            return this.channelApi.enumerateChannels()
                .then<void>((channels) => {
                    this.channels = [];
                    _.forEach(channels, (channel) => {
                        var channelClass = new ChannelClassModule.Implementation.Class.ChannelClass(
                            this.channelApi,
                            this.userItemApi,
                            channel);
                        this.channels.push(channelClass);
                    });
                });
        }

        removeChannel(channel: Model.Class.IChannelClass): void {
            this.channels = _.filter(this.channels, (chan) => chan.channelGuid !== channel.channelGuid);
            this.channelApi.removeChannel(channel.channelGuid);
        }

        sort(): void {
            this.channels.sort((a, b) => {
                var aName = a.title.toLocaleLowerCase();
                var bName = b.title.toLocaleLowerCase();
                return aName === bName ? 0 : aName < bName ? -1 : 1;
            });
        }
    }
}