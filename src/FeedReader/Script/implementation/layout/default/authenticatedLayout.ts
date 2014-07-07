/// <reference path='../../../model/base/authenticatedLayout.d.ts' />
/// <amd-dependency path='text!implementation/layout/default/authenticatedLayout.html' />
import LayoutModule = require('implementation/base/layout');

export module Implementation.Layout.Default {

    export class AuthenticatedLayout extends LayoutModule.Implementation.Base.Layout implements Model.Base.IAuthenticatedLayout {

        private allChannel: IChannel = {
            channelGuid: undefined,
            items: [],
            link: undefined,
            moreItems: true,
            rss: undefined,
            title: 'All'
        };

        private searchChannel: IChannel = {
            channelGuid: undefined,
            items: [],
            link: undefined,
            moreItems: true,
            rss: undefined,
            title: 'Search Results'
        };

        private addChannel(channel: IChannel): void {
            this.viewModel.channels.valueWillMutate();
            this.addChannelNoMutate(channel);
            this.sortChannels();
            this.viewModel.channels.valueHasMutated();
        }

        private addChannelNoMutate(channel: IChannel): void {
            _.defaults(channel, {
                items: [],
                moreItems: true,
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

        private authenticationChangedCallback: Model.Base.IAuthenticationChangedCallback = (): void => {
            this.viewModel.userName(this.authentication.session.userName);
        };

        private clearStatusText(): void {
            this.viewModel.statusText(undefined);
        }

        private getMoreItems(channel: IChannel): void {
            if (channel === this.allChannel) {
                this.getMoreItemsAll();
                return;
            }

            var itemGuid = null;
            var lastItem = _.last(channel.items);
            if (!_.isUndefined(lastItem)) {
                itemGuid = lastItem.itemGuid;
            }

            var limit = 10;
            this.authentication.get('/api/channel/{channelGuid}?limit={limit?}&itemGuid={itemGuid?}', {
                channelGuid: channel.channelGuid,
                limit: limit,
                itemGuid: itemGuid
            }).done((items: IItem[]) => {
                _.forEach(items, (item) => {
                    _.defaults(item, { descriptionPlain: jQuery('<div>').html(item.description).text() });
                });

                channel.items = channel.items.concat(items);
                if (items.length < limit) {
                    channel.moreItems = false;
                }

                //Update if visible.
                if (this.viewModel.selectedChannel() == channel) {
                    this.viewModel.items(channel.items);
                    this.viewModel.showMoreItems(channel.moreItems);
                } else if (this.viewModel.selectedChannel() == this.allChannel) {
                    if (items.length > 0) {
                        this.selectChannelAll();
                    }
                }
            });
        }

        private getMoreItemsAll(): void {
            _.forEach(this.viewModel.channels(), (channel: IChannel) => {
                this.getMoreItems(channel);
            });
        }

        private routerCatchAllCallback: Model.Base.IRouterCatchAllCallback = (): void => {
        };

        private selectChannel(channel: IChannel): void {
            this.viewModel.selectedChannel(channel);
            this.viewModel.showDeleteChannel(true);
            this.viewModel.showMoreItems(channel.moreItems);
            this.viewModel.items(channel.items);
            if (channel.items.length === 0) {
                this.getMoreItems(channel);
            }
        }

        private selectChannelAll(): void {
            this.viewModel.selectedChannel(this.allChannel);
            this.viewModel.showDeleteChannel(false);
            var items = [];
            var moreItems = false;
            _.forEach(this.viewModel.channels(), (channel: IChannel) => {
                items = items.concat(channel.items);
                moreItems = moreItems || channel.moreItems;
            });

            items.sort((a: IItem, b: IItem) => b.sequence - a.sequence);

            this.viewModel.items(items);
            this.viewModel.showMoreItems(moreItems);

            if (items.length === 0) {
                this.getMoreItemsAll();
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
            this.viewModel.showMoreItems(false);

            var items = _.filter(this.viewModel.items(), (item) => {
                return item.descriptionPlain.indexOf(search) != -1;
            });
            this.viewModel.items(items);
        }

        private sortChannels() {
            this.viewModel.channels.sort((a: IChannel, b: IChannel) => {
                var aName = a.title.toLowerCase();
                var bName = b.title.toLowerCase();
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
                items: ko.observableArray([]),
                search: ko.observable(),
                selectedChannel: ko.observable(this.allChannel),
                showDeleteChannel: ko.observable(false),
                showMoreItems: ko.observable(false),
                statusText: ko.observable(),

                addChannelClicked: (): void => {
                    this.clearStatusText();
                    this.authentication.post('/api/channel', { rss: this.viewModel.channelRss() })
                        .done((channel: IChannel) => {
                            this.addChannel(channel);
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

                itemClicked: (item: IItem, event: JQueryEventObject): void => {
                    jQuery('#authenticatedLayout-items .collapse.in').collapse('hide');
                    jQuery(event.target).parent().parent().children('.collapse').collapse('show');
                },

                logoutClicked: (): void => {
                    this.logout();
                },

                moreClicked: (): void => {
                    this.getMoreItems(this.viewModel.selectedChannel());
                },

                removeSelectedChannelClicked: (): void => {
                    if (this.viewModel.selectedChannel() === this.allChannel) {
                        return;
                    }

                    this.authentication.delete('/api/channel/{channelGuid}', {
                            channelGuid: this.viewModel.selectedChannel().channelGuid
                        })
                        .done(() => {
                            var channel = this.viewModel.selectedChannel();
                            this.viewModel.selectedChannel(this.allChannel);
                            this.viewModel.channels.remove(channel);
                        });
                },

                searchClicked: (): void => {
                    this.selectChannelSearch();
                }
            });

            this.viewModel.userName(this.authentication.session.userName);
            this.authentication.changedJqCallback.add(this.authenticationChangedCallback);

            this.router.catchAllJqCallback.add(this.routerCatchAllCallback);

            this.router.navigated(this.router.currentUrlString());

            this.authentication.get("/api/channel")
                .done((channels: IChannel[]) => {
                    this.viewModel.channels.valueWillMutate();
                    _.forEach(channels, (channel: IChannel) => {
                        this.addChannelNoMutate(channel);
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
        items: KnockoutObservableArray<IItem>;
        search: KnockoutObservable<string>;
        selectedChannel: KnockoutObservable<IChannel>;
        showDeleteChannel: KnockoutObservable<boolean>;
        showMoreItems: KnockoutObservable<boolean>;
        statusText: KnockoutObservable<string>;
    }

    export interface IChannel {
        channelGuid: string;
        items: IItem[];
        link: string;
        moreItems: boolean;
        rss: string;
        title: string;
    }

    export interface IItem {
        description: string;
        descriptionPlain: string;
        itemGuid: string;
        link: string;
        sequence: number;
        title: string;
    }

}