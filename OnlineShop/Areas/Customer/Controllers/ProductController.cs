using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OnlineShop.Models;
using OnlineShop.ServiceHelper.Interface;

namespace OnlineShop.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class ProductController : Controller
    {
        private readonly IHttpClientHelper httpClientHelper;

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
                var products = JsonConvert.DeserializeObject<IEnumerable<Product>>(apiresult.Data);
                return View(products);
            }
            catch (Exception Ex)
            {
                return Json(Ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Index(string searchstring)
        {
            try
            {
                var apiresult = await httpClientHelper.GetAsync("Product/SearchProducts/"+searchstring);
                var products = JsonConvert.DeserializeObject<IEnumerable<Product>>(apiresult.Data);
                return View(products);
            }
            catch (Exception Ex)
            {
                return Json(Ex.Message);
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

                return Json(ex.Message);
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
                products=new List<Product>();
                return View("Index", products);
            }
            catch (Exception Ex)
            {
                return Json(Ex.Message);
            }
        }
        // GET: ProductController/Details/5
    }
}
