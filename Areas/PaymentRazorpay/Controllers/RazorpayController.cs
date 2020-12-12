using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SimplCommerce.Infrastructure.Data;
using SimplCommerce.Module.Core.Extensions;
using SimplCommerce.Module.Orders.Services;
using SimplCommerce.Module.PaymentRazorpay.Models;
using SimplCommerce.Module.Payments.Models;
using SimplCommerce.Module.ShoppingCart.Areas.ShoppingCart.ViewModels;
using SimplCommerce.Module.ShoppingCart.Services;

namespace SimplCommerce.Module.PaymentRazorpay.Areas.PaymentRazorpay.Controllers
{
    [Authorize]
    [Area("PaymentRazorpay")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class RazorpayController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IWorkContext _workContext;
        private readonly ICartService _cartService;
        private readonly IRepositoryWithTypedId<PaymentProvider, string> _paymentProviderRepository;
        private Lazy<RazorpaySetting> _setting;

        public RazorpayController(
            ICartService cartService,
            IOrderService orderService,
            IRepositoryWithTypedId<PaymentProvider, string> paymentProviderRepository,
            IWorkContext workContext)
        {
            _paymentProviderRepository = paymentProviderRepository;
            _cartService = cartService;
            _orderService = orderService;
            _workContext = workContext;
            _setting = new Lazy<RazorpaySetting>(GetSetting());
        }

        public async Task<IActionResult> RazorpayCheckout()
        {
            var currentUser = await _workContext.GetCurrentUser();
            var cart = await _cartService.GetActiveCartDetails(currentUser.Id);
            if(cart == null)
            {
                return NotFound();
            }

            if (!ValidateRazorpay(cart))
            {
                TempData["Error"] = "Payment Method is not eligible for this order.";
                return Redirect("~/checkout/payment");
            }

            var calculatedFee = CalculateFee(cart);           
            var orderCreateResult = await _orderService.CreateOrder(cart.Id, "Razorpay", calculatedFee);

            if (!orderCreateResult.Success)
            {
                TempData["Error"] = orderCreateResult.Error;
                return Redirect("~/checkout/payment");
            }

            return Redirect($"~/checkout/success?orderId={orderCreateResult.Value.Id}");
        }

        private RazorpaySetting GetSetting()
        {
            var razorpayProvider = _paymentProviderRepository.Query().FirstOrDefault(x => x.Id == PaymentProviderHelper.RazorpayProviderId);
            if (string.IsNullOrEmpty(razorpayProvider.AdditionalSettings))
            {
                return new RazorpaySetting();
            }

            var coDSetting = JsonConvert.DeserializeObject<RazorpaySetting>(razorpayProvider.AdditionalSettings);
            return coDSetting;
        }

        private bool ValidateRazorpay(CartVm cart)
        {
            if (_setting.Value.MinOrderValue.HasValue && _setting.Value.MinOrderValue.Value > cart.OrderTotal)
            {
                return false;
            }

            if (_setting.Value.MaxOrderValue.HasValue && _setting.Value.MaxOrderValue.Value < cart.OrderTotal)
            {
                return false;
            }

            return true;
        }

        private decimal CalculateFee(CartVm cart)
        {
            var percent = _setting.Value.PaymentFee;
            return (cart.OrderTotal / 100) * percent;
        }
    }
}
