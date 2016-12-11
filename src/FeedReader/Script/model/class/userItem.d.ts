declare module Model.Class {

    interface IUserItemClass extends Model.Api.IChannelApiUserItem {
        descriptionPlain: string;

        save(): void;
    }
}