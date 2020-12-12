/*global angular*/
(function () {
    'use strict';

    angular
        .module('simplAdmin.paymentRazorpay', [])
        .config(['$stateProvider',
            function ($stateProvider) {
                $stateProvider
                    .state('payments-razorpay-config', {
                        url: '/payments/razorpay/config',
                        templateUrl: '_content/SimplCommerce.Module.PaymentRazorpay/admin/razorpay/razorpay-config-form.html',
                        controller: 'RazorpayConfigFormCtrl as vm'
                    })
                    ;
            }
        ]);
})();
