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
                
                if(string.IsNullOrEmpty(apiresult))
                {
                   // return NotFound();
                }
                IEnumerable<Category> categories = JsonConvert.DeserializeObject<IEnumerable<Category>>(apiresult);
                return View(categories);
            }
            catch (Exception)
            {

                throw;
            }
           
        }

        // GET: CategoryController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            try
            {
                var apiresult = await httpClientHelper.GetAsync("Category/"+id.ToString());
                if(apiresult != "Category Not Found.")
                {
                    Category item = JsonConvert.DeserializeObject<Category>(apiresult);
                    return View(item);
                }
                return NotFound();

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
                if(rs !="")
                {
                    return BadRequest(rs);
                }
                return RedirectToAction(nameof(Index));
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
                if (apiresult != "Category Not Found.")
                {
                    Category item = JsonConvert.DeserializeObject<Category>(apiresult);
                    return View(item);
                }
                return NotFound();

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
                if (apiresult != "Category Not Found.")
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
                if (apiresult != "Category Not Found.")
                {
                    Category item = JsonConvert.DeserializeObject<Category>(apiresult);
                    return View(item);
                }
                return NotFound();
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
                if (apiresult != "Category Not Found." && apiresult == id.ToString())
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
    }
}
