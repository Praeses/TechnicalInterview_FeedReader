define(["require", "exports"], function(require, exports) {
    (function (Implementation) {
        (function (Layout) {
            /// <reference path='../../../model/base/ioc.d.ts' />
            (function (Default) {
                function config(container) {
                    container.registerNamed('Model.Base.IAnonymousLayout', 'Implementation.Layout.Default.AnonymousLayout', 'layout.default.anonymousLayout', function () {
                        return {
                            viewModel: {},
                            authentication: container.resolve('Model.Base.IAuthentication'),
                            router: container.resolve('Model.Base.IRouter')
                        };
                    });

                    container.registerNamed('Model.Base.IAuthenticatedLayout', 'Implementation.Layout.Default.AuthenticatedLayout', 'layout.default.authenticatedLayout', function () {
                        return {
                            viewModel: {},
                            authentication: container.resolve('Model.Base.IAuthentication'),
                            router: container.resolve('Model.Base.IRouter')
                        };
                    });

                    container.registerNamed('Model.Base.ILayoutController', 'Implementation.Base.LayoutController', 'Default', function () {
                        return {
                            anonymousLayoutResolver: container.lazyResolveNamed('Model.Base.IAnonymousLayout', 'layout.default.anonymousLayout'),
                            authentication: container.resolve('Model.Base.IAuthentication'),
                            authenticatedLayoutResolver: container.lazyResolveNamed('Model.Base.IAuthenticatedLayout', 'layout.default.authenticatedLayout'),
                            router: container.resolve('Model.Base.IRouter')
                        };
                    }).singleton();
                }
                Default.config = config;
            })(Layout.Default || (Layout.Default = {}));
            var Default = Layout.Default;
        })(Implementation.Layout || (Implementation.Layout = {}));
        var Layout = Implementation.Layout;
    })(exports.Implementation || (exports.Implementation = {}));
    var Implementation = exports.Implementation;
});
//# sourceMappingURL=config.js.map
