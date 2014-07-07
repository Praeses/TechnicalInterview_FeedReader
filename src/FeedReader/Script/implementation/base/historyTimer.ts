/// <reference path='../../model/base/history.d.ts' />
import HistoryModule = require('implementation/base/history');

export module Implementation.Base {

    export class HistoryTimer extends HistoryModule.Implementation.Base.History {

        constructor(private checkIntervalMsec: number = 50) {
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
            this.checkIntervalMsec = window.setInterval(
                () => this.checkUrlCallback(),
                this.checkIntervalMsec);
        }

        stopHistory(): void {
            window.clearInterval(this.checkIntervalMsec);
        }

    }

}