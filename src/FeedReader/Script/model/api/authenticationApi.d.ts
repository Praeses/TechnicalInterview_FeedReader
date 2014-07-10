declare module Model.Api {

    interface IAuthenticationApi {
        changedJqCallback: JQueryCallback;
        session: IAuthenticationApiSession;

        login(userName: string, password: string, rememberMe: boolean): IDtoPromise<IAuthenticationApiToken>;
        logout(): IDtoPromise<void>;
    }

    interface IAuthenticationApiChangedCallback {
        (): void;
    }

    interface IAuthenticationApiToken {
        created: Date;
        tokenGuid: string;
        tokenName: string;
        tokenType: string;
    }

    interface IAuthenticationApiSession {
        authenticated: boolean;
        authenticatedWithToken: boolean;
        rememberMe: boolean;
        tokenGuid: string;
        userName: string;
    }
}