declare module Model.Base {

    interface IHistory {
        navigatedJqCallback: JQueryCallback;

        getCurrentUrlString(): string;
        navigate(urlString: string, reload?: boolean): IHistory;
        navigated(urlString: string): IHistory;
        reset(): IHistory;
        start(): IHistory;
        stop(): IHistory;
    }

    interface IHistoryCallback {
        (urlString: string): void;
    }

}