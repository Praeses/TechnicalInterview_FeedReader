/// <reference path='../../lib/underscore.d.ts' />
export module Implementation.Base {

    export class SessionStorage /*implements Storage*/ {
        private sessions: { [key: string]: any } = {};
        length: number = 0;
        remainingSpace: number = 0;

        private get() {
            try {
                this.sessions = JSON.parse(window.name);
            } catch (e) {
                this.sessions = {};
            }
        }

        private set(): void {
            window.name = JSON.stringify(this.sessions);
        }

        clear(): void {
            this.sessions = {};
            this.set();
            this.length = 0;
        }

        getItem(key: string): any {
            return this.sessions[key] || null;
        }

        key(index: number): string {
            return _.keys(this.sessions)[index];
        }

        removeItem(key: string): void {
            delete this.sessions[key];
            this.set();
            this.length = 0;
            _.forEach(this.sessions, () => this.length += 1);
        }

        setItem(key: string, data: string): void {
            this.sessions[key] = data;
            this.set();
            this.length = 0;
            _.forEach(this.sessions, () => this.length += 1);
        }

    }

}