declare module Model.Api {

    interface IRegistrationApi {
        register(userName: string, password: string, rememberMe: boolean): IDtoPromise<IAuthenticationApiToken>;
        unregister(userName: string, password: string): IDtoPromise<void>;
        userNameExists(userName: string): IDtoPromise<void>;
    }
}