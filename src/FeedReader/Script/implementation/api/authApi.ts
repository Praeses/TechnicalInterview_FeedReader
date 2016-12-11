/// <reference path='../../model/api/authenticationApi.d.ts' />
import UrlModule = require('implementation/base/url');

export module Implementation.Api {
    export class AuthApi implements Model.Api.IAuthenticationApi, Model.Api.IRegistrationApi, Model.Api.IDto {
        private view: Model.Base.IView;

        private getStorageKey(): string {
            return 'authentication';
        }

        private getTokenName(): string {
            var match = navigator.userAgent.match(/(chrome|firefox|msie|opera|safari|trident)/i);
            if (match && match[0]) {
                return match[0].toLowerCase();
            }

            return 'browser';
        }

        private loadSession(): void {
            var sessionJson = this.sessionStorage.getItem(this.getStorageKey());
            if (sessionJson) {
                this.session = jQuery.parseJSON(sessionJson);
                if (this.session.tokenGuid && this.session.userName) {
                    this.changedJqCallback.fire();
                    return;
                }
            }

            sessionJson = this.localStorage.getItem(this.getStorageKey());
            if (sessionJson) {
                this.session = jQuery.parseJSON(sessionJson);
                if (this.session.userName) {
                    this.session.authenticatedWithToken = true;
                    this.saveSession();
                    return;
                }
            }

            this.session = {
                authenticated: false,
                authenticatedWithToken: false,
                rememberMe: true,
                tokenGuid: null,
                userName: null
            };

            this.saveSession();
        }

        private logoutSession(): void {
            this.session.authenticated = false;
            this.session.authenticatedWithToken = false;
            this.session.tokenGuid = null;
            this.saveSession();
        }

        private saveSession(): void {
            var sessionJson = JSON.stringify(this.session);
            if (this.session.rememberMe) {
                this.localStorage.setItem(this.getStorageKey(), sessionJson);
            } else {
                this.localStorage.removeItem(this.getStorageKey());
            }

            if (this.session.tokenGuid) {
                this.sessionStorage.setItem(this.getStorageKey(), sessionJson);
            } else {
                this.sessionStorage.removeItem(this.getStorageKey());
            }

            this.changedJqCallback.fire();
        }

        constructor(
            private localStorage: Storage,
            private sessionStorage: Storage) {
            this.loadSession();
            if (this.session.tokenGuid === null) {
                return;
            }

            this.send(new AuthenticationTokenRequest())
                .fail(() => this.logoutSession());
        }

        changedJqCallback: JQueryCallback = jQuery.Callbacks('memory unique');
        session: Model.Api.IAuthenticationApiSession;

        login(userName: string, password: string, rememberMe: boolean): Model.Api.IDtoPromise<Model.Api.IAuthenticationApiToken> {
            return this.send<IAuthenticationApiLogin, Model.Api.IAuthenticationApiToken>(
                    new AuthenticationLoginRequest(userName, password, this.getTokenName()))
                .done((response) => {
                    this.session = {
                        authenticated: true,
                        authenticatedWithToken: false,
                        rememberMe: rememberMe,
                        tokenGuid: response.tokenGuid,
                        userName: userName
                    };
                    this.saveSession();
                });
        }

        logout(): Model.Api.IDtoPromise<void> {
            return this.send<void, void>(
                    new AuthenticationLogoutRequest())
                .done(() => {
                    this.logoutSession();
                });
        }

        register(userName: string, password: string, rememberMe: boolean): Model.Api.IDtoPromise<Model.Api.IAuthenticationApiToken> {
            return this.send<IRegistrationApiRegisterUserName, Model.Api.IAuthenticationApiToken>(
                    new RegistrationRegisterUserNameRequest(userName, password, this.getTokenName()))
                .done((response) => {
                    this.session = {
                        authenticated: true,
                        authenticatedWithToken: false,
                        rememberMe: rememberMe,
                        tokenGuid: response.tokenGuid,
                        userName: userName
                    };
                    this.saveSession();
                });
        }

