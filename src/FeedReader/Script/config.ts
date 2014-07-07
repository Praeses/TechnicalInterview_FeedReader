/// <amd-dependency path='implementation/binding/iso6801' />
import IocModule = require('implementation/base/ioc');
import BaseConfigModule = require('implementation/base/config');
import LayoutConfigModule = require('implementation/layout/default/config');

export module Configure {
    var container = new IocModule.Implementation.Base.Container();

    BaseConfigModule.Implementation.Base.config(container);
    LayoutConfigModule.Implementation.Layout.Default.config(container);

    container.resolveNamed('Model.Base.ILayoutController', 'Default');
}