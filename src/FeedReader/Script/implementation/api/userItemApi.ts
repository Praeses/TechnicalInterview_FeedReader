/// <reference path='../../model/api/authenticationApi.d.ts' />
import UrlModule = require('implementation/base/url');

export module Implementation.Api {
    export class UserItemApi implements Model.Api.IUserItemApi {

        constructor(private dto: Model.Api.IDto) {}

        putUserItem(itemGuid: string, read: boolean): JQueryPromise<void> {
            return this.dto.send<IUserItemApiPutUserItem, void>(
                    new UserItemApiPutUserItemRequest(itemGuid, read))
                .then(() => {});
        }

    }

    // UserItem Dtos
    class UserItemApiPutUserItemRequest implements Model.Api.IDtoRequest<IUserItemApiPutUserItem, void> {
        constructor(private itemGuid: string, private read: boolean) {
            this.data = {
                itemGuid: itemGuid,
                read: read
            };
            this.method = "PUT";
            this.url = new UrlModule.Implementation.Base.Url('/api/userItem/{itemGuid}');
        }

        data: IUserItemApiPutUserItem;
        method: string;
        url: Model.Base.IUrl;
    }

    interface IUserItemApiPutUserItem {
        itemGuid: string;
        read: boolean;
    }

}