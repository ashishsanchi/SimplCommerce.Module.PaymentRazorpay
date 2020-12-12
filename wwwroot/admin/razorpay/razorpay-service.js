/*global angular*/
(function () {
    angular
        .module('simplAdmin.paymentRazorpay')
        .factory('paymentRazorpayService', ['$http', paymentRazorpayService]);

    function paymentRazorpayService($http) {
        var service = {
            getSettings: getSettings,
            updateSetting: updateSetting
        };
        return service;

        function getSettings() {
            return $http.get('api/razorpay/config');
        }

        function updateSetting(settings) {
            return $http.put('api/razorpay/config', settings);
        }
    }
})();
