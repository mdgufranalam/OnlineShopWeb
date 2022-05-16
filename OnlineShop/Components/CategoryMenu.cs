using Microsoft.AspNetCore.Mvc;
using OnlineShop.Models;
using OnlineShop.ServiceHelper.Interface;
using Newtonsoft.Json;

namespace OnlineShop.Components
{
    public class CategoryMenu : ViewComponent
    {
        private readonly IHttpClientHelper httpClientHelper;

        public CategoryMenu(IHttpClientHelper httpClientHelper)
        {
            this.httpClientHelper = httpClientHelper;
        }

        public IViewComponentResult Invoke()
        {
            try
            {
                var t = Task.Run(() => httpClientHelper.GetAsync("Category"));
                t.Wait();
                var cat = t.Result;
                //var cat = httpClientHelper.GetAsync("Category").AsyncState;
                if (cat.Success)
                {
                    IEnumerable<Category> category = JsonConvert.DeserializeObject<IEnumerable<Category>>(cat.Data);
                    return View("Default", category);
                }
            }
            catch (Exception ex)
            {
                
                throw;
            }
            return View();
        }

    }
}
