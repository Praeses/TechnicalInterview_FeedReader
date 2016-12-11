define(["require", "exports", 'implementation/base/url'], function(require, exports, UrlModule) {
    (function (Implementation) {
        (function (Base) {
            var Authentication = (function () {
                function Authentication(localStorage, sessionStorage) {
                    var _this = this;
                    this.localStorage = localStorage;
                    this.sessionStorage = sessionStorage;
                    this.changedJqCallback = jQuery.Callbacks('memory unique');
                    this.loadSession();
                    if (this.session.tokenGuid === null) {
                        return;
                    }

                    this.head('/api/authentication').fail(function () {
                        _this.logoutSession();
                    });
                }
                Authentication.prototype.ajaxSettings = function (type, data) {
                    var settings = {
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
                };

                Authentication.prototype.createUrl = function (urlString, data) {
                    var url = new UrlModule.Implementation.Base.Url(urlString);
                    var replacedUrl = url.replaceVariables(data, true);
                    replacedUrl.removeVariables();
                    return replacedUrl.toString();
                };

                Authentication.prototype.getStorageKey = function () {
                    return 'authentication';
                };

                Authentication.prototype.getTokenName = function () {
                    var match = navigator.userAgent.match(/(chrome|firefox|msie|opera|safari|trident)/i);
                    if (match && match[0]) {
                        return match[0].toLowerCase();
                    }

                    return 'browser';
                };

                Authentication.prototype.loadSession = function () {
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
                };

                Authentication.prototype.logoutSession = function () {
                    this.session.authenticated = false;
                    this.session.authenticatedWithToken = false;
                    this.session.tokenGuid = null;
                    this.saveSession();
                };

                Authentication.prototype.saveSession = function () {
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
                };

                Authentication.prototype.login = function (userName, password, rememberMe) {
                    var _this = this;
                    return this.post("/api/authentication", {
                        userName: userName,
                        password: password,
                        tokenName: this.getTokenName()
                    }).done(function (token) {
                        _this.session = {
                            authenticated: true,
                            authenticatedWithToken: false,
                            rememberMe: rememberMe,
                            tokenGuid: token.tokenGuid,
                            userName: userName
                        };
                        _this.saveSession();
                    });
                };

                Authentication.prototype.logout = function () {
                    var _this = this;
                    return this.delete("/api/authentication").done(function () {
                        return _this.logoutSession();
                    });
                };

                Authentication.prototype.registerUser = function (userName, password, rememberMe) {
                    var _this = this;
                    return this.post("/api/registration", {
                        userName: userName,
                        password: password,
                        tokenName: this.getTokenName()
                    }).done(function (token) {
                        _this.session = {
                            authenticated: true,
                            authenticatedWithToken: false,
                            rememberMe: rememberMe,
                            tokenGuid: token.tokenGuid,
                            userName: userName
                        };
                        _this.saveSession();
                    });
                };

                Authentication.prototype.unregisterUser = function (userName, password) {
                    var _this = this;
                    return this.delete("/api/registration").done(function () {
                        return _this.logoutSession();
                    });
                };

                Authentication.prototype.userNameExists = function (userName) {
                    return this.head("/api/registration?username={userName}", {
                        userName: userName
                    });
                };

                Authentication.prototype.delete = function (urlString, data) {
                    var url = this.createUrl(urlString, data);
                    if (_.size(data)) {
                        throw 'Can not pass data in body';
                    }
                    return jQuery.ajax(url, this.ajaxSettings("DELETE"));
                };

                Authentication.prototype.get = function (urlString, data) {
                    var url = this.createUrl(urlString, data);
                    if (_.size(data)) {
                        throw 'Can not pass data in body';
                    }
                    return jQuery.ajax(url, this.ajaxSettings("GET"));
                };

                Authentication.prototype.head = function (urlString, data) {
                    var url = this.createUrl(urlString, data);
                    if (_.size(data)) {
                        throw 'Can not pass data in body';
                    }
                    return jQuery.ajax(url, this.ajaxSettings("HEAD"));
                };

                Authentication.prototype.post = function (urlString, data) {
                    var url = this.createUrl(urlString, data);
                    return jQuery.ajax(url, this.ajaxSettings("POST", data));
                };

                Authentication.prototype.put = function (urlString, data) {
                    var url = this.createUrl(urlString, data);
                    return jQuery.ajax(url, this.ajaxSettings("PUT", data));
                };
                return Authentication;
            })();
            Base.Authentication = Authentication;
        })(Implementation.Base || (Implementation.Base = {}));
        var Base = Implementation.Base;
    })(exports.Implementation || (exports.Implementation = {}));
    var Implementation = exports.Implementation;
});
//# sourceMappingURL=authentication.js.map
