declare module Model.Api {

    interface IUserItemApi {
        putUserItem(itemGuid: string, read: boolean): JQueryPromise<void>;
    }

}