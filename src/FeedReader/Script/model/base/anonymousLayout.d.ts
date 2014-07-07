/// <reference path='../../lib/knockout.d.ts' />
declare module Model.Base {

    interface IAnonymousLayout extends ILayout {
        viewModel: ILayoutViewModel;
    }

    interface IAnonymousLayoutViewModel extends ILayoutViewModel {
        password: KnockoutObservable<string>;
        passwordIncorrect: KnockoutObservable<boolean>;
        rememberMe: KnockoutObservable<boolean>;
        userName: KnockoutObservable<string>;
        userNameExists: KnockoutObservable<boolean>;
    }

}