/// <reference path='../../../model/base/anonymousLayout.d.ts' />
/// <amd-dependency path='text!implementation/layout/default/anonymousLayout.html' />
import LayoutModule = require('implementation/base/layout');

export module Implementation.Layout.Default {

    export class AnonymousLayout extends LayoutModule.Implementation.Base.Layout implements Model.Base.IAnonymousLayout {

        private authenticationChangedCallback: Model.Base.IAuthenticationChangedCallback = (): void => {
            this.viewModel.userName(this.authentication.session.userName);
            this.userNameChanged();
            jQuery('#anonymousLayout-signupModal').modal('hide');
            jQuery('.modal-backdrop').remove();
        };

        private userNameChanged(): void {
            if (!this.viewModel.userName()) {
                this.viewModel.userNameExists(undefined);
                this.viewModel.passwordIncorrect(undefined);
                return;
            }

            this.viewModel.passwordIncorrect(undefined);
            this.authentication.userNameExists(this.viewModel.userName())
                .done(() => this.viewModel.userNameExists(true))
                .fail(() => this.viewModel.userNameExists(false));
            return;
        }

        constructor(
            public viewModel: Model.Base.IAnonymousLayoutViewModel,
            public authentication: Model.Base.IAuthentication,
            public router: Model.Base.IRouter) {
            super(viewModel, authentication, router);

            _.defaults(this.viewModel, {
                password: ko.observable(),
                passwordIncorrect: ko.observable(),
                rememberMe: ko.observable(true),
                userName: ko.observable(),
                userNameExists: ko.observable(),

                loginClicked: (): void => {
                    if (!this.viewModel.userName()) {
                        this.viewModel.userNameExists(false);
                        return;
                    }

                    if (!this.viewModel.password()) {
                        this.viewModel.passwordIncorrect(true);
                        return;
                    }

                    this.authentication.login(
                            this.viewModel.userName(),
                            this.viewModel.password(),
                            this.viewModel.rememberMe())
                        .fail(() => {
                            this.viewModel.passwordIncorrect(true);
                        });
                },

                passwordInputEvent: (): void => {
                    this.viewModel.passwordIncorrect(false);
                },

                registerClicked: (): void => {
                    if (!this.viewModel.userName()) {
                        this.viewModel.userNameExists(true);
                        return;
                    }

                    if (!this.viewModel.password()) {
                        this.viewModel.passwordIncorrect(true);
                        return;
                    }

                    this.authentication.registerUser(
                            this.viewModel.userName(),
                            this.viewModel.password(),
                            this.viewModel.rememberMe())
                        .fail(() => {
                            this.viewModel.passwordIncorrect(true);
                        });
                },

                userNameChangedEvent: (): void => {
                    this.userNameChanged();
                }
            });

            this.viewModel.rememberMe(this.authentication.session.rememberMe);
            this.viewModel.userName(this.authentication.session.userName);
            this.authentication.changedJqCallback.add(this.authenticationChangedCallback);
            this.userNameChanged();
        }

        render(el: JQuery, html?: HTMLElement): Model.Base.IView;
        render(el: JQuery, html?: string): Model.Base.IView;
        render(el: JQuery, html?: JQuery): Model.Base.IView;
        render(el: JQuery, html?: any): Model.Base.IView {
            jQuery('body').css('overflow-x', 'hidden').css('overflow-y', 'scroll').css('padding-top', '60px');
            super.render(el, require('text!implementation/layout/default/anonymousLayout.html'));

            el.find('.dropdown').on('shown.bs.dropdown', function() {
                setTimeout(() => jQuery(this).find('.autofocus:visible:first').focus(), 200);
            });

            return this;
        }

    }

}