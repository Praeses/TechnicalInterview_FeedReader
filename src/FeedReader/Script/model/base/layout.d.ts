/// <reference path='view.d.ts' />
declare module Model.Base {

    interface ILayout extends IView {
        authentication: IAuthentication;
        router: IRouter;
        viewModel: ILayoutViewModel;

        logout(): JQueryPromise<void>;
    }

    interface ILayoutViewModel extends IViewModel {
        userName: KnockoutObservable<string>;
    }

}