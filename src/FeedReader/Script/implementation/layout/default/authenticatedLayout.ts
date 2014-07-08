/// <reference path='../../../model/base/authenticatedLayout.d.ts' />
/// <amd-dependency path='text!implementation/layout/default/authenticatedLayout.html' />
import LayoutModule = require('implementation/base/layout');

export module Implementation.Layout.Default {

    export class AuthenticatedLayout extends LayoutModule.Implementation.Base.Layout implements Model.Base.IAuthenticatedLayout {

        private allChannel: IChannel = {
            channelGuid: undefined,
            userItems: [],
            link: undefined,
            moreUserItems: true,
            rss: undefined,
            title: 'All'
        };

        private searchChannel: IChannel = {
            channelGuid: undefined,
            userItems: [],
            link: undefined,
            moreUserItems: true,
            rss: undefined,
            title: 'Search Results'
        };

        private addChannel(channel: IChannel): void {
            _.defaults(channel, {
                userItems: [],
                moreUserItems: true,
                title: ''
            });
            var foundChannel = _.find(this.viewModel.channels(), (chan) => {
                return channel.rss === chan.rss;
            });
            if (foundChannel) {
                _.extend(foundChannel, channel);
                return;
            }

            this.viewModel.channels.push(channel);
        }

        private addChannelSort(channel: IChannel): void {
            this.viewModel.channels.valueWillMutate();
            this.addChannel(channel);
            this.sortChannels();
            this.viewModel.channels.valueHasMutated();
            this.getMoreUserItems(channel);
        }

        private authenticationChangedCallback: Model.Base.IAuthenticationChangedCallback = (): void => {
            this.viewModel.userName(this.authentication.session.userName);
        };

        private clearStatusText(): void {
            this.viewModel.statusText(undefined);
        }

        private getMoreUserItems(channel: IChannel): void {
            if (channel === this.allChannel) {
                this.getMoreUserItemsAll();
                return;
            }

            var itemGuid = null;
            var sequence = Number.MAX_VALUE;
            _.forEach(channel.userItems, (userItem: IUserItem) => {
                if (userItem.sequence > sequence) {
                    return;
                }

                itemGuid = userItem.itemGuid;
                sequence = userItem.sequence;
            });

            var limit = 10;
            this.authentication.get('/api/channel/{channelGuid}?limit={limit?}&itemGuid={itemGuid?}', {
                channelGuid: channel.channelGuid,
                limit: limit,
                itemGuid: itemGuid
            }).done((userItems: IUserItem[]) => {
                _.forEach(userItems, (userItem) => {
                    _.defaults(userItem, {
                        descriptionPlain: jQuery('<div>').html(userItem.description).text(),
                        readObservable: ko.observable(userItem['read'])
                    });
                });

                channel.userItems = channel.userItems.concat(userItems);
                if (userItems.length < limit) {
                    channel.moreUserItems = false;
                }

                //Update if visible.
                if (this.viewModel.selectedChannel() == channel) {
                    this.setUserItems(channel.userItems);
                    this.viewModel.showMoreUserItems(channel.moreUserItems);
                } else if (this.viewModel.selectedChannel() == this.allChannel) {
                    if (userItems.length > 0) {
                        this.selectChannelAll();
                    }
                }
            });
        }

        private getMoreUserItemsAll(): void {
            _.forEach(this.viewModel.channels(), (channel: IChannel) => {
                this.getMoreUserItems(channel);
            });
        }

        private markAllAsRead(): void {
            _.forEach(this.viewModel.userItems(), (userItem: IUserItem) => {
                if (userItem.readObservable()) {
                    return;
                }

                userItem.readObservable(true);
                this.saveUserItem(userItem);
            });
        }

        private routerCatchAllCallback: Model.Base.IRouterCatchAllCallback = (): void => {
        };

        private saveUserItem(userItem: IUserItem): void {
            this.authentication.put('/api/userItem/{itemGuid}', {
                itemGuid: userItem.itemGuid,
                read: userItem.readObservable()
            });
        }

