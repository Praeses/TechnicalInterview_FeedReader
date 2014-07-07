/// <reference path='../../../model/base/ioc.d.ts' />
export module Implementation.Layout.Default {

    export function config(container: Model.Base.IContainer): void {
        container.registerNamed('Model.Base.IAnonymousLayout', 'Implementation.Layout.Default.AnonymousLayout', 'layout.default.anonymousLayout', () => {
            return {
                viewModel: {},
                authentication: container.resolve('Model.Base.IAuthentication'),
                router: container.resolve('Model.Base.IRouter'),
            };
        });

        container.registerNamed('Model.Base.IAuthenticatedLayout', 'Implementation.Layout.Default.AuthenticatedLayout', 'layout.default.authenticatedLayout', () => {
            return {
                viewModel: {},
                authentication: container.resolve('Model.Base.IAuthentication'),
                router: container.resolve('Model.Base.IRouter'),
            };
        });

        container.registerNamed('Model.Base.ILayoutController', 'Implementation.Base.LayoutController', 'Default', () => {
            return {
                anonymousLayoutResolver: container.lazyResolveNamed('Model.Base.IAnonymousLayout', 'layout.default.anonymousLayout'),
                authentication: container.resolve('Model.Base.IAuthentication'),
                authenticatedLayoutResolver: container.lazyResolveNamed('Model.Base.IAuthenticatedLayout', 'layout.default.authenticatedLayout'),
                router: container.resolve('Model.Base.IRouter')
            };
        }).singleton();

    }

}