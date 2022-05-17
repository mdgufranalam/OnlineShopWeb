using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using OnlineShop.Models;
using OnlineShop.ServiceHelper.Interface;

namespace OnlineShop.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class ProductController : Controller
    {
        private readonly IHttpClientHelper httpClientHelper;
        private ServiceResult<string> exception;

        public ProductController(IHttpClientHelper httpClientHelper)
        {
            this.httpClientHelper = httpClientHelper;
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
                var products = JsonConvert.DeserializeObject<IEnumerable<Product>>(apiresult.Data);
                return View(products);
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
        public async Task<ActionResult> Details(int id)
        {
            try
            {
                var apiresult = await httpClientHelper.GetAsync("Product/" + id);
                var products = JsonConvert.DeserializeObject<Product>(apiresult.Data);
                if (products != null)
                {
                    IEnumerable<Category> categories = null;
                    var categoriesApiResult = await httpClientHelper.GetAsync("Category");
                    if (categoriesApiResult.Success)
                    {
                        var categorySelectList = JsonConvert.DeserializeObject<List<Category>>(categoriesApiResult.Data).Select(c => new { c.Id, c.Name }).Distinct().OrderBy(c => c.Name).ToList();
                        //categorySelectList.Insert(0, new { Id = 0, Name = "--Select Category--" });
                        ViewBag.Categories = categorySelectList;
                    }
                    return View(products);
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
