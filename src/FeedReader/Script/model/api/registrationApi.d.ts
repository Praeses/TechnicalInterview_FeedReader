declare module Model.Api {

    interface IRegistrationApi {
        register(userName: string, password: string, rememberMe: boolean): JQueryPromise<void>;
        unregister(userName: string, password: string): JQueryPromise<void>;
        userNameExists(userName: string): JQueryPromise<void>;
    }
}