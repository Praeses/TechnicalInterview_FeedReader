/// <reference path='../../model/base/url.d.ts' />
export module Implementation.Base {
    var variableOptional = '?';
    var variableStart = '{';
    var variableStop = '}';
    var variableWild = '*';

    function isMatch(variableName: string, variable: string, value: string): boolean {
        if (variable === requiredVariable(variableName)) {
            if (_.isUndefined(value)) {
                throw 'Cannot replace {required} variable [' + variable + '] with undefined value.';
            }

            return true;
        }

        if (_.isUndefined(value)) {
            return false;
        }

        if ((variable === optionalVariable(variableName)) ||
        (variable === wildVariable(variableName))) {
            return true;
        }

        return false;
    }

    function isOptionalVariable(value: string): boolean {
        if (!isVariable(value)) {
            return false;
        }

        return value[value.length - 2] === variableOptional;
    }

    function isRemovableVariable(value: string): boolean {
        if (!isVariable(value)) {
            return false;
        }

        if ((value[value.length - 2] === variableOptional) ||
        (value[value.length - 2] === variableWild)) {
            return true;
        }

        throw 'Cannot remove required variable [' + value + '].';
    }

    function isRequiredVariable(value: string): boolean {
        if (!isVariable(value)) {
            return false;
        }

        if ((value[value.length - 2] === variableOptional) ||
        (value[value.length - 2] === variableWild)) {
            return false;
        }

        return true;
    }

    function isVariable(value: string): boolean {
        if (!value || (value.length < 3)) {
            return false;
        }

        if ((value[0] !== variableStart) ||
        (value[value.length - 1] !== variableStop)) {
            return false;
        }

        return true;
    }

    function isWildVariable(value: string): boolean {
        if (!isVariable(value)) {
            return false;
        }

        return value[value.length - 2] === variableWild;
    }

    function matchValue(thisValue: string, thatValue: string, request: Object): boolean {
        var name = variableName(thisValue);
        if (!name) {
            return thisValue === thatValue;
        }

        if (_.isUndefined(thatValue)) {
            if (isRequiredVariable(thisValue)) {
                return false;
            }
        }

        if (request[name]) {
            throw 'Multiple values for [' + name + '] found.';
        }

        request[name] = thatValue;
        return true;
    }

    function optionalVariable(variableName: string): string {
        return variableStart + variableName + variableOptional + variableStop;
    }

    function requiredVariable(variableName: string): string {
        return variableStart + variableName + variableStop;
    }

    function variableName(variable: string): string {
        if (!isVariable(variable)) {
            return null;
        }

        if ((variable[variable.length - 2] === variableOptional) ||
        (variable[variable.length - 2] === variableWild)) {
            return variable.substring(1, variable.length - 2);
        }

        return variable.substring(1, variable.length - 1);
    }

    function wildVariable(variableName: string): string {
        return variableStart + variableName + variableWild + variableStop;
    }


    export class Url implements Model.Base.IUrl {

        private replaceVariable(variableName: string, value: string): boolean {
            var replaced = false;

            if (isMatch(variableName, this.fragment, value)) {
                replaced = true;
                this.fragment = value;
            }

            if (isMatch(variableName, this.hostName, value)) {
                replaced = true;
                this.hostName = value;
            }

            _.forEach(this.pathParts, (part, index) => {
                if (isMatch(variableName, part, value)) {
                    replaced = true;
                    this.pathParts[index] = value;
                }
            });

            if (isMatch(variableName, this.password, value)) {
                replaced = true;
                this.password = value;
            }

            var replacedQuery: Model.Base.IUrlQuery = {};
            _.forEach(this.query, (queryValue, key) => {
                if (isMatch(variableName, key, value)) {
                    replaced = true;
                    key = value;
                }

                if (isMatch(variableName, queryValue, value)) {
                    replaced = true;
                    queryValue = value;
                }

                replacedQuery[key] = queryValue;
            });
            this.query = replacedQuery;

            if (isMatch(variableName, this.scheme, value)) {
                replaced = true;
                this.scheme = value;
            }

            if (isMatch(variableName, this.username, value)) {
                replaced = true;
                this.username = value;
            }

            return replaced;
        }

