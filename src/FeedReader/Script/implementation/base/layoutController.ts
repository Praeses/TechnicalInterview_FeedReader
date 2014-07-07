/// <reference path='../../model/base/authentication.d.ts' />
/// <reference path='../../model/base/layoutController.d.ts' />
import ViewModule = require('implementation/base/view');

export module Implementation.Base {

    export class LayoutController implements Model.Base.ILayoutController {
        private currentLayout: Model.Base.ILayout;

        private authenticateChangedCallback: Model.Base.IAuthenticationChangedCallback = (): void => {
            this.render();
        };

        private render(): Model.Base.ILayoutController {
            if (!jQuery.isReady) {
                return this;
            }

            this.currentLayout = null;

            this.authentication.reset();
            this.authentication.changedJqCallback.add(this.authenticateChangedCallback);

            this.router.reset();
            this.router.start();

            if (!this.authentication.session.authenticated) {
                this.anonymousLayoutResolver()
                    .done((layout: Model.Base.ILayout) => {
                        this.currentLayout = layout;
                        layout.render(jQuery('#boot'));
                    });
                return this;
            }

            this.authenticatedLayoutResolver()
                .done((layout: Model.Base.ILayout) => {
                    this.currentLayout = layout;
                    layout.render(jQuery('#boot'));
                });

            return this;
        }

        constructor(
            private anonymousLayoutResolver: Model.Base.IContainerLazyResolver,
            private authentication: Model.Base.IAuthentication,
            private authenticatedLayoutResolver: Model.Base.IContainerLazyResolver,
            private router: Model.Base.IRouter) {

            this.authentication.changedJqCallback.add(this.authenticateChangedCallback);
            this.router.start();
        }

    }

}