/// <reference path='../../lib/jquery.d.ts' />
declare module Model.Base {

    interface IContainer {
        lazyResolve(modelName: string, parameters?: IContainerParameterFactory): IContainerLazyResolver;
        lazyResolveNamed(modelName: string, name: string, parameters?: IContainerParameterFactory): IContainerLazyResolver;
        register(modelName: string, implementationName: string, parameterFactory?: IContainerParameterFactory): IContainerRegistration;
        registerNamed(modelName: string, implementationName: string, name: string, parameterFactory?: IContainerParameterFactory): IContainerRegistration;
        resolve(modelName: string, parameters?: { [parameterName: string]: any }): JQueryPromise<any>;
        resolveNamed(modelName: string, name: string, parameters?: { [parameterName: string]: any }): JQueryPromise<any>;
    }

    interface IContainerLazyResolver {
        (parameters?: { [name: string]: any }): JQueryPromise<any>;
    }

    interface IContainerParameterFactory {
        (): { [name: string]: any };
    }

    interface IContainerRegistration {
        resolve(parameters?: { [parameterName: string]: any }): JQueryPromise<any>;
        singleton(): IContainerRegistration;
    }

}