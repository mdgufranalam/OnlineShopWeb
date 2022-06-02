using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OnlineShop.Models;
using OnlineShop.ServiceHelper.Interface;
using OnlineShop.Utility;

namespace OnlineShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class HomeController : Controller
    {
        private readonly IHttpClientHelper httpClientHelper;
        private readonly Auth0 auth;
        private readonly IAuth0ClientHelper auth0ClientHelper;
        ServiceResult<string> exception = new ServiceResult<string>();

        public HomeController(IHttpClientHelper httpClientHelper, Auth0 auth, IAuth0ClientHelper auth0ClientHelper)
        {
            this.httpClientHelper = httpClientHelper;
            this.auth = auth;
            this.auth0ClientHelper = auth0ClientHelper;
        }

        public async Task<IActionResult> Index()
        {
            if (TempData.ContainsKey("Result"))
                ViewData["Result"] = TempData["Result"];
            var apiresult = await httpClientHelper.GetAsync("Product");
            var products = JsonConvert.DeserializeObject<IEnumerable<Product>>(apiresult.Data);
            return View(products);
        }
    }
}
