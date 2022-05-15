using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OnlineShop.Models;
using OnlineShop.ServiceHelper.Interface;

namespace OnlineShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly IHttpClientHelper httpClientHelper;

        public CategoryController(IHttpClientHelper httpClientHelper)
        {
            this.httpClientHelper = httpClientHelper;
        }


        // GET: CategoryController
        public async Task<ActionResult> Index()
        {
            
            try
            {
                var apiresult=await httpClientHelper.GetAsync("Category");
                IEnumerable<Category> categories = JsonConvert.DeserializeObject<IEnumerable<Category>>(apiresult.Data);
                return View(categories);
            }
            catch (Exception)
            {
                //ToDO return content of Json
                throw;
            }
           
        }

        // GET: CategoryController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            try
            {
                var apiresult = await httpClientHelper.GetAsync("Category/"+id.ToString());
                if(apiresult.Success)
                {
                    Category item = JsonConvert.DeserializeObject<Category>(apiresult.Data);
                    return View(item);
                }
                else
                {
                    return Json(apiresult);
                }               

            }
            catch (Exception Ex)
            {

                throw;
            }   
           
        }

        // GET: CategoryController/Create
        public ActionResult Create()
        {
           
            return View();
        }

        // POST: CategoryController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public ActionResult Create(IFormCollection collection)
        public async Task<ActionResult> Create(Category item)
        {
            try
            {
                var rs=await httpClientHelper.PostAsync("Category", JsonConvert.SerializeObject(item));
                if(rs.Success)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    return Json(rs);
                }
               
            }
            catch
            {
                return View();
            }
        }

        // GET: CategoryController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {

            try
            {
                var apiresult = await httpClientHelper.GetAsync("Category/" + id.ToString());
                if (apiresult.Success)
                {
                    Category item = JsonConvert.DeserializeObject<Category>(apiresult.Data);
                    return View(item);
                }
                else
                {
                    return Json(apiresult);
                }
                

            }
            catch (Exception Ex)
            {
                return BadRequest(Ex.Message);  
            }
        }

        // POST: CategoryController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, Category Item)
        {
            try
            {
                var apiresult = await httpClientHelper.PutAsync("Category/" + id.ToString(),JsonConvert.SerializeObject(Item));
                if (apiresult.Success)
                {
                    return RedirectToAction(nameof(Index));
                }
                return NotFound();
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        // GET: CategoryController/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var apiresult = await httpClientHelper.GetAsync("Category/" + id.ToString());
                if (apiresult.Success)
                {
                    Category item = JsonConvert.DeserializeObject<Category>(apiresult.Data);
                    return View(item);
                }
                return Json(apiresult);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        // POST: CategoryController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id, Category item)
        {
            try
            {
                var apiresult = await httpClientHelper.DeleteAsync("Category/" + id.ToString());
                if (apiresult.Success)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    return Json(apiresult);
                }
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
    }
}
