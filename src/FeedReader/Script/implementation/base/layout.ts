/// <reference path='../../lib/underscore.d.ts' />
/// <reference path='../../model/base/layout.d.ts' />
import ViewModule = require('implementation/base/view');

export module Implementation.Base {

    export class Layout extends ViewModule.Implementation.Base.View implements Model.Base.ILayout {

        constructor(
            public viewModel: Model.Base.ILayoutViewModel,
            public authentication: Model.Base.IAuthentication,
            public router: Model.Base.IRouter) {

            super(viewModel);
            _.defaults(this.viewModel, {
                userName: ko.observable(),

                brandClicked: (): void => {
                    this.router.navigate('/');
                },
            });

        }

        logout(): JQueryPromise<void> {
            this.router.navigate('/');
            return this.authentication.logout();
        }

    }

}