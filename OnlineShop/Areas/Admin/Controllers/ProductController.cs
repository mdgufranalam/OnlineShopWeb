using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OnlineShop.Models;
using OnlineShop.ServiceHelper.Interface;

namespace OnlineShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IHttpClientHelper httpClientHelper;
        ServiceResult<string> exception = new ServiceResult<string>();
        public ProductController(IHttpClientHelper httpClientHelper)
        {
            this.httpClientHelper = httpClientHelper;
        }

        // GET: ProductController
        public async Task<ActionResult> Index()
        {
            try
            {
                if(TempData.ContainsKey("Result"))
                   ViewData["Result"] = TempData["Result"];
                var apiresult = await httpClientHelper.GetAsync("Product");
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
                var apiresult = await httpClientHelper.GetAsync("Product/"+id);
                var products = JsonConvert.DeserializeObject<Product>(apiresult.Data);
                if(products!=null)
                {                
                //IEnumerable<Category> categories = null;
                //var categoriesApiResult = await httpClientHelper.GetAsync("Category");
                //if (categoriesApiResult.Success)
                //{
                //    var categorySelectList = JsonConvert.DeserializeObject<List<Category>>(categoriesApiResult.Data).Select(c => new { c.Id, c.Name }).Distinct().OrderBy(c => c.Name).ToList();
                //        //categorySelectList.Insert(0, new { Id = 0, Name = "--Select Category--" });
                //    ViewBag.Categories = categorySelectList;
                //}
                return View(products);
                }
                ViewData["Result"] = JsonConvert.SerializeObject(apiresult);
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
                    if(uploadResult.Success)
                    {                       
                        ImgurUpload resultdata = JsonConvert.DeserializeObject<ImgurUpload>(uploadResult.Data);
                        product.ImageUrl= resultdata.data.link;
                        var result = await httpClientHelper.PostAsync("Product", JsonConvert.SerializeObject(product));
                        ViewData["Result"] = JsonConvert.SerializeObject(result); ;
                    }
                    else
                    {
                        ViewData["Result"] = JsonConvert.SerializeObject(uploadResult); ;
                    }

                    ViewData["Result"] = uploadResult;
                }
                return RedirectToAction(nameof(Index));
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
                if (file == null && Request.Form.Files.Count!=0)
                    file= Request.Form.Files[0];
                if (file != null || Request.Form.Files.Count !=0)
                {
                   
                    var uploadResult = await httpClientHelper.PostMultipartAsync("Product/UploadFile", JsonConvert.SerializeObject(product), file);
                    if(uploadResult.Success)
                    {
                        ImgurUpload resultdata = JsonConvert.DeserializeObject<ImgurUpload>(uploadResult.Data);
                        product.ImageUrl = resultdata.data.link;
                       
                    }
                    else
                    {
                        TempData["Result"] = JsonConvert.SerializeObject(uploadResult);
                        return RedirectToAction(nameof(Index));
                    }
                   
                }
                var apiresult=await httpClientHelper.PutAsync("Product/" + id, JsonConvert.SerializeObject(product));
                TempData["Result"] = JsonConvert.SerializeObject(apiresult); 
                return RedirectToAction(nameof(Index));
            }
            catch(Exception ex)
            {
                exception = new ServiceResult<string>();
                exception.Success = false;
                exception.Message = ex.Message;
                TempData["Result"] = JsonConvert.SerializeObject(exception);
                return RedirectToAction(nameof(Index));
                
            }
        }

        // GET: ProductController/Delete/5
        public async Task<ActionResult> Delete(int id)
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
                        categorySelectList.Insert(0, new { Id = 0, Name = "--Select Category--" });
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

        // POST: ProductController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id, Product product)
        {
            try
            {
                var apiresult = await httpClientHelper.DeleteAsync("Product/" + id);
                TempData["Result"] = JsonConvert.SerializeObject(apiresult); ;
                return RedirectToAction(nameof(Index));
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
