/// <reference path='url.d.ts' />
declare module Model.Base {

    interface IRouter {
        catchAllJqCallback: JQueryCallback;
        routes: Array<IRouterRoute>;
        routerRoute(allowAnonymous: boolean, urlString: string): IRouterRoute;
        routerRoute(allowAnonymous: boolean, url: IUrl): IRouterRoute;

        currentRequest(): any;
        currentRoute(): IRouterRoute;
        currentUrlString(): string;

        navigate(urlString: string, reload?: boolean): IRouter;
        navigated(urlString: string): IRouter;
        reset(): IRouter;
        start(): IRouter;
        stop(): IRouter;

        catchAll(): IRouter;
    }

    interface IRouterCatchAllCallback {
        (urlString: string): void;
    }

    interface IRouterRoute {
        allowAnonymous(): boolean;
        routeJqCallback: JQueryCallback;
        url(): IUrl;

        navigate(request: Object): IRouterRoute;
        toUrlString(request: Object): string;

        route(urlString: string, request: Object): IRouterRoute;
    }

    interface IRouterRouteCallback {
        (urlString: string, request: Object): void;
    }

}