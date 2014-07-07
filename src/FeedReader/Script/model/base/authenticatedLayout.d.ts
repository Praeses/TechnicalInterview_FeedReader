declare module Model.Base {

    interface IAuthenticatedLayout extends ILayout {
        viewModel: ILayoutViewModel;
    }

    interface IAuthenticatedLayoutViewModel extends ILayoutViewModel {
    }

}