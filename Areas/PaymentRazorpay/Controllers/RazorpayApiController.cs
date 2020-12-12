using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SimplCommerce.Infrastructure.Data;
using SimplCommerce.Module.PaymentRazorpay.Areas.PaymentRazorpay.ViewModels;
using SimplCommerce.Module.PaymentRazorpay.Models;
using SimplCommerce.Module.Payments.Models;

namespace SimplCommerce.Module.PaymentRazorpay.Areas.PaymentRazorpay.Controllers
{
    [Authorize(Roles = "admin")]
    [Area("PaymentRazorpay")]
    [Route("api/razorpay")]
    public class RazorpayApiController : Controller
    {
        private readonly IRepositoryWithTypedId<PaymentProvider, string> _paymentProviderRepository;

        public RazorpayApiController(IRepositoryWithTypedId<PaymentProvider, string> paymentProviderRepository)
        {
            _paymentProviderRepository = paymentProviderRepository;
        }

        [HttpGet("config")]
        public async Task<IActionResult> Config()
        {
            var razorpayProvider = await _paymentProviderRepository.Query().FirstOrDefaultAsync(x => x.Id == PaymentProviderHelper.RazorpayProviderId);
            if (string.IsNullOrEmpty(razorpayProvider.AdditionalSettings))
            {
                return Ok(new RazorpayConfigForm());
            }

            var model = JsonConvert.DeserializeObject<RazorpayConfigForm>(razorpayProvider.AdditionalSettings);
            return Ok(model);
        }

        [HttpPut("config")]
        public async Task<IActionResult> Config([FromBody] RazorpayConfigForm model)
        {
            if (ModelState.IsValid)
            {
                var codProvider = await _paymentProviderRepository.Query().FirstOrDefaultAsync(x => x.Id == PaymentProviderHelper.RazorpayProviderId);
                codProvider.AdditionalSettings = JsonConvert.SerializeObject(model);
                await _paymentProviderRepository.SaveChangesAsync();
                return Accepted();
            }

            return BadRequest(ModelState);
        }
    }
}
