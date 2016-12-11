/// <reference path='lib/require.d.ts' />
require.config({
    baseUrl: '/script',

    paths: {
        bootstrap: '//ajax.aspnetcdn.com/ajax/bootstrap/3.2.0/bootstrap',
        css: 'lib/css-0.1.2',
        d3: 'lib/d3-3.4.8',
        jquery: '//ajax.aspnetcdn.com/ajax/jQuery/jquery-1.11.1',
        ko: '//ajax.aspnetcdn.com/ajax/knockout/knockout-3.1.0.debug',
        normalize: 'lib/normalize-2.0.12',
        text: 'lib/text-2.0.12',
        underscore: 'lib/underscore-1.6.0'
    },

    shim: {
        bootstrap: { deps: ['jquery'] },
        underscore: { exports: '_' }
    }
});

require(['ko', 'bootstrap', 'jquery', 'underscore'], (ko) => {
    window['ko'] = ko;

    require(['config'], () => {
    });
});