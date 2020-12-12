using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SimplCommerce.Infrastructure.Data;
using SimplCommerce.Module.Core.Extensions;
using SimplCommerce.Module.Orders.Areas.Orders.ViewModels;
using SimplCommerce.Module.Orders.Services;
using SimplCommerce.Module.PaymentRazorpay.Models;
using SimplCommerce.Module.Payments.Models;
using SimplCommerce.Module.ShoppingCart.Areas.ShoppingCart.ViewModels;
using SimplCommerce.Module.ShoppingCart.Models;
using SimplCommerce.Module.ShoppingCart.Services;

namespace SimplCommerce.Module.PaymentRazorpay.Areas.PaymentRazorpay.Controllers
{
    [Area("Catalog")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
   // [Route("api/razorpay")]
    public class MobileRazorpayApiController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IWorkContext _workContext;
        private readonly ICartService _cartService;
        private readonly IRepositoryWithTypedId<PaymentProvider, string> _paymentProviderRepository;
        private readonly IRepository<Cart> _cartRepository;
        private Lazy<RazorpaySetting> _setting;
        public MobileRazorpayApiController(ICartService cartService,
            IOrderService orderService,
            IRepositoryWithTypedId<PaymentProvider, string> paymentProviderRepository, IRepository<Cart> cartRepository,
            IWorkContext workContext)
        {
            _paymentProviderRepository = paymentProviderRepository;
            _cartService = cartService;
            _orderService = orderService;
            _workContext = workContext;
            _setting = new Lazy<RazorpaySetting>(GetSetting());
            _cartRepository = cartRepository;

        }

        [HttpPost("api/mobile/razorSuccess/{userId}")]
        public async Task<JsonResult> RazorpayCheckout(long userId, DeliveryInformationVm model)
        {
            //var currentUser = await _workContext.GetCurrentUser();
            var cart = await _cartService.GetActiveCart(userId);
            
            if (cart == null)
            {
               // return NotFound();
            }

            if (cart.CreatedById != userId)
            {
               // return Forbid();
            }

            cart.ShippingData = JsonConvert.SerializeObject(model);
            cart.OrderNote = model.OrderNote;
            _cartRepository.SaveChanges();

            //  var calculatedFee = CalculateFee(cart);
            // var orderCreateResult = await _orderService.CreateOrder(cart.Id, "Razorpay", calculatedFee);
            var orderCreateResult = await _orderService.CreateOrder(cart.Id, "Razorpay", 0);

            //if (!orderCreateResult.Success)
            //{
            //    TempData["Error"] = orderCreateResult.Error;
            //    return Redirect("~/checkout/payment");
            //}
            orderCreateResult.Value = null;
            return Json(new { success= orderCreateResult.Success, error= orderCreateResult.Error });
           
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
