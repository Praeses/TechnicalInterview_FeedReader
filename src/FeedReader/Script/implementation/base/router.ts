/// <reference path='../../model/base/history.d.ts' />
/// <reference path='../../model/base/router.d.ts' />
import UrlModule = require('implementation/base/url');
var urlMod = UrlModule.Implementation.Base.Url;

export module Implementation.Base {

    export class Router implements Model.Base.IRouter {
        private $currentRequest: any;
        private $currentRoute: Model.Base.IRouterRoute;

        private navigatedCallback: (urlString: string) => void;

        constructor(private history: Model.Base.IHistory) {
            this.navigatedCallback = (urlString: string) => this.navigated(urlString);
            this.reset();
        }

        catchAllJqCallback: JQueryCallback;
        routes: Array<Model.Base.IRouterRoute> = [];

        currentRequest(): any {
            return this.$currentRequest;
        }

        currentRoute(): Model.Base.IRouterRoute {
            return this.$currentRoute;
        }

        currentUrlString(): string {
            return this.history.getCurrentUrlString();
        }

        addCatchAllCallback(callback: Model.Base.IRouterCatchAllCallback): Model.Base.IRouter {
            this.catchAllJqCallback.add(callback);
            return this;
        }

        catchAll(): Model.Base.IRouter {
            return this;
        }

        navigate(urlString: string, reload?: boolean): Model.Base.IRouter {
            this.history.navigate(urlString, reload);
            return this;
        }

        navigated(urlString: string): Model.Base.IRouter {
            if (!_.some(this.routes, (route) => {
                var request = route.url().match(urlString);
                if (request) {
                    this.$currentRequest = request;
                    this.$currentRoute = route;
                    route.route(urlString, request);
                    return true;
                }

                return false;
            })) {
                this.catchAllJqCallback.fire(urlString);
            }

            return this;
        }

        removeCatchAllCallback(callback: Model.Base.IRouterCatchAllCallback): Model.Base.IRouter {
            this.catchAllJqCallback.remove(callback);
            return this;
        }

        routerRoute(allowAnonymous: boolean, urlString: string): Model.Base.IRouterRoute;
        routerRoute(allowAnonymous: boolean, url: Model.Base.IUrl): Model.Base.IRouterRoute;
        routerRoute(allowAnonymous: boolean, urlObject: any): Model.Base.IRouterRoute {
            if (_.isString(urlObject)) {
                return new RouterRoute(allowAnonymous, new urlMod(urlObject), this);
            }

            return new RouterRoute(allowAnonymous, urlObject, this);
        }

        reset(): Model.Base.IRouter {
            this.stop();
            this.history.reset();
            this.catchAllJqCallback = jQuery.Callbacks('unique');
            _.forEach(this.routes, (route) => route.routeJqCallback.empty());
            this.routes = [];
            return this;
        }

        start(): Model.Base.IRouter {
            this.history.navigatedJqCallback.add(this.navigatedCallback);
            this.history.start();
            return this;
        }

        stop(): Model.Base.IRouter {
            this.history.navigatedJqCallback.remove(this.navigatedCallback);
            this.history.stop();
            return this;
        }

    }

    class RouterRoute implements Model.Base.IRouterRoute {
        private $allowAnonymous: boolean;
        private $url: Model.Base.IUrl;

        constructor(allowAnonymous: boolean, url: Model.Base.IUrl, private router: Model.Base.IRouter) {
            this.$allowAnonymous = allowAnonymous;
            this.$url = url;
        }

        routeJqCallback: JQueryCallback = jQuery.Callbacks('unique');

        allowAnonymous(): boolean {
            return this.$allowAnonymous;
        }

        navigate(request: Object): Model.Base.IRouterRoute {
            this.router.navigate(this.toUrlString(request));
            return this;
        }

        route(urlString: string, request: Object): Model.Base.IRouterRoute {
            this.routeJqCallback.fire(urlString, request);
            return this;
        }

        toUrlString(request: Object): string {
            return this.$url.replaceVariables(request, false).removeVariables().toString();
        }

        url(): Model.Base.IUrl {
            return this.$url;
        }

    }

}