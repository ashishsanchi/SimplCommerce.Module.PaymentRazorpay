using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Razorpay.Api;
using SimplCommerce.Infrastructure.Data;
using SimplCommerce.Infrastructure.Helpers;
using SimplCommerce.Infrastructure.Web;
using SimplCommerce.Module.Core.Extensions;

using SimplCommerce.Module.PaymentRazorpay.Areas.PaymentRazorpay.ViewModels;
using SimplCommerce.Module.PaymentRazorpay.Models;
using SimplCommerce.Module.Payments.Models;
using SimplCommerce.Module.ShoppingCart.Services;

namespace SimplCommerce.Module.PaymentRazorpay.Areas.PaymentRazorpay.Components
{
    public class RazorpayLandingViewComponent : ViewComponent
    {
        private readonly ICartService _cartService;
        private readonly IWorkContext _workContext;
        private readonly IRepositoryWithTypedId<PaymentProvider, string> _paymentProviderRepository;


        public RazorpayLandingViewComponent(ICartService cartService, IWorkContext workContext, IRepositoryWithTypedId<PaymentProvider, string> paymentProviderRepository)
        {
            _cartService = cartService;
            _workContext = workContext;
            _paymentProviderRepository = paymentProviderRepository;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var razorpayProvider = await _paymentProviderRepository.Query().FirstOrDefaultAsync(x => x.Id == PaymentProviderHelper.RazorpayProviderId);
            var razorpaySetting = JsonConvert.DeserializeObject<RazorpayConfigForm>(razorpayProvider.AdditionalSettings);
            var currentUser = await _workContext.GetCurrentUser();
            var cart = await _cartService.GetActiveCartDetails(currentUser.Id);

            var amount = string.Format("{0:.##}", cart.OrderTotal * 100);

            // var orderId = DateTime.Today.ToString("ddMMyyyy") + "_" + cart.Id;
            // var message = "appId=" + payuSetting.AppId + "&orderId=" + orderId + "&orderAmount=" + amount + "&returnUrl=" + razorpaySetting.ReturnURL + "&paymentModes=" + payuSetting.PaymentModes;
            // var paymentToken = PaymentProviderHelper.GetToken(message, "mJrHzy5n|" + orderId + "|" + amount + "||" + currentUser.FullName + "|" + currentUser.Email + "|||||||||||MgpnDIIFmR");


            string key = razorpaySetting.AppId;// "<Enter your Api Key here>";
            string secret = razorpaySetting.SecretKey; //"<Enter your Api Secret here>";

            RazorpayClient client = new RazorpayClient(key, secret);

            Dictionary<string, object> options = new Dictionary<string, object>();
            options.Add("amount", amount); // amount in the smallest currency unit
            options.Add("receipt", "order_rcptid_11");
            options.Add("currency", "INR");
            Order order = client.Order.Create(options);


            var model = new RazorpayCheckoutForm
            {
                Key = razorpaySetting.AppId,
              //  Hash = paymentToken.ToString(),
                OrderId = order["id"].ToString(),
                OrderAmount = amount,
                CustomerName = currentUser.FullName,
                CustomerEmail = currentUser.Email,
                CustomerPhone = string.IsNullOrWhiteSpace(currentUser.PhoneNumber) ? "1234567890" : currentUser.PhoneNumber, // Phone number is mandatory for Cashfree payment
                Mode = razorpaySetting.IsSandbox ? "TEST" : "PROD",
                ReturnURL = "/default",// payuSetting.ReturnURL,
                NotifyURL = "",//payuSetting.NotifyURL
            };

            return View(this.GetViewPath(), model);
        }
    }
}
