define(["require", "exports"], function(require, exports) {
    (function (Implementation) {
        /// <reference path='../../lib/modernizr.d.ts' />
        /// <reference path='../../model/base/ioc.d.ts' />
        (function (Base) {
            function config(container) {
                container.registerNamed('Model.Base.IHistory', 'Implementation.Base.HistoryHash', 'base.historyHash').singleton();
                container.registerNamed('Model.Base.IHistory', 'Implementation.Base.HistoryHistory', 'base.historyHistory').singleton();
                container.registerNamed('Model.Base.IHistory', 'Implementation.Base.HistoryTimer', 'base.historyTimer').singleton();

                container.registerNamed('Storage', 'Implementation.Base.LocalStorage', 'base.localStorage').singleton();
                container.registerNamed('Storage', 'Implementation.Base.SessionStorage', 'base.sessionStorage').singleton();

                container.register('Model.Base.IRouter', 'Implementation.Base.Router', function () {
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
            Base.config = config;
        })(Implementation.Base || (Implementation.Base = {}));
        var Base = Implementation.Base;
    })(exports.Implementation || (exports.Implementation = {}));
    var Implementation = exports.Implementation;
});
//# sourceMappingURL=config.js.map
