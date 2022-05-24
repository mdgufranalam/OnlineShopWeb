using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using OnlineShop.Models;
using OnlineShop.ServiceHelper.Interface;
using OnlineShop.Utility;
using System.Security.Claims;

namespace OnlineShop.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly IHttpClientHelper httpClientHelper;
        private ServiceResult<string> exception;
        private readonly Auth0 auth;
        private readonly IAuth0ClientHelper auth0ClientHelper;
        private readonly ApplicationDbContext _db;


        public HomeController(IHttpClientHelper httpClientHelper, Auth0 auth, IAuth0ClientHelper auth0ClientHelper, ApplicationDbContext db)
        {
            this.httpClientHelper = httpClientHelper;
            this.auth = auth;
            this.auth0ClientHelper = auth0ClientHelper;
            if (auth.expires_out < auth.current_time)
            {
                var authres = auth0ClientHelper.GetTokenAsync(this.auth).GetAwaiter().GetResult();
                if (authres.Success)
                {
                    this.auth = authres.Data;
                }
            }
            httpClientHelper.auth = auth;
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                if (TempData.ContainsKey("Result"))
                    ViewData["Result"] = TempData["Result"];
                var apiresult = await httpClientHelper.GetAsync("Product");
                if (apiresult.Success)
                {
                    var products = JsonConvert.DeserializeObject<IEnumerable<Product>>(apiresult.Data);
                    return View(products);
                }
                else
                {
                    ViewBag.Result = JsonConvert.SerializeObject(apiresult);
                    return Json(apiresult);
                }
            }
            catch (Exception ex)
            {
                exception = new ServiceResult<string>();
                exception.Success = false;
                exception.Message = ex.Message;
                TempData["Result"] = JsonConvert.SerializeObject(exception);
                return Json(exception);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Index(string searchstring)
        {
            try
            {
                var apiresult = await httpClientHelper.GetAsync("Product/SearchProducts/" + searchstring);
                if(apiresult.Success)
                {
                    var products = JsonConvert.DeserializeObject<IEnumerable<Product>>(apiresult.Data);
                    return View(products);
                }
                else
                {
                    exception = new ServiceResult<string>();
                    exception.Success = false;
                    exception.Message =$"{searchstring} Not Found.";
                    ViewBag.Result = JsonConvert.SerializeObject(exception);
                    return View(new List<Product>() { });
                }
                
            }
            catch (Exception ex)
            {
                exception = new ServiceResult<string>();
                exception.Success = false;
                exception.Message = ex.Message;
                TempData["Result"] = JsonConvert.SerializeObject(exception);
                return RedirectToAction(nameof(Index));
            }
        }
        // GET: ProductController/Details/5
        public async Task<ActionResult> Details(int ProductId)
        {
            try
            {
                ShoppingCart shoppingCart =new ShoppingCart();
                var apiresult = await httpClientHelper.GetAsync("Product/" + ProductId);
                var products = JsonConvert.DeserializeObject<Product>(apiresult.Data);
                if (products != null)
                {
                    //IEnumerable<Category> categories = null;
                    //var categoriesApiResult = await httpClientHelper.GetAsync("Category");
                    //if (categoriesApiResult.Success)
                    //{
                    //    var categorySelectList = JsonConvert.DeserializeObject<List<Category>>(categoriesApiResult.Data).Select(c => new { c.Id, c.Name }).Distinct().OrderBy(c => c.Name).ToList();
                    //    //categorySelectList.Insert(0, new { Id = 0, Name = "--Select Category--" });
                    //    ViewBag.Categories = categorySelectList;
                    //}
                    shoppingCart = new()
                    {
                        Count = 1,
                        ProductId = ProductId,
                        Products = products
                    };
                    return View(shoppingCart);
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                exception = new ServiceResult<string>();
                exception.Success = false;
                exception.Message = ex.Message;
                TempData["Result"] = JsonConvert.SerializeObject(exception);
                return RedirectToAction(nameof(Index));
            }



        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<ActionResult> Details(ShoppingCart shoppingCart,string? mode=null)
        {
            try
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                shoppingCart.ApplicationUserId = claim.Value;

                var apiresult = await httpClientHelper.PostAsync("ShoppingCart", JsonConvert.SerializeObject(shoppingCart));
                if (apiresult.Success)
                {
                    if (!String.IsNullOrEmpty(apiresult.Data))
                    {
                        HttpContext.Session.SetInt32(SD.SessionCart, Convert.ToInt32(apiresult.Data));
                    }
                    if(mode=="buy")
                    {
                        return RedirectToAction("Index","Cart");
                    }
                    return RedirectToAction(nameof(Index));
                }
                return NotFound();

            }
            catch (Exception ex)
            {
                exception = new ServiceResult<string>();
                exception.Success = false;
                exception.Message = ex.Message;
                TempData["Result"] = JsonConvert.SerializeObject(exception);
                return RedirectToAction(nameof(Index));
            }



        }

        [HttpGet("[action]/{category}")]
        public async Task<IActionResult> CategoryWiseProduct(string category)
        {
            try
            {
                if(category.ToLower() == "all")
                {
                    return RedirectToAction(nameof(Index));
                }
                IEnumerable<Product> products;

                var apiresult = await httpClientHelper.GetAsync("Product/SearchProducts/" + category);
                if (apiresult.Success)
                {
                    products = JsonConvert.DeserializeObject<IEnumerable<Product>>(apiresult.Data);
                    return View("Index", products);
                }
                products = new List<Product>();
                return View("Index", products);
            }
            catch (Exception ex)
            {
                exception = new ServiceResult<string>();
                exception.Success = false;
                exception.Message = ex.Message;
                TempData["Result"] = JsonConvert.SerializeObject(exception);
                return RedirectToAction(nameof(Index));
            }
        }
        [HttpGet("[action]")]
        public async Task<IActionResult> SortProduct(string? criteria, string? sort)
        {
            try
            {
                var sortcriteria = criteria ?? "lastupdatedate";
                var sortwise = sort ?? "desc";
                var query = new Dictionary<string, string>()
                {
                    ["criteria"] = sortcriteria,
                    ["sort"] = sortwise
                };
                var querystringurl=QueryHelpers.AddQueryString("Product/SortProduct", query);
                var apiresult = await httpClientHelper.GetAsync(querystringurl);
                if (apiresult.Success)
                {
                    var products = JsonConvert.DeserializeObject<IEnumerable<Product>>(apiresult.Data);
                    return View("Index", products);
                }
                else
                {
                    TempData["Result"] = JsonConvert.SerializeObject(apiresult);
                }
                return View("Index", new List<Product>());
            }
            catch (Exception ex)
            {
                exception = new ServiceResult<string>();
                exception.Success = false;
                exception.Message = ex.Message;
                TempData["Result"] = JsonConvert.SerializeObject(exception);
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
