declare module Model.Base {

    interface IUrl {
        //scheme://username:password@hostname:port/path?query#fragment
        //scheme://username:password@hostname:port/#/path?query
        fragment: string;
        hashRouting: boolean;
        hostName: string;
        pathParts: string[];
        password: string;
        port: number;
        query: IUrlQuery;
        scheme: string;
        username: string;

        match(urlString: string): Object;
        parse(urlString: string): IUrl;
        removeVariables(): IUrl;
        replaceVariables(data: any, remove: boolean): IUrl;
        toString(): string;
    }

    interface IUrlQuery {
        [key: string]: string;
    }

}