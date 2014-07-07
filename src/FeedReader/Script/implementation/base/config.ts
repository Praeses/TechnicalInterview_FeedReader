/// <reference path='../../lib/modernizr.d.ts' />
/// <reference path='../../model/base/ioc.d.ts' />
export module Implementation.Base {

    export function config(container: Model.Base.IContainer): void {
        container.register('Model.Base.IAuthentication', 'Implementation.Base.Authentication', () => {
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

        container.registerNamed('Model.Base.IHistory', 'Implementation.Base.HistoryHash', 'base.historyHash').singleton();
        container.registerNamed('Model.Base.IHistory', 'Implementation.Base.HistoryHistory', 'base.historyHistory').singleton();
        container.registerNamed('Model.Base.IHistory', 'Implementation.Base.HistoryTimer', 'base.historyTimer').singleton();

        container.registerNamed('Storage', 'Implementation.Base.LocalStorage', 'base.localStorage').singleton();
        container.registerNamed('Storage', 'Implementation.Base.SessionStorage', 'base.sessionStorage').singleton();

        container.register('Model.Base.IRouter', 'Implementation.Base.Router', () => {
            var history;
            if (Modernizr.history) {
                history = container.resolveNamed('Model.Base.IHistory', 'base.historyHistory');
            } else if (Modernizr.hashchange) {
                history = container.resolveNamed('Model.Base.IHistory', 'base.histroyHash');
            } else {
                history = container.resolveNamed('Model.Base.IHistory', 'base.histroyTimer');
            }

            return {
                history: history
            };
        }).singleton();
    }

}