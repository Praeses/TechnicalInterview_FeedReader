declare module Model.Class {

    interface IChannelClass extends Model.Api.IChannelApiChannel {
        moreUserItems: boolean;
        userItems: IUserItemClass[];

        getMoreUserItems(): Model.Api.IDtoPromise<void>;
        refresh(): Model.Api.IDtoPromise<void>;
        sort(): void;
    }
}