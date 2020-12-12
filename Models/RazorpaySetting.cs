namespace SimplCommerce.Module.PaymentRazorpay.Models
{
    public class RazorpaySetting
    {
        public decimal? MinOrderValue { get; set; }

        public decimal? MaxOrderValue { get; set; }

        public decimal PaymentFee { get; set; }
    }
}
