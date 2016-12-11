import UserItemClassModule = require('implementation/class/userItemClass');

export module Implementation.Class {

    export class ChannelClass implements Model.Class.IChannelClass {

        private addUserItems(userItems: Model.Api.IChannelApiUserItem[], limit: number): void {
            _.forEach(userItems, (userItem) => {
                var existingUserItem = _.find(this.userItems, (item) => {
                    return item.itemGuid === userItem.itemGuid;
                });
                if (existingUserItem) {
                    return;
                }

                var userItemClass = new UserItemClassModule.Implementation.Class.UserItemClass(this.userItemApi, userItem);
                this.userItems.push(userItemClass);
            });
        }

        constructor(
            private channelApi: Model.Api.IChannelApi,
            private userItemApi: Model.Api.IUserItemApi,
            channel: Model.Api.IChannelApiChannel) {
            _.extend(this, channel);
            this.moreUserItems = true;
        }

        channelGuid: string = undefined;
        link: string = undefined;
        moreUserItems: boolean = undefined;
        rss: string = undefined;
        title: string = undefined;
        userItems: Model.Class.IUserItemClass[] = [];

        getMoreUserItems(): Model.Api.IDtoPromise<void> {
            var itemGuid = null;
            var sequence = Number.MAX_VALUE;
            _.forEach(this.userItems, (userItem: Model.Class.IUserItemClass) => {
                if (userItem.sequence > sequence) {
                    return;
                }

                itemGuid = userItem.itemGuid;
                sequence = userItem.sequence;
            });

            var limit = 10;
            return this.channelApi.enumerateUserItems(this.channelGuid, limit, false, itemGuid)
                .then<void>((userItems: Model.Api.IChannelApiUserItem[]) => {
                    this.moreUserItems = userItems.length >= limit;
                    this.addUserItems(userItems, limit);
                });
        }

        refresh(): Model.Api.IDtoPromise<void> {
            var itemGuid = null;
            var sequence = 0;
            _.forEach(this.userItems, (userItem: Model.Class.IUserItemClass) => {
                if (userItem.sequence < sequence) {
                    return;
                }

                itemGuid = userItem.itemGuid;
                sequence = userItem.sequence;
            });

            var limit = 10;
            return this.channelApi.enumerateUserItems(this.channelGuid, limit, true, itemGuid)
                .then<void>((userItems: Model.Api.IChannelApiUserItem[]) => {
                    this.addUserItems(userItems, limit);
                });
        }

        sort(): void {
            this.userItems.sort((a, b) => {
                if (!a.read && b.read) {
                    return -1;
                }

                if (a.read && !b.read) {
                    return 1;
                }

                return b.sequence - a.sequence;
            });

        }

    }
}