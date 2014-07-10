/// <reference path='../../../model/base/view.d.ts' />
/// <amd-dependency path='text!implementation/view/default/anonymousView.html' />
import ViewModule = require('implementation/base/view');

export module Implementation.View.Default {

    export class AnonymousView extends ViewModule.Implementation.Base.View {

        private authenticationChangedCallback: Model.Api.IAuthenticationApiChangedCallback = (): void => {
            this.viewModel.userName(this.authenticationApi.session.userName);
            this.userNameChanged();
            jQuery('#anonymousView-signupModal').modal('hide');
            jQuery('.modal-backdrop').remove();
        };

        private userNameChanged(): void {
            if (!this.viewModel.userName()) {
                this.viewModel.userNameExists(undefined);
                this.viewModel.passwordIncorrect(undefined);
                return;
            }

            this.viewModel.passwordIncorrect(undefined);
            this.registrationApi.userNameExists(this.viewModel.userName())
                .done(() => this.viewModel.userNameExists(true))
                .fail(() => this.viewModel.userNameExists(false));
            return;
        }

        constructor(
            public viewModel: IAnonymousViewModel,
            public authenticationApi: Model.Api.IAuthenticationApi,
            public registrationApi: Model.Api.IRegistrationApi) {
            super(viewModel);

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

                    this.authenticationApi.login(
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

                    this.registrationApi.register(
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

            this.viewModel.rememberMe(this.authenticationApi.session.rememberMe);
            this.viewModel.userName(this.authenticationApi.session.userName);
            this.authenticationApi.changedJqCallback.add(this.authenticationChangedCallback);
            this.userNameChanged();
        }

        render(el: JQuery, html?: HTMLElement): Model.Base.IView;
        render(el: JQuery, html?: string): Model.Base.IView;
        render(el: JQuery, html?: JQuery): Model.Base.IView;
        render(el: JQuery, html?: any): Model.Base.IView {
            jQuery('body').css('overflow-x', 'hidden').css('overflow-y', 'scroll').css('padding-top', '60px');
            super.render(el, require('text!implementation/view/default/anonymousView.html'));

            el.find('.dropdown').on('shown.bs.dropdown', function() {
                setTimeout(() => jQuery(this).find('.autofocus:visible:first').focus(), 200);
            });

            return this;
        }

    }

    export interface IAnonymousViewModel extends Model.Base.IViewModel {
        password: KnockoutObservable<string>;
        passwordIncorrect: KnockoutObservable<boolean>;
        rememberMe: KnockoutObservable<boolean>;
        userName: KnockoutObservable<string>;
        userNameExists: KnockoutObservable<boolean>;
    }

}