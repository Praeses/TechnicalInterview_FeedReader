/// <reference path='../../model/api/authenticationApi.d.ts' />
export module Implementation.View {

    export class ViewController {
        private view: Model.Base.IView;

        private authenticateChangedCallback: Model.Api.IAuthenticationApiChangedCallback = (): void => {
            if (!jQuery.isReady) {
                return;
            }

            this.view = null;

            this.authenticationApi.changedJqCallback.empty();
            this.authenticationApi.changedJqCallback.add(this.authenticateChangedCallback);

            if (!this.authenticationApi.session.authenticated) {
                this.anonymousViewResolver()
                    .done((view: Model.Base.IView) => {
                        this.view = view;
                        this.view.render(jQuery('#boot'));
                    });
                return;
            }

            this.authenticatedViewResolver()
                .done((view: Model.Base.IView) => {
                    this.view = view;
                    this.view.render(jQuery('#boot'));
                });
        };

        constructor(
            private anonymousViewResolver: Model.Base.IContainerLazyResolver,
            private authenticatedViewResolver: Model.Base.IContainerLazyResolver,
            private authenticationApi: Model.Api.IAuthenticationApi) {

            this.authenticationApi.changedJqCallback.add(this.authenticateChangedCallback);
        }

    }

}