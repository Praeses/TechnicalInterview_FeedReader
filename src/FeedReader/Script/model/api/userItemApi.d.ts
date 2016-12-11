declare module Model.Api {

    interface IUserItemApi {
        putUserItem(itemGuid: string, read: boolean): IDtoPromise<void>;
    }

}