        constructor(urlString: string);
        constructor(urlString: Model.Base.IUrl);
        constructor(urlString: any) {
            if (_.isString(urlString)) {
                this.parse(urlString);
                return;
            }

            _.extend(this, urlString);
        }

        fragment: string;
        hashRouting: boolean;
        hostName: string;
        pathParts: string[];
        password: string;
        port: number;
        query: Model.Base.IUrlQuery;
        scheme: string;
        username: string;

        match(urlString: string): Object {
            var request: Object = {};
            var url = new Url(urlString);

            if (!matchValue(this.fragment, url.fragment, request)) {
                return null;
            }

            if (!matchValue(this.hostName, url.hostName, request)) {
                return null;
            }

            if ((this.pathParts.length < url.pathParts.length) &&
                !isWildVariable(this.pathParts[this.pathParts.length])) {
                return null;
            }

            if (!_.every(this.pathParts, (part, index) => {
                var thatPart;
                if (isWildVariable(part)) {
                    thatPart = url.pathParts.slice(index).join('/');
                } else {
                    thatPart = url.pathParts[index];
                }

                return matchValue(part, thatPart, request);
            })) {
                return null;
            }

            if (!matchValue(this.password, url.password, request)) {
                return null;
            }

            if (!_.every(this.query, (value: any, key) => {
                if (isVariable(key.toString())) {
                    throw 'Query keys cannot be {variables}. [' + key + ']';
                }

                return matchValue(value, url.query[key], request);
            })) {
                return null;
            }

            if (!matchValue(this.scheme, url.scheme, request)) {
                return null;
            }

            if (!matchValue(this.username, url.username, request)) {
                return null;
            }

            return request;
        }

        parse(urlString: string): Model.Base.IUrl {
            var index: number;

            this.hashRouting = false;
            this.hostName = urlString;
            this.pathParts = [];
            this.query = {};

            index = this.hostName.indexOf('#');
            if (index !== -1) {
                this.fragment = this.hostName.substring(index + 1);
                this.hostName = this.hostName.substring(0, index);
            }

            var query;
            index = this.hostName.split(variableOptional + variableStop).join('xx').indexOf('?');
            if (index !== -1) {
                query = this.hostName.substring(index + 1);
                this.hostName = this.hostName.substring(0, index);
            }

            index = this.hostName.indexOf('://');
            if (index !== -1) {
                this.scheme = this.hostName.substring(0, index);
                this.hostName = this.hostName.substring(index + 3);
            }

            index = this.hostName.indexOf('@');
            if (index !== -1) {
                this.username = this.hostName.substring(index + 1);
                this.hostName = this.hostName.substring(0, index);

                index = this.username.indexOf(':');
                if (index !== -1) {
                    this.password = this.username.substring(index + 1);
                    this.username = this.username.substring(0, index);
                }
            }

            var path;
            index = this.hostName.indexOf('/');
            if (index !== -1) {
                path = this.hostName.substring(index + 1);
                this.hostName = this.hostName.substring(0, index);
            }

            index = this.hostName.indexOf(':');
            if (index !== -1) {
                this.port = parseInt(this.hostName.substring(index + 1));
                this.hostName = this.hostName.substring(0, index);
            }

            if (!path && !query && this.fragment) {
                path = this.fragment;
                this.fragment = undefined;
                this.hashRouting = true;

                index = path.replace(variableOptional + variableStop, 'xx').indexOf('?');
                if (index !== -1) {
                    query = path.substring(index + 1);
                    path = path.substring(0, index);
                }
            }

            if (query) {
                _.forEach(query.split('&'), (kvp: any) => {
                    if (!kvp) {
                        return;
                    }

                    var parts = (kvp).split('=');
                    if (!parts[0]) {
                        throw 'Query keys cannot be empty.';
                    }

                    var key = decodeURIComponent(parts[0]);
                    if (isWildVariable(key)) {
                        throw 'Query keys cannot contain {wild*} variables. [' + parts[0] + ']';
                    }

                    var value = parts[1] ? decodeURIComponent(parts[1]) : undefined;
                    if (isWildVariable(value)) {
                        throw 'Query values cannot contain {wild*} variables. [' + parts[1] + ']';
                    }

                    this.query[key] = value;
                });
            }

            if (path === '') {
                this.pathParts.push(path);
            } else if (path) {
                var optional = false;
                var wild = false;
                this.pathParts = path.split('/');
                _.forEach(this.pathParts, (part, pathPartIndex) => {
                    if (part) {
                        part = decodeURIComponent(part);
                    }

                    if (optional && !isOptionalVariable(part) && !isWildVariable(part)) {
                        throw 'Required part found after {optional?} part [' + part + ']';
                    }

                    if (wild) {
                        throw 'Part found after {wild*} part [' + part + ']';
                    }

                    optional = isOptionalVariable(part);
                    wild = isWildVariable(part);

                    this.pathParts[pathPartIndex] = part;
                });
            }

            if (this.fragment) {
                this.fragment = decodeURIComponent(this.fragment);
            }

            if (this.hostName) {
                this.hostName = decodeURIComponent(this.hostName);
            }

            if (this.password) {
                this.password = decodeURIComponent(this.password);
            }

            if (this.scheme) {
                this.scheme = decodeURIComponent(this.scheme);
            }

            if (this.username) {
                this.username = decodeURIComponent(this.username);
            }

            return this;
        }

