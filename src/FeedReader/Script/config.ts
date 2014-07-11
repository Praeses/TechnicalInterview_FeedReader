/// <reference path='lib/modernizr.d.ts' />
/// <reference path='model/base/ioc.d.ts' />
/// <amd-dependency path='implementation/binding/iso6801' />
import IocModule = require('implementation/base/ioc');

export module Configure {
    var container = new IocModule.Implementation.Base.Container();

    // Configure Base.
    container.registerNamed('Storage', 'Implementation.Base.LocalStorage', 'base.localStorage').singleton();
    container.registerNamed('Storage', 'Implementation.Base.SessionStorage', 'base.sessionStorage').singleton();

    // Configure Class
    container.register('Model.Class.IRssClass', 'Implementation.Class.RssClass', () => {
        return {
            channelApi: container.resolve('Model.Api.IChannelApi'),
            userItemApi: container.resolve('Model.Api.IUserItemApi')
        };
    });

    // Configure View
    container.register('Implementation.View.ViewController', 'Implementation.View.ViewController', () => {
        return {
            anonymousViewResolver: container.lazyResolveNamed('Model.Base.IView', 'view.default.anonymousView'),
            authenticatedViewResolver: container.lazyResolveNamed('Model.Base.IView', 'view.default.authenticatedView'),
            authenticationApi: container.resolve('Model.Api.IAuthenticationApi')
        };
    }).singleton();

    // Configure View/Default
    container.registerNamed('Model.Base.IView', 'Implementation.View.Default.AnonymousView', 'view.default.anonymousView', () => {
        return {
            viewModel: {},
            authenticationApi: container.resolve('Model.Api.IAuthenticationApi'),
            registrationApi: container.resolve('Model.Api.IRegistrationApi')
        };
    });

    container.registerNamed('Model.Base.IView', 'Implementation.View.Default.AuthenticatedView', 'view.default.authenticatedView', () => {
        return {
            viewModel: {},
            authenticationApi: container.resolve('Model.Api.IAuthenticationApi'),
            channelApi: container.resolve('Model.Api.IChannelApi'),
            userItemApi: container.resolve('Model.Api.IUserItemApi'),
            rssClass: container.resolve('Model.Class.IRssClass')
        };
    });

    // Configure Api.
    container.register('Model.Api.IChannelApi', 'Implementation.Api.ChannelApi', () => {
        return {
            dto: container.resolve('Model.Api.IDto'),
        };
    });

    container.register('Model.Api.IUserItemApi', 'Implementation.Api.UserItemApi', () => {
        return {
            dto: container.resolve('Model.Api.IDto'),
        };
    });

    container.register('Model.Api.IAuthenticationApi', 'Implementation.Api.AuthApi', () => {
        var localStorage;
        if (Modernizr.localstorage) {
            localStorage = window.localStorage;
        } else {
            localStorage = container.resolveNamed('Storage', 'api.localStorage');
        }

        var sessionStorage;
        if (Modernizr.sessionstorage) {
            sessionStorage = window.sessionStorage;
        } else {
            sessionStorage = container.resolveNamed('Storage', 'api.sessionStorage');
        }

        return {
            localStorage: localStorage,
            sessionStorage: sessionStorage
        };
    }).singleton();

    container.resolve('Model.Api.IAuthenticationApi')
        .done((authApi) => {
            container.registerSingleton('Model.Api.IRegistrationApi', authApi);
            container.registerSingleton('Model.Api.IDto', authApi);

            container.resolve('Implementation.View.ViewController');
        });
}