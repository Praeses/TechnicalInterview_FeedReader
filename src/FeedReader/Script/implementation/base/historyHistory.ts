/// <reference path='../../model/base/history.d.ts' />
import HistoryModule = require('implementation/base/history');

export module Implementation.Base {

    export class HistoryHistory extends HistoryModule.Implementation.Base.History {

        constructor() {
            super();
        }

        pushHistory(urlString: string): void {
            window.history.pushState(
                {},
                window.document.title,
                window.location.protocol + '//' + window.location.host + urlString);
        }

        startHistory(): void {
            jQuery(window).bind("popstate", () => this.checkUrlCallback());
        }

        stopHistory(): void {
            jQuery(window).unbind("popstate");
        }

    }

}