        send<TRequest, TResponse>(request: Model.Api.IDtoRequest<TRequest, TResponse>): Model.Api.IDtoPromise<TResponse> {
            var dataCopy = _.extend({}, request.data);
            var url = request.url.replaceVariables(dataCopy, true).removeVariables();

            var method = request.method.toUpperCase();
            if (((method === 'DELETE') || (method === 'GET') || (method === 'HEAD')) &&
                _.size(dataCopy)) {
                _.forEach(dataCopy, (value: string, key) => {
                    url.query[key] = value;
                });
                dataCopy = undefined;
            }

            var settings: JQueryAjaxSettings = {
                data: dataCopy,
                dataType: 'json',
                headers: {
                    Accept: 'application/json'
                },
                type: method
            };
            if (this.session.tokenGuid) {
                settings.headers['X-Token'] = this.session.tokenGuid;
            }

            return jQuery.ajax(url.toString(), settings)
                .then(
                (data: TResponse, textStatus, jqXhr: JQueryXHR) => {
                    return jQuery.Deferred().resolve(data, jqXhr).promise();
                },
                (jqXhr: JQueryXHR) => {
                    // Check for unauthorized.
                    if (jqXhr.status === 401) {
                        this.logoutSession();
                        return jqXhr;
                    }

                    var error = _.extend({
                        dumpObjects: undefined,
                        exceptionType: undefined,
                        innerException: undefined,
                        message: undefined,
                        status: jqXhr.status,
                        statusText: jqXhr.statusText
                    }, jqXhr.responseJSON);
                    return jQuery.Deferred().reject(error, jqXhr).promise();
                });
        }

        unregister(userName: string, password: string): Model.Api.IDtoPromise<void> {
            return this.send<void, void>(
                    new RegistrationUnregisterUserRequest())
                .always(() => {
                    this.logoutSession();
                });
        }

        userNameExists(userName: string): Model.Api.IDtoPromise<void> {
            return this.send<IRegistrationApiUserNameExists, void>(
                new RegistrationUserNameExistsRequest(userName));
        }
    }

    // Authentication Dtos
    class AuthenticationLoginRequest implements Model.Api.IDtoRequest<IAuthenticationApiLogin, Model.Api.IAuthenticationApiToken> {
        constructor(private userName: string, private password: string, private tokenName: string) {
            this.data = {
                password: password,
                tokenName: tokenName,
                userName: userName
            };
            this.method = "POST";
            this.url = new UrlModule.Implementation.Base.Url('/api/authentication');
        }

        data: IAuthenticationApiLogin;
        method: string;
        url: Model.Base.IUrl;
    }

    class AuthenticationLogoutRequest implements Model.Api.IDtoRequest<void, void> {
        constructor() {
            this.method = "DELETE";
            this.url = new UrlModule.Implementation.Base.Url('/api/authentication');
        }

        data: void;
        method: string;
        url: Model.Base.IUrl;
    }

    class AuthenticationTokenRequest implements Model.Api.IDtoRequest<void, void> {
        constructor() {
            this.method = "HEAD";
            this.url = new UrlModule.Implementation.Base.Url('/api/authentication');
        }

        data: void;
        method: string;
        url: Model.Base.IUrl;
    }

    interface IAuthenticationApiLogin {
        password: string;
        tokenName: string;
        userName: string;
    }

    // Registration Dtos
    class RegistrationRegisterUserNameRequest implements Model.Api.IDtoRequest<IRegistrationApiRegisterUserName, Model.Api.IAuthenticationApiToken> {
        constructor(private userName: string, private password: string, private tokenName: string) {
            this.data = {
                password: password,
                tokenName: tokenName,
                userName: userName
            };
            this.method = "POST";
            this.url = new UrlModule.Implementation.Base.Url('/api/registration');
        }

        data: IAuthenticationApiLogin;
        method: string;
        url: Model.Base.IUrl;
    }

    class RegistrationUnregisterUserRequest implements Model.Api.IDtoRequest<void, void> {
        constructor() {
            this.method = "DELETE";
            this.url = new UrlModule.Implementation.Base.Url('/api/registration');
        }

        data: void;
        method: string;
        url: Model.Base.IUrl;
    }

    class RegistrationUserNameExistsRequest implements Model.Api.IDtoRequest<IRegistrationApiUserNameExists, void> {
        constructor(private userName: string) {
            this.data = {
                userName: userName
            };
            this.method = "HEAD";
            this.url = new UrlModule.Implementation.Base.Url('/api/registration');
        }

        data: IRegistrationApiUserNameExists;
        method: string;
        url: Model.Base.IUrl;
    }

    interface IRegistrationApiRegisterUserName {
        password: string;
        tokenName: string;
        userName: string;
    }

    interface IRegistrationApiUserNameExists {
        userName: string;
    }
}