        private selectChannel(channel: IChannel): void {
            this.viewModel.selectedChannel(channel);
            this.viewModel.showDeleteChannel(true);
            this.viewModel.showMoreUserItems(channel.moreUserItems);
            this.setUserItems(channel.userItems);
            if (channel.userItems.length === 0) {
                this.getMoreUserItems(channel);
            }
        }

        private selectChannelAll(): void {
            this.viewModel.selectedChannel(this.allChannel);
            this.viewModel.showDeleteChannel(false);
            var userItems = [];
            var moreUserItems = false;
            _.forEach(this.viewModel.channels(), (channel: IChannel) => {
                userItems = userItems.concat(channel.userItems);
                moreUserItems = moreUserItems || channel.moreUserItems;
            });

            userItems.sort((a: IUserItem, b: IUserItem) => b.sequence - a.sequence);

            this.setUserItems(userItems);
            this.viewModel.showMoreUserItems(moreUserItems);

            if (userItems.length === 0) {
                this.getMoreUserItemsAll();
            }
        }

        private selectChannelSearch(): void {
            var search = this.viewModel.search();
            if (!search) {
                return;
            }

            jQuery('.nav.nav-pills.nav-stacked li').removeClass('active');

            this.viewModel.selectedChannel(this.searchChannel);
            this.viewModel.showDeleteChannel(false);
            this.viewModel.showMoreUserItems(false);

            var searchRegx = new RegExp(search.replace(/[-\/\\^$*+?.()|[\]{}]/g, '\\$&'), 'i');
            var userItems = _.filter(this.viewModel.userItems(), (userItem) => {
                var x = userItem.descriptionPlain.search(searchRegx);
                if (x != -1) {
                    return true;
                }

                x = userItem.title.search(searchRegx);
                if (x != -1) {
                    return true;
                }

                return false;
            });
            this.setUserItems(userItems);
        }

        private setUserItems(userItems: IUserItem[]): void {
            this.viewModel.userItems.valueWillMutate();
            this.viewModel.userItems(userItems);
            this.viewModel.userItems.sort((a: IUserItem, b: IUserItem) => {
                if (!a.readObservable() && b.readObservable()) {
                    return -1;
                }

                if (a.readObservable() && !b.readObservable()) {
                    return 1;
                }

                return b.sequence - a.sequence;
            });
            this.viewModel.userItems.valueHasMutated();
        }

        private sortChannels() {
            this.viewModel.channels.sort((a: IChannel, b: IChannel) => {
                var aName = a.title.toLocaleLowerCase();
                var bName = b.title.toLocaleLowerCase();
                return aName === bName ? 0 : aName < bName ? -1 : 1;
            });
        }

