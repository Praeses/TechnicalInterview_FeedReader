/// <reference path='../../model/base/view.d.ts' />
export module Implementation.Base {

    export class View implements Model.Base.IView {

        constructor(public viewModel: Model.Base.IViewModel) {
            _.defaults(this.viewModel, { el: undefined });
        }

        render(el: JQuery, html?: HTMLElement): Model.Base.IView;
        render(el: JQuery, html?: string): Model.Base.IView;
        render(el: JQuery, html?: JQuery): Model.Base.IView;
        render(el: JQuery, html?: any): Model.Base.IView {
            this.viewModel.el = el;

            el.empty();
            if (html) {
                el.append(html);
                ko.cleanNode(el[0]);
                ko.applyBindings(this.viewModel, el[0]);

                setTimeout(() => el.find('.autofocus:visible:first').focus(), 200);
                el.find('.modal').on('shown.bs.modal', function() {
                    setTimeout(() => jQuery(this).find('.autofocus:visible:first').focus(), 200);
                });
            }

            return this;
        }

    }

}