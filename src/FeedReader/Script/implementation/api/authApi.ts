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

        login(userName: string, password: string, rememberMe: boolean): JQueryPromise<void> {
            return this.send<IAuthenticationApiLogin, IAuthenticationApiToken>(
                    new AuthenticationLoginRequest(userName, password, this.getTokenName()))
                .then((response) => {
                    this.session = {
                        authenticated: true,
                        authenticatedWithToken: false,
                        rememberMe: rememberMe,
                        tokenGuid: response.data.tokenGuid,
                        userName: userName
                    };
                    this.saveSession();
                });
        }

        logout(): JQueryPromise<void> {
            return this.send<void, void>(
                    new AuthenticationLogoutRequest())
                .then(() => {
                    this.logoutSession();
                });
        }

        register(userName: string, password: string, rememberMe: boolean): JQueryPromise<void> {
            return this.send<IRegistrationApiRegisterUserName, IAuthenticationApiToken>(
                    new RegistrationRegisterUserNameRequest(userName, password, this.getTokenName()))
                .then((response) => {
                    this.session = {
                        authenticated: true,
                        authenticatedWithToken: false,
                        rememberMe: rememberMe,
                        tokenGuid: response.data.tokenGuid,
                        userName: userName
                    };
                    this.saveSession();
                });
        }

        send<TRequest, TResponse>(request: Model.Api.IDtoRequest<TRequest, TResponse>): JQueryPromise<Model.Api.IDtoResponse<TResponse>> {
            var data = _.extend({}, request.data);
            var url = request.url.replaceVariables(data, true).removeVariables();

            var method = request.method.toUpperCase();
            if (((method === 'DELETE') || (method === 'GET') || (method === 'HEAD')) &&
                _.size(data)) {
                _.forEach(data, (value: string, key) => {
                    url.query[key] = value;
                });
                data = undefined;
            }

            var settings: JQueryAjaxSettings = {
                data: data,
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
                (json: TResponse) => {
                    var jqXhr: JQueryXHR = arguments[2];
                    return {
                        data: json,
                        status: jqXhr.status,
                        statusText: jqXhr.statusText
                    };
                },
                () => {
                    var jqXhr: JQueryXHR = arguments[0];
                    return {
                        data: jqXhr['responseJSON'],
                        status: jqXhr.status,
                        statusText: jqXhr.statusText
                    };
                });
        }

        unregister(userName: string, password: string): JQueryPromise<void> {
            return this.send<void, void>(
                    new RegistrationUnregisterUserRequest())
                .then(() => {
                    this.logoutSession();
                });
        }

        userNameExists(userName: string): JQueryPromise<void> {
            return this.send<IRegistrationApiUserNameExists, void>(
                    new RegistrationUserNameExistsRequest(userName))
                .then(() => {});
        }
    }

    // Authentication Dtos
    class AuthenticationLoginRequest implements Model.Api.IDtoRequest<IAuthenticationApiLogin, IAuthenticationApiToken> {
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

    interface IAuthenticationApiToken {
        created: Date;
        tokenGuid: string;
        tokenName: string;
        tokenType: string;
    }

    // Registration Dtos
    class RegistrationRegisterUserNameRequest implements Model.Api.IDtoRequest<IRegistrationApiRegisterUserName, IAuthenticationApiToken> {
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