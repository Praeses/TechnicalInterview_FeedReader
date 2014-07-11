/// <reference path='../base/url.d.ts' />
declare module Model.Api {

    interface IDto {
        send<TRequest, TResponse>(request: IDtoRequest<TRequest, TResponse>): IDtoPromise<TResponse>;
    }

    interface IDtoError {
        dumpObjects: Object[];
        exceptionType: string;
        innerException: IDtoError;
        message: string;
        status: number;
        statusText: string;
    }

    interface IDtoRequest<TRequest, TResponse> {
        data: TRequest;
        method: string;
        url: Model.Base.IUrl;
    }

    interface IDtoFilterCallback<T> {
        (value?: T, jqXhr?: JQueryXHR): void;
    }

    interface IDtoPromiseCallbackAlways {
        (jqXhr?: JQueryXHR): void;
    }

    interface IDtoPromiseCallback<T> {
        (value?: T, jqXhr?: JQueryXHR): void;
    }

    interface IDtoPromise<T> {
        always(callback?: IDtoPromiseCallbackAlways): IDtoPromise<T>;
        done(callback?: IDtoPromiseCallback<T>): IDtoPromise<T>;
        fail(callback?: IDtoPromiseCallback<IDtoError>): IDtoPromise<T>;
        then<TFilter>(callback: IDtoFilterCallback<T>): IDtoPromise<TFilter>;
    }
}