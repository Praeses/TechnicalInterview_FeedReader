/// <reference path='../base/url.d.ts' />
declare module Model.Api {

    interface IDto {
        send<TRequest, TResponse>(request: IDtoRequest<TRequest, TResponse>): JQueryPromise<IDtoResponse<TResponse>>;
    }

    interface IDtoRequest<TRequest, TResponse> {
        data: TRequest;
        method: string;
        url: Model.Base.IUrl;
    }

    interface IDtoResponse<T> {
        data: T;
        status: number;
        statusText: string;
    }
}