/// <reference path='../../lib/jquery.d.ts' />
/// <reference path='../../model/base/history.d.ts' />
export module Implementation.Base {

    export class History implements Model.Base.IHistory {
        private currentUrlString: string;

        private getUrl(): string {
            var urlString = window.location.pathname;
            if (window.location.hash) {
                urlString += '/' + window.location.hash.slice(1);
            }

            if (window.location.search) {
                urlString += window.location.search;
            }

            return this.normalizeUrl(urlString);
        }

        private normalizeUrl(urlString): string {
            return '/' + urlString.
                replace(/\/+/g, '/').
                replace(/^\/|\/$/g, '');
        }

        constructor() {
            this.reset();
        }

        navigatedJqCallback: JQueryCallback;

        checkUrlCallback(): void {
            this.navigated(this.getUrl());
        }

        getCurrentUrlString(): string {
            return this.currentUrlString;
        }

        pushHistory(urlString: string): void {
            throw 'abstract';
        }

        navigate(urlString: string, reload?: boolean): Model.Base.IHistory {
            urlString = this.normalizeUrl(urlString);
            if (reload) {
                window.location.href = urlString;
                return this;
            }

            this.pushHistory(urlString);
            return this.navigated(urlString);
        }

        navigated(urlString: string): Model.Base.IHistory {
            this.currentUrlString = this.normalizeUrl(urlString);
            this.navigatedJqCallback.fire(this.currentUrlString);
            return this;
        }

        reset(): Model.Base.IHistory {
            this.stop();
            this.navigatedJqCallback = jQuery.Callbacks('memory unique');
            return this;
        }

        start(): Model.Base.IHistory {
            this.startHistory();
            this.checkUrlCallback();
            return this;
        }

        startHistory(): void {
            throw 'abstract';
        }

        stop(): Model.Base.IHistory {
            this.stopHistory();
            return this;
        }

        stopHistory(): void {
            throw 'abstract';
        }

    }

}