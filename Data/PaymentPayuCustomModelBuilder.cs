using Microsoft.EntityFrameworkCore;
using SimplCommerce.Infrastructure.Data;
using SimplCommerce.Module.Payments.Models;

namespace SimplCommerce.Module.PaymentPayu.Data
{
    public class PaymentCashfreeCustomModelBuilder : ICustomModelBuilder
    {
        public void Build(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PaymentProvider>().HasData(
                new PaymentProvider("Razorpay") { Name = "Razorpay Payment Gateway", LandingViewComponentName = "RazorpayLanding", ConfigureUrl = "payments-razorpay-config", IsEnabled = true, AdditionalSettings = "{ \"IsSandbox\":true, \"AppId\":\"358035b02486f36ca27904540853\", \"SecretKey\":\"26f48dcd6a27f89f59f28e65849e587916dd57b9\" }" }
            );
        }
    }
}
