using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using OnlineShop.Models;
using OnlineShop.ServiceHelper.Interface;

namespace OnlineShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IHttpClientHelper httpClientHelper;

        public ProductController(IHttpClientHelper httpClientHelper)
        {
            this.httpClientHelper = httpClientHelper;
        }

        // GET: ProductController
        public async Task<ActionResult> Index()
        {
            try
            {
                var apiresult = await httpClientHelper.GetAsync("Product");
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
                var apiresult = await httpClientHelper.GetAsync("Product/"+id);
                var products = JsonConvert.DeserializeObject<Product>(apiresult.Data);
                if(products!=null)
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

        // GET: ProductController/Create
        public async Task<ActionResult> Create()
        {
            IEnumerable<Category> categories = null;
            var categoriesApiResult = await httpClientHelper.GetAsync("Category");
            if(categoriesApiResult.Success)
            {
                var categorySelectList= JsonConvert.DeserializeObject<List<Category>>(categoriesApiResult.Data).Select(c => new { c.Id,c.Name}).Distinct().OrderBy(c=>c.Name).ToList();
                categorySelectList.Insert(0, new { Id = 0, Name = "--Select Category--" });
                ViewBag.Categories = categorySelectList;
            }           

            return View();
        }

        // POST: ProductController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Product product, IFormFile? file)
        {
            try
            {
                if(ModelState.IsValid && file!=null)
                {
                    var uploadResult=await httpClientHelper.PostMultipartAsync("Product/UploadFile", JsonConvert.SerializeObject(product), file);
                    uploadResult.Success = true;
                    if(uploadResult.Success)
                    {
                        product.ImageUrl= "imageurl";
                        var result = await httpClientHelper.PostAsync("Product", JsonConvert.SerializeObject(product));
                        
                        ViewBag.Result = result;
                       
                    }
                    else
                    {
                        ViewBag.Result = uploadResult;
                    }

                    ViewBag.Result = uploadResult;
                }
                return RedirectToAction(nameof(Index));
            }
            catch(Exception ex)
            {
                return Json(ex.Message);
            }
        }

        // GET: ProductController/Edit/5
        public async Task<ActionResult> Edit(int id)
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

        // POST: ProductController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async  Task<ActionResult> Edit(int id, Product product, IFormFile? file)
        {
            try
            {
                var apiresult=await httpClientHelper.PutAsync("Product/" + id, JsonConvert.SerializeObject(product));
                ViewBag.Result = apiresult;
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ProductController/Delete/5
        public async Task<ActionResult> Delete(int id)
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
                    categorySelectList.Insert(0, new { Id = 0, Name = "--Select Category--" });
                    ViewBag.Categories = categorySelectList;
                }
                return View(products);
            }
            return NotFound();

        }

        // POST: ProductController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id, Product product)
        {
            try
            {
                var apiresult = await httpClientHelper.DeleteAsync("Product/" + id);
                ViewBag.Result = apiresult;
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