        removeVariables(): Model.Base.IUrl {
            if (isRemovableVariable(this.fragment)) {
                this.fragment = undefined;
            }

            if (isRemovableVariable(this.hostName)) {
                this.hostName = undefined;
            }

            this.pathParts = _.filter(this.pathParts, (part) => !isRemovableVariable(part));

            if (isRemovableVariable(this.password)) {
                this.password = undefined;
            }

            var filteredQuery: Model.Base.IUrlQuery = {};
            _.forEach(this.query, (value, key) => {
                if (!isRemovableVariable(value)) {
                    filteredQuery[key] = value;
                }
            });
            this.query = filteredQuery;

            if (isRemovableVariable(this.scheme)) {
                this.scheme = undefined;
            }

            if (isRemovableVariable(this.username)) {
                this.username = undefined;
            }

            return this;
        }

        replaceVariables(data: any, remove: boolean): Model.Base.IUrl {
            var url = new Url(this);

            _.forEach(data, (value, key: any) => {
                if (url.replaceVariable(key, value) && remove) {
                    delete data[key];
                }
            });

            return url;
        }

        toString(): string {
            var urlString = '';

            if (this.scheme) {
                urlString += encodeURIComponent(this.scheme) + '://';
            }

            if (this.username || this.password) {
                urlString += encodeURIComponent(this.username);
                if (this.password) {
                    urlString += ':' + encodeURIComponent(this.password);
                }

                urlString += '@';
            }

            urlString += encodeURIComponent(this.hostName);
            if (this.port && (this.port !== 80)) {
                urlString += ':' + this.port;
            }

            if (this.hashRouting) {
                urlString += '/#';
            }

            _.forEach(this.pathParts, (part) => urlString += '/' + encodeURIComponent(part));

            if (this.fragment) {
                urlString += '#' + encodeURIComponent(this.fragment);
            }


            if (_.size(this.query) > 0) {
                var queryString = '';
                _.forEach(this.query, (value, key) => queryString += '&' + key + '=' + encodeURIComponent(value));
                urlString += '?' + queryString.substring(1);
            }

            return urlString;
        }

    }

}