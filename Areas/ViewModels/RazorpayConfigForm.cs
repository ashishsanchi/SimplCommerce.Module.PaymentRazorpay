using System;
using System.ComponentModel.DataAnnotations;

namespace SimplCommerce.Module.PaymentRazorpay.Areas.PaymentRazorpay.ViewModels
{
    public class RazorpayConfigForm
    {
        public bool IsSandbox { get; set; }
        public string AppId { get; set; }
        public string SecretKey { get; set; }
        public string apiUrl { get; set; }
        public string ReturnURL { get; set; }
        public string NotifyURL { get; set; }
        public string PaymentModes { get; set; }
    }
}
