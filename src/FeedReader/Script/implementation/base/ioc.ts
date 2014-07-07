/// <reference path='../../lib/underscore.d.ts' />
/// <reference path='../../model/base/ioc.d.ts' />
export module Implementation.Base {

    var registrations: { [fullName: string]: Model.Base.IContainerRegistration } = {};
    var stripComments: RegExp = /((\/\/.*$)|(\/\*[\s\S]*?\*\/)|\s)/mg;

    function createFullName(modelName: string, name?: string): string {
        return name + ':' + modelName;
    }

    export class Container implements Model.Base.IContainer {

        private resolveLazy(
            modelName: string,
            parameters?: { [parameterName: string]: any },
            lazyParameters?: { [parameterName: string]: any }) {
            return this.resolve(modelName, _.extend({}, parameters, lazyParameters));
        }

        private resolveLazyNamed(
            modelName: string,
            name: string,
            parameters?: { [parameterName: string]: any },
            lazyParameters?: { [parameterName: string]: any }) {
            return this.resolveNamed(modelName, name, _.extend({}, parameters, lazyParameters));
        }

        lazyResolve(modelName: string, parameters?: Model.Base.IContainerParameterFactory): Model.Base.IContainerLazyResolver {
            return <Model.Base.IContainerLazyResolver>_.bind(this.resolveLazy, this, modelName, parameters);
        }

        lazyResolveNamed(
            modelName: string,
            name: string,
            parameters?: Model.Base.IContainerParameterFactory): Model.Base.IContainerLazyResolver {
            return <Model.Base.IContainerLazyResolver>_.bind(this.resolveLazyNamed, this, modelName, name, parameters);
        }

        register(
            modelName: string,
            implementationName: string,
            parameterFactory?: Model.Base.IContainerParameterFactory): Model.Base.IContainerRegistration {
            return new ContainerRegistration(modelName, implementationName, undefined, parameterFactory);
        }

        registerNamed(
            modelName: string,
            implementationName: string,
            name: string,
            parameterFactory?: Model.Base.IContainerParameterFactory): Model.Base.IContainerRegistration {
            return new ContainerRegistration(modelName, implementationName, name, parameterFactory);
        }

        resolve(modelName: string, parameters?: { [parameterName: string]: any }): JQueryPromise<any> {
            return this.resolveNamed(modelName, undefined, parameters);
        }

        resolveNamed(modelName: string, name: string, parameters?: { [parameterName: string]: any }): JQueryPromise<any> {
            var fullName = createFullName(modelName, name);
            var registration = registrations[fullName];
            if (!registration) {
                throw fullName + ' is not registered.';
            }

            return registration.resolve(parameters);
        }

    }

    export class ContainerRegistration implements Model.Base.IContainerRegistration {
        private implementation: any;
        private parameterNames: string[] = [];
        private singletonDeferred: JQueryDeferred<any>;

        private loadImplementation(): JQueryPromise<any> {
            var deferred = jQuery.Deferred();

            if (this.implementation) {
                deferred.resolve(this.implementation);
                return deferred.promise();
            }

            var moduleName = this.implementationName.charAt(0)
                .toLowerCase() + this.implementationName.slice(1);
            moduleName = moduleName
                .replace(/\.([A-Z])/g, (match, p1?) => '/' + p1.toLowerCase());

            require([moduleName], (implementation) => {
                _.forEach(this.implementationName.split('.'), (part) => {
                    implementation = implementation[part];
                    if (!implementation) {
                        throw 'implementation not defined.';
                    }
                });
                this.implementation = implementation;

                var code = implementation.toString().replace(stripComments, '');
                var parameters = code.slice(code.indexOf('(') + 1, code.indexOf(')'));
                if (parameters) {
                    this.parameterNames = parameters.split(',');
                }

                deferred.resolve(implementation);
            });

            return deferred.promise();
        }

        constructor(
            private modelName: string,
            private implementationName: string,
            private name: string,
            private parameterFactory?: Model.Base.IContainerParameterFactory) {
            var fullName = createFullName(modelName, name);
            if (registrations[fullName]) {
                throw 'modelName/name has already been defined!';
            }

            registrations[fullName] = this;
        }

        resolve(parameters?: { [parameterName: string]: any }): JQueryPromise<any> {
            if (this.singletonDeferred) {
                return this.singletonDeferred.promise();
            }

            var deferred = jQuery.Deferred();
            if (this.singletonDeferred === null) {
                this.singletonDeferred = deferred;
            }

            this.loadImplementation()
                .done((implementation) => {
                    var deferreds: JQueryDeferred<any>[] = [];
                    var factoryParameters: { [parameterName: string]: any } = {};
                    var resolvedParameters: { [parameterName: string]: any } = {};

                    if (this.parameterFactory) {
                        factoryParameters = this.parameterFactory();
                    }

                    _.forEach(this.parameterNames, (parameterName) => {
                        var parameterValue;
                        if (parameters && _.has(parameters, parameterName)) {
                            parameterValue = parameters[parameterName];
                        } else if (_.has(factoryParameters, parameterName)) {
                            parameterValue = factoryParameters[parameterName];
                        } else {
                            throw 'Parameter ' + parameterName + ' is not defined for ' + this.implementationName;
                        }

                        if (_.isFunction(parameterValue.promise)) {
                            deferreds.push(parameterValue);
                            parameterValue.done((instance) => resolvedParameters[parameterName] = instance);
                            return;
                        }

                        resolvedParameters[parameterName] = parameterValue;
                    });

                    jQuery.when.apply(this, deferreds)
                        .done(() => {
                            var args = [null];
                            _.forEach(this.parameterNames, (parameterName) => {
                                args.push(resolvedParameters[parameterName]);
                            });
                            var factory = implementation.bind.apply(implementation, args);
                            deferred.resolve(new factory());
                        });
                });

            return deferred.promise();
        }

        singleton(): Model.Base.IContainerRegistration {
            this.singletonDeferred = null;
            return this;
        }

    }

}