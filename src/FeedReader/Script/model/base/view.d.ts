/// <reference path='../../lib/jquery.d.ts' />
declare module Model.Base {

    interface IView {
        viewModel: IViewModel;

        render(el: JQuery, html?: HTMLElement): IView;
        render(el: JQuery, html?: string): IView;
        render(el: JQuery, html?: JQuery): IView;
    }

    interface IViewModel {
        el: JQuery;
    }

}