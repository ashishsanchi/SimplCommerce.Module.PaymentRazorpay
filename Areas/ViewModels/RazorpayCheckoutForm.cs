namespace SimplCommerce.Module.PaymentRazorpay.Areas.PaymentRazorpay.ViewModels
{
    public class RazorpayCheckoutForm
    {
        public string Key { get; set; }

        public string Hash { get; set; }

        public string Mode { get; set; }

        public string OrderAmount { get; set; }

        public string OrderId { get; set; }

        public string CustomerName { get; set; }

        public string CustomerPhone { get; set; }

        public string CustomerEmail { get; set; }

        public string ReturnURL { get; set; }

        public string NotifyURL { get; set; }
    }
}
