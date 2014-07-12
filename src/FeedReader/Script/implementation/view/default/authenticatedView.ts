/// <reference path='../../../model/base/view.d.ts' />
/// <amd-dependency path='text!implementation/view/default/authenticatedView.html' />
import ViewModule = require('implementation/base/view');

export module Implementation.View.Default {

    export class AuthenticatedView extends ViewModule.Implementation.Base.View {

        private allChannel: IChannel = {
            activeObservable: ko.observable(true),
            channelGuid: undefined,
            link: undefined,
            moreUserItems: true,
            rss: undefined,
            title: 'All',
            showDeleteChannel: false,
            userItems: [],

            getMoreUserItems: (): Model.Api.IDtoPromise<void> => {
                var deferreds: JQueryDeferred<void>[] = [];
                _.forEach(this.viewModel.channels(), (subChannel: IChannel) => {
                    deferreds.push(<any>this.getMoreUserItems(subChannel)
                        .done(() => {
                            if (this.viewModel.selectedChannel() == this.allChannel) {
                                this.setUserItems(this.allChannel);
                            }
                        }));
                });

                return jQuery.when.apply(this, deferreds);
            },

            refresh: (): Model.Api.IDtoPromise<void> => {
                var deferreds: JQueryDeferred<void>[] = [];
                _.forEach(this.viewModel.channels(), (subChannel: IChannel) => {
                    deferreds.push(<any>this.refresh(subChannel)
                        .done(() => {
                            if (this.viewModel.selectedChannel() == this.allChannel) {
                                this.setUserItems(this.allChannel);
                            }
                        }));
                });

                return jQuery.when.apply(this, deferreds);
            },

            sort: (): void => {
                this.allChannel.moreUserItems = false;
                this.allChannel.userItems = [];
                _.forEach(this.viewModel.channels(), (channel: IChannel) => {
                    this.allChannel.userItems = this.allChannel.userItems.concat(channel.userItems);
                    this.allChannel.moreUserItems = this.allChannel.moreUserItems || channel.moreUserItems;
                });

                this.allChannel.userItems.sort((a, b) => {
                    if (!a.read && b.read) {
                        return -1;
                    }

                    if (a.read && !b.read) {
                        return 1;
                    }

                    return b.sequence - a.sequence;
                });
            }
        };

