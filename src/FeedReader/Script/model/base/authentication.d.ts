declare module Model.Base {

    interface IAuthentication {
        changedJqCallback: JQueryCallback;
        session: IAuthenticationSession;

        login(userName: string, password: string, rememberMe: boolean): JQueryPromise<IAuthenticationToken>;
        logout(): JQueryPromise<void>;
        registerUser(userName: string, password: string, rememberMe: boolean): JQueryPromise<IAuthenticationToken>;
        reset(): IAuthentication;
        unregisterUser(userName: string, password: string): JQueryPromise<void>;
        userNameExists(userName: string): JQueryPromise<void>;

        delete(urlString: string, data?: Object): JQueryPromise<void>;
        get(urlString: string, data?: Object): JQueryPromise<any>;
        head(urlString: string, data?: Object): JQueryPromise<void>;
        post(urlString: string, data: Object): JQueryPromise<any>;
        put(urlString: string, data: Object): JQueryPromise<any>;
    }

    interface IAuthenticationChangedCallback {
        (): void;
    }

    interface IAuthenticationSession {
        authenticated: boolean;
        authenticatedWithToken: boolean;
        rememberMe: boolean;
        tokenGuid: string;
        userName: string;
    }

    interface IAuthenticationToken {
        created: Date;
        tokenGuid: string;
        tokenName: string;
        tokenType: string;
    }

}