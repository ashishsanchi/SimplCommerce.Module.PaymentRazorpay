/*global angular, jQuery*/
(function ($) {
    angular
        .module('simplAdmin.paymentRazorpay')
        .controller('RazorpayConfigFormCtrl', ['paymentRazorpayService', 'translateService', RazorpayConfigFormCtrl]);

    function RazorpayConfigFormCtrl(paymentRazorpayService, translateService) {
        var vm = this;
        vm.translate = translateService;
        vm.razorpayConfig = {};

        vm.save = function save() {
            vm.validationErrors = [];
            paymentRazorpayService.updateSetting(vm.razorpayConfig)
                .then(function (result) {
                    toastr.success('Application settings have been saved');
                })
                .catch(function (response) {
                    var error = response.data;
                    vm.validationErrors = [];
                    if (error && angular.isObject(error)) {
                        for (var key in error) {
                            vm.validationErrors.push(error[key][0]);
                        }
                    } else {
                        vm.validationErrors.push('Could not save Cashfree settings.');
                    }
                });
        };

        function init() {
            paymentRazorpayService.getSettings().then(function (result) {
                vm.razorpayConfig = result.data;
            });
        }

        init();
    }
})(jQuery);
