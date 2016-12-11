export module Implementation.Base {

    export class LocalStorage /*implements Storage*/ {
        length: number = 0;
        remainingSpace: number = 0;

        clear(): void {
        }

        getItem(key: string): any {
            return null;
        }

        key(index: number): string {
            return null;
        }

        removeItem(key: string): void {
        }

        setItem(key: string, data: string): void {
        }

    }

}