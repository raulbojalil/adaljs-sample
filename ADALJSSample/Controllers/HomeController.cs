using Microsoft.AspNetCore.Authentication.AzureAD.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace ADALJSSample.Controllers
{
    public class HomeController : Controller
    {
        private readonly IOptions<AzureADOptions> _options;

        public HomeController(IOptions<AzureADOptions> options)
        {
            _options = options;
        }

        public IActionResult Index()
        {
            ViewBag.ClientId = _options.Value.ClientId;
            return View();
        }
    }
}
