using Microsoft.AspNetCore.Mvc;
using OnlineShop.ServiceHelper.Interface;
using OnlineShop.Utility;
using System.Security.Claims;

namespace OnlineShop.Components
{
    public class ShopingCartComp : ViewComponent
    {
        private readonly IHttpClientHelper httpClientHelper;
        public ShopingCartComp(IHttpClientHelper httpClientHelper)
        {
            this.httpClientHelper = httpClientHelper;
        }

        public IViewComponentResult Invoke()
        {
            try
            {
               
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                if (claim != null)
                {
                    if (HttpContext.Session.GetInt32(SD.SessionCart) != null)
                    {
                        return View(HttpContext.Session.GetInt32(SD.SessionCart));
                    }
                    else
                    {
                        var apiresult = httpClientHelper.GetAsync("ShoppingCart/CartCount/" + claim.Value).GetAwaiter().GetResult();
                        if(apiresult.Success)
                        {
                            HttpContext.Session.SetInt32(SD.SessionCart, Convert.ToInt32(apiresult.Data));
                            return View(HttpContext.Session.GetInt32(SD.SessionCart));
                        }
                        return View(0);
                    }
                }
                else
                {
                    HttpContext.Session.Clear();
                    return View(0);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }
    }
}