        private searchChannel: IChannel = {
            activeObservable: ko.observable(false),
            channelGuid: undefined,
            link: undefined,
            moreUserItems: false,
            rss: undefined,
            title: 'Search Results',
            showDeleteChannel: false,
            userItems: [],

            getMoreUserItems: (): Model.Api.IDtoPromise<void> => {
                return jQuery.Deferred().resolve().promise();
            },

            refresh: (): Model.Api.IDtoPromise<void> => {
                this.searchChannel.sort();
                return jQuery.Deferred().resolve().promise();
            },

            sort: (): void => {
                var search = this.viewModel.search();
                if (!search) {
                    this.searchChannel.userItems = this.viewModel.userItems();
                    return;
                }

                var searchRegx = new RegExp(search.replace(/[-\/\\^$*+?.()|[\]{}]/g, '\\$&'), 'i');
                this.searchChannel.userItems = _.filter(this.viewModel.userItems(), (userItem: IUserItem) => {
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
            }
        };

        private clearStatusText(): void {
            this.viewModel.statusText(undefined);
        }

        private getMoreUserItems(channel: IChannel): Model.Api.IDtoPromise<void> {
            return channel.getMoreUserItems()
                .done(() => {
                    if (this.viewModel.selectedChannel() == channel) {
                        this.setUserItems(channel);
                    }
                });
        }

        private refresh(channel: IChannel): Model.Api.IDtoPromise<void> {
            return channel.refresh()
                .done(() => {
                    if (this.viewModel.selectedChannel() == channel) {
                        this.setUserItems(channel);
                    }
                });
        }

        private selectChannel(channel: IChannel): void {
            this.allChannel.activeObservable(false);
            this.searchChannel.activeObservable(false);
            _.forEach(this.viewModel.channels(), (chan) => chan.activeObservable(false));
            channel.activeObservable(true);
            this.viewModel.selectedChannel(channel);
            this.viewModel.showDeleteChannel(channel.showDeleteChannel);
            this.viewModel.showMoreUserItems(channel.moreUserItems);
            this.setUserItems(channel);
            if (channel.userItems.length === 0) {
                this.viewModel.gettingMore(true);
                this.getMoreUserItems(channel)
                    .always(() => this.viewModel.gettingMore(false));
            }
        }

        private setChannels() {
            this.rssClass.sort();
            _.forEach(this.rssClass.channels, (channel) => {
                var chan = <IChannel>channel;
                chan.showDeleteChannel = true;
                if (_.isUndefined(chan.activeObservable)) {
                    chan.activeObservable = ko.observable(false);
                }
            });

            this.viewModel.channels(<IChannel[]>this.rssClass.channels);
        }

        private setUserItems(channel: IChannel) {
            jQuery('#authenticatedLayout-userItems .collapse.in').collapse('hide');
            channel.sort();
            _.forEach(channel.userItems, (userItem) => {
                var item = <IUserItem>userItem;
                if (_.isUndefined(item.readObservable)) {
                    item.readObservable = ko.observable(item.read);
                }
            });
            this.viewModel.showMoreUserItems(channel.moreUserItems);
            this.viewModel.userItems(<IUserItem[]>channel.userItems);
        }

        constructor(
            public viewModel: IAuthenticatedViewModel,
            private authenticationApi: Model.Api.IAuthenticationApi,
            private channelApi: Model.Api.IChannelApi,
            private userItemApi: Model.Api.IUserItemApi,
            private rssClass: Model.Class.IRssClass) {
            super(viewModel);

            _.defaults(this.viewModel, {
                activeObservable: this.allChannel.activeObservable,
                addingChannel: ko.observable(false),
                channelRss: ko.observable(),
                channelRssExists: ko.observable(),
                channels: ko.observableArray([]),
                gettingMore: ko.observable(false),
                refreshing: ko.observable(false),
                search: ko.observable(),
                selectedChannel: ko.observable(this.allChannel),
                showDeleteChannel: ko.observable(false),
                showMoreUserItems: ko.observable(false),
                statusText: ko.observable(),
                userItems: ko.observableArray([]),
                userName: ko.observable(),

                addChannelClicked: (): void => {
                    if (this.viewModel.addingChannel()) {
                        return;
                    }

                    this.clearStatusText();
                    this.viewModel.addingChannel(true);
                    this.rssClass.addChannel(this.viewModel.channelRss())
                        .always(() => this.viewModel.addingChannel(false))
                        .done((channel: IChannel) => {
                            jQuery('#authenticatedLayout-addChannelModal').modal('hide');
                            this.setChannels();
                            this.selectChannel(channel);
                        })
                        .fail((error: Model.Api.IDtoError) => {
                            this.viewModel.statusText(error.message);
                        });
                },

                allChannelsClicked: (): void => {
                    this.selectChannel(this.allChannel);
                },

                channelClicked: (channel: IChannel): void => {
                    this.selectChannel(channel);
                },

                clearStatusText: (): void => {
                    this.clearStatusText();
                },

                logoutClicked: (): void => {
                    this.authenticationApi.logout();
                },

                markAllAsReadClicked: (): void => {
                    _.forEach(this.viewModel.userItems(), (userItem: IUserItem) => {
                        if (userItem.readObservable()) {
                            return;
                        }

                        userItem.read = true;
                        userItem.readObservable(userItem.read);
                        userItem.save();
                    });
                },

                markAllAsUnreadClicked: (): void => {
                    _.forEach(this.viewModel.userItems(), (userItem: IUserItem) => {
                        if (!userItem.readObservable()) {
                            return;
                        }

                        userItem.read = false;
                        userItem.readObservable(userItem.read);
                        userItem.save();
                    });
                },

                markAsUnreadClicked: (userItem: IUserItem): void => {
                    userItem.read = false;
                    userItem.readObservable(userItem.read);
                    userItem.save();
                },

                moreClicked: (): void => {
                    if (this.viewModel.gettingMore()) {
                        return;
                    }

                    this.viewModel.gettingMore(true);
                    this.getMoreUserItems(this.viewModel.selectedChannel())
                        .always(() => this.viewModel.gettingMore(false));
                },

                refreshClicked: (): void => {
                    if (this.viewModel.refreshing()) {
                        return;
                    }

                    this.viewModel.refreshing(true);
                    this.refresh(this.viewModel.selectedChannel())
                        .always(() => {
                            setTimeout(() => this.viewModel.refreshing(false), 500);
                        });
                },

                removeSelectedChannelClicked: (): void => {
                    var channel = this.viewModel.selectedChannel();
                    jQuery('#authenticatedLayout-deleteChannelModal').modal('hide');
                    if ((channel === this.allChannel) || (channel === this.searchChannel)) {
                        return;
                    }

                    this.rssClass.removeChannel(channel);
                    this.setChannels();
                    this.selectChannel(this.allChannel);
                },

                searchClicked: (): void => {
                    this.selectChannel(this.searchChannel);
                },

                userItemClicked: (userItem: IUserItem, event: JQueryEventObject): void => {
                    var target = jQuery(event.target).parent().parent().children('.collapse');
                    if (target.hasClass('in')) {
                        target.collapse('hide');
                        return;
                    }

                    jQuery('#authenticatedLayout-userItems .collapse.in').collapse('hide');
                    target.collapse('show');
                    if (!userItem.read) {
                        userItem.read = true;
                        userItem.readObservable(userItem.read);
                        userItem.save();
                    }
                },
            });

            this.viewModel.userName(this.authenticationApi.session.userName);
            this.rssClass.refresh()
                .done(() => {
                    this.setChannels();
                    this.selectChannel(this.allChannel);
                });
        }

        render(el: JQuery, html?: HTMLElement): Model.Base.IView;
        render(el: JQuery, html?: string): Model.Base.IView;
        render(el: JQuery, html?: JQuery): Model.Base.IView;
        render(el: JQuery, html?: any): Model.Base.IView {
            jQuery('body').css('overflow-x', 'hidden').css('overflow-y', 'scroll').css('padding-top', '60px');
            super.render(el, require('text!implementation/view/default/authenticatedView.html'));
            return this;
        }
    }

    export interface IAuthenticatedViewModel extends Model.Base.IViewModel {
        addingChannel: KnockoutObservable<boolean>;
        channelRss: KnockoutObservable<string>;
        channelRssExists: KnockoutObservable<boolean>;
        channels: KnockoutObservableArray<IChannel>;
        gettingMore: KnockoutObservable<boolean>;
        refreshing: KnockoutObservable<boolean>;
        search: KnockoutObservable<string>;
        selectedChannel: KnockoutObservable<IChannel>;
        showDeleteChannel: KnockoutObservable<boolean>;
        showMoreUserItems: KnockoutObservable<boolean>;
        statusText: KnockoutObservable<string>;
        userItems: KnockoutObservableArray<IUserItem>;
        userName: KnockoutObservable<string>;
    }

    export interface IChannel extends Model.Class.IChannelClass {
        activeObservable: KnockoutObservable<boolean>;
        showDeleteChannel: boolean;
    }

    export interface IUserItem extends Model.Class.IUserItemClass {
        readObservable: KnockoutObservable<boolean>;
    }

}