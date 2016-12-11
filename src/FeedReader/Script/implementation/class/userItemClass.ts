export module Implementation.Class {

    export class UserItemClass implements Model.Class.IUserItemClass {
        constructor(
            private userItemApi: Model.Api.IUserItemApi,
            userItem: Model.Api.IChannelApiUserItem) {
            _.extend(this, userItem);
            this.descriptionPlain = jQuery('<div>').html(userItem.description).text();
        }

        description: string = undefined;
        descriptionPlain: string = undefined;
        itemGuid: string = undefined;
        link: string = undefined;
        read: boolean = undefined;
        sequence: number = undefined;
        title: string = undefined;

        save(): void {
            this.userItemApi.putUserItem(this.itemGuid, this.read);
        }
    }
}