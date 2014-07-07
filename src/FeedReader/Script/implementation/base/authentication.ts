/// <reference path='../../model/base/authentication.d.ts' />
import UrlModule = require('implementation/base/url');

export module Implementation.Base {
    export class Authentication implements Model.Base.IAuthentication {

        private ajaxSettings(type: string, data?: Object): JQueryAjaxSettings {
            var settings: JQueryAjaxSettings = {
                data: data,
                dataType: 'json',
                headers: {
                    Accept: 'application/json'
                },
                type: type
            };
            if (this.session.tokenGuid) {
                settings.headers['X-Token'] = this.session.tokenGuid;
            }

            return settings;
        }

        private createUrl(urlString: string, data?: Object): string {
            var url = new UrlModule.Implementation.Base.Url(urlString);
            var replacedUrl = url.replaceVariables(data, true);
            replacedUrl.removeVariables();
            return replacedUrl.toString();
        }

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

        constructor(private localStorage: Storage, private sessionStorage: Storage) {
            this.reset();
            this.loadSession();
            if (this.session.tokenGuid === null) {
                return;
            }

            this.head('/api/authentication')
                .fail(() => {
                    this.logoutSession();
                });
        }

        changedJqCallback: JQueryCallback;
        session: Model.Base.IAuthenticationSession;

        login(userName: string, password: string, rememberMe: boolean): JQueryPromise<Model.Base.IAuthenticationToken> {
            return this.post("/api/authentication", {
                    userName: userName,
                    password: password,
                    tokenName: this.getTokenName()
                })
                .done((token: Model.Base.IAuthenticationToken) => {
                    this.session = {
                        authenticated: true,
                        authenticatedWithToken: false,
                        rememberMe: rememberMe,
                        tokenGuid: token.tokenGuid,
                        userName: userName
                    };
                    this.saveSession();
                });
        }

        logout(): JQueryPromise<void> {
            return this.delete("/api/authentication")
                .done(() => this.logoutSession());
        }

        registerUser(userName: string, password: string, rememberMe: boolean): JQueryPromise<Model.Base.IAuthenticationToken> {
            return this.post("/api/registration", {
                    userName: userName,
                    password: password,
                    tokenName: this.getTokenName()
                })
                .done((token: Model.Base.IAuthenticationToken) => {
                    this.session = {
                        authenticated: true,
                        authenticatedWithToken: false,
                        rememberMe: rememberMe,
                        tokenGuid: token.tokenGuid,
                        userName: userName
                    };
                    this.saveSession();
                });
        }

        reset(): Model.Base.IAuthentication {
            this.changedJqCallback = jQuery.Callbacks('memory unique');
            return this;
        }

        unregisterUser(userName: string, password: string): JQueryPromise<void> {
            return this.delete("/api/registration")
                .done(() => this.logoutSession());
        }

        userNameExists(userName: string): JQueryPromise<void> {
            return this.head("/api/registration?username={userName}", {
                userName: userName
            });
        }

        delete(urlString: string, data?: Object): JQueryPromise<void> {
            var url = this.createUrl(urlString, data);
            if (_.size(data)) {
                throw 'Can not pass data in body';
            }
            return jQuery.ajax(url, this.ajaxSettings("DELETE"));
        }

        get(urlString: string, data?: Object): JQueryPromise<any> {
            var url = this.createUrl(urlString, data);
            if (_.size(data)) {
                throw 'Can not pass data in body';
            }
            return jQuery.ajax(url, this.ajaxSettings("GET"));
        }

        head(urlString: string, data?: Object): JQueryPromise<void> {
            var url = this.createUrl(urlString, data);
            if (_.size(data)) {
                throw 'Can not pass data in body';
            }
            return jQuery.ajax(url, this.ajaxSettings("HEAD"));
        }

        post(urlString: string, data: Object): JQueryPromise<any> {
            var url = this.createUrl(urlString, data);
            return jQuery.ajax(url, this.ajaxSettings("POST", data));
        }

        put(urlString: string, data: Object): JQueryPromise<any> {
            var url = this.createUrl(urlString, data);
            return jQuery.ajax(url, this.ajaxSettings("PUT", data));
        }

    }
}