        constructor(
            public viewModel: IAuthenticatedLayoutViewModel,
            public authentication: Model.Base.IAuthentication,
            public router: Model.Base.IRouter) {
            super(viewModel, authentication, router);

            _.defaults(this.viewModel, {
                channelRss: ko.observable(),
                channelRssExists: ko.observable(),
                channels: ko.observableArray([]),
                search: ko.observable(),
                selectedChannel: ko.observable(this.allChannel),
                showDeleteChannel: ko.observable(false),
                showMoreUserItems: ko.observable(false),
                statusText: ko.observable(),
                userItems: ko.observableArray([]),

                addChannelClicked: (): void => {
                    this.clearStatusText();
                    this.authentication.post('/api/channel', { rss: this.viewModel.channelRss() })
                        .done((channel: IChannel) => {
                            this.addChannelSort(channel);
                            jQuery('#authenticatedLayout-addChannelModal').modal('hide');
                        })
                        .fail((jqXhr: JQueryXHR) => {
                            this.viewModel.statusText(jqXhr.responseJSON.message);
                        });
                },

                allChannelsClicked: (): void => {
                    this.selectChannelAll();
                },

                channelClicked: (channel: IChannel): void => {
                    this.selectChannel(channel);
                },

                clearStatusText: (): void => {
                    this.clearStatusText();
                },

                logoutClicked: (): void => {
                    this.logout();
                },

                markAllAsReadClicked: (): void => {
                    this.markAllAsRead();
                },

                markAsUnreadClicked: (userItem: IUserItem): void => {
                    if (userItem.readObservable()) {
                        userItem.readObservable(false);
                        this.saveUserItem(userItem);
                    }
                },

                moreClicked: (): void => {
                    this.getMoreUserItems(this.viewModel.selectedChannel());
                },

                removeSelectedChannelClicked: (): void => {
                    var channel = this.viewModel.selectedChannel();
                    jQuery('#authenticatedLayout-deleteChannelModal').modal('hide');
                    if ((channel === this.allChannel) || (channel === this.searchChannel)) {
                        return;
                    }

                    this.authentication.delete('/api/channel/{channelGuid}', {
                            channelGuid: channel.channelGuid
                        })
                        .done(() => {
                            this.selectChannel(this.allChannel);
                            this.viewModel.channels.remove(channel);
                        });
                },

                searchClicked: (): void => {
                    this.selectChannelSearch();
                },

                userItemClicked: (userItem: IUserItem, event: JQueryEventObject): void => {
                    var target = jQuery(event.target).parent().parent().children('.collapse');
                    if (target.hasClass('in')) {
                        target.collapse('hide');
                        return;
                    }

                    jQuery('#authenticatedLayout-userItems .collapse.in').collapse('hide');
                    target.collapse('show');
                    if (!userItem.readObservable()) {
                        userItem.readObservable(true);
                        this.saveUserItem(userItem);
                    }
                },
            });

            this.viewModel.userName(this.authentication.session.userName);
            this.authentication.changedJqCallback.add(this.authenticationChangedCallback);

            this.router.catchAllJqCallback.add(this.routerCatchAllCallback);

            this.router.navigated(this.router.currentUrlString());

            this.authentication.get("/api/channel")
                .done((channels: IChannel[]) => {
                    this.viewModel.channels.valueWillMutate();
                    _.forEach(channels, (channel: IChannel) => {
                        this.addChannel(channel);
                    });
                    this.sortChannels();
                    this.viewModel.channels.valueHasMutated();

                    this.selectChannelAll();
                });
        }

        render(el: JQuery, html?: HTMLElement): Model.Base.IView;
        render(el: JQuery, html?: string): Model.Base.IView;
        render(el: JQuery, html?: JQuery): Model.Base.IView;
        render(el: JQuery, html?: any): Model.Base.IView {
            jQuery('body').css('overflow-x', 'hidden').css('overflow-y', 'scroll').css('padding-top', '60px');
            super.render(el, require('text!implementation/layout/default/authenticatedLayout.html'));
            return this;
        }
    }

    export interface IAuthenticatedLayoutViewModel extends Model.Base.IAuthenticatedLayoutViewModel {
        channelRss: KnockoutObservable<string>;
        channelRssExists: KnockoutObservable<boolean>;
        channels: KnockoutObservableArray<IChannel>;
        userItems: KnockoutObservableArray<IUserItem>;
        search: KnockoutObservable<string>;
        selectedChannel: KnockoutObservable<IChannel>;
        showDeleteChannel: KnockoutObservable<boolean>;
        showMoreUserItems: KnockoutObservable<boolean>;
        statusText: KnockoutObservable<string>;
    }

    export interface IChannel {
        channelGuid: string;
        link: string;
        moreUserItems: boolean;
        rss: string;
        title: string;
        userItems: IUserItem[];
    }

    export interface IUserItem {
        description: string;
        descriptionPlain: string;
        itemGuid: string;
        link: string;
        readObservable: KnockoutObservable<boolean>;
        sequence: number;
        title: string;
    }

}