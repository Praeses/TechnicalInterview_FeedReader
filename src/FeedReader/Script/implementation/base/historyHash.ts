/// <reference path='../../model/base/history.d.ts' />
import HistoryModule = require('implementation/base/history');

export module Implementation.Base {

    export class HistoryHash extends HistoryModule.Implementation.Base.History {

        constructor() {
            super();
        }

        pushHistory(urlString: string): void {
            if (window.location.pathname !== '/') {
                window.location.href = window.location.protocol + '//' +
                    window.location.host + '/#' + urlString;
            } else if (urlString === '/') {
                window.location.hash = '';
            } else {
                window.location.hash = '#' + urlString;
            }
        }

        startHistory(): void {
            jQuery(window).bind("hashchange", () => this.checkUrlCallback());
        }

        stopHistory(): void {
            jQuery(window).unbind("hashchange");
        }

    }

}