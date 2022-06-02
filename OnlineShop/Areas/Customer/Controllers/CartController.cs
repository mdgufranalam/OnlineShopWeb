using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OnlineShop.Models;
using OnlineShop.Models.ViewModel;
using OnlineShop.ServiceHelper.Interface;
using OnlineShop.Utility;
using Stripe.Checkout;
using System.Security.Claims;

namespace OnlineShop.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IHttpClientHelper httpClientHelper;        
     
        private readonly IAuth0ClientHelper auth0ClientHelper;
        private readonly ApplicationDbContext _db;
        private readonly IConfiguration config;
        private ServiceResult<string> exception;
        private readonly Auth0 auth;
        [BindProperty]
        public ShoppingCartVM ShoppingCartVM { get; set; }

        public CartController(IHttpClientHelper httpClientHelper, Auth0 auth, IAuth0ClientHelper auth0ClientHelper, ApplicationDbContext db)
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
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var apiresult = await httpClientHelper.GetAsync("ShoppingCart/GetShoppingCart/"+ claim.Value);
            if (TempData.ContainsKey("Result"))
                ViewData["Result"] = TempData["Result"];
            if (apiresult.Success)
            {
                ShoppingCartVM = new ShoppingCartVM()
                {
                    ListCart = JsonConvert.DeserializeObject<IEnumerable<ShoppingCart>>(apiresult.Data),
                    OrderHeader = new()
                };
                foreach (var cart in ShoppingCartVM.ListCart)
                {
                    cart.Price = cart.Products.Price;
                    ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
                }
                return View(ShoppingCartVM);
            }
            return View(new List<ShoppingCart>());
        }

        public async Task<IActionResult> Summary()
        {
            try
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                var apiresult = await httpClientHelper.GetAsync("ShoppingCart/GetShoppingCart/" + claim.Value);
                if(apiresult.Success)
                {
                    ShoppingCartVM = new ShoppingCartVM()
                    {
                        ListCart = JsonConvert.DeserializeObject<IEnumerable<ShoppingCart>>(apiresult.Data),
                        OrderHeader = new()
                    };
                    var UserDetails = await httpClientHelper.GetAsync("ApplicationUser/" + claim.Value);
                    if(UserDetails.Success)
                    {
                        ShoppingCartVM.OrderHeader.ApplicationUser = JsonConvert.DeserializeObject<ApplicationUser>(UserDetails.Data);

                        ShoppingCartVM.OrderHeader.Name = ShoppingCartVM.OrderHeader.ApplicationUser.Name;
                        ShoppingCartVM.OrderHeader.PhoneNumber = ShoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber;
                        ShoppingCartVM.OrderHeader.StreetAddress = ShoppingCartVM.OrderHeader.ApplicationUser.StreetAddress;
                        ShoppingCartVM.OrderHeader.City = ShoppingCartVM.OrderHeader.ApplicationUser.City;
                        ShoppingCartVM.OrderHeader.State = ShoppingCartVM.OrderHeader.ApplicationUser.State;
                        ShoppingCartVM.OrderHeader.PostalCode = ShoppingCartVM.OrderHeader.ApplicationUser.PostalCode;

                    }



                    foreach (var cart in ShoppingCartVM.ListCart)
                    {
                        cart.Price = cart.Products.Price;
                        ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
                    }
                    return View(ShoppingCartVM);
                }
                return View(ShoppingCartVM);

            }
            catch (Exception)
            {

                throw;
            }
        }
        [HttpPost]
        [ActionName("Summary")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SummaryPOST()
        {
            try
            {
                //if(string.IsNullOrEmpty(ShoppingCartVM.OrderHeader.PhoneNumber) || ShoppingCartVM.OrderHeader.PhoneNumber.Length<10)
                //{
                //    ModelState.AddModelError("OrderHeader.PhoneNumber", "The PhoneNumber field is required.");
                //    return RedirectToAction("Summary");
                //}
                //else if(ModelState.IsValid)
                //{
                    var claimsIdentity = (ClaimsIdentity)User.Identity;
                    var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                    var apiresult = await httpClientHelper.GetAsync("ShoppingCart/GetShoppingCart/" + claim.Value);
                    if (apiresult.Success)
                    {
                        ShoppingCartVM.ListCart = JsonConvert.DeserializeObject<IEnumerable<ShoppingCart>>(apiresult.Data);
                    }
                    ShoppingCartVM.OrderHeader.OrderDate = System.DateTime.Now;
                    ShoppingCartVM.OrderHeader.ApplicationUserId = claim.Value;


                    foreach (var cart in ShoppingCartVM.ListCart)
                    {
                        cart.Price = cart.Products.Price;
                        ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
                    }
                    var UserDetails = await httpClientHelper.GetAsync("ApplicationUser/" + claim.Value);
                    ApplicationUser applicationUser;
                    if (UserDetails.Success)
                    {
                        applicationUser = JsonConvert.DeserializeObject<ApplicationUser>(UserDetails.Data);
                    }
                    ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
                    ShoppingCartVM.OrderHeader.OrderStatus = SD.StatusPending;

                    var CreateOrderHeader = await httpClientHelper.PostAsync("OrderHeader", JsonConvert.SerializeObject(ShoppingCartVM.OrderHeader));
                    if (CreateOrderHeader.Success)
                    {
                        ShoppingCartVM.OrderHeader = JsonConvert.DeserializeObject<OrderHeader>(CreateOrderHeader.Data);
                    }

                    foreach (var cart in ShoppingCartVM.ListCart)
                    {
                        OrderDetail orderDetail = new()
                        {
                            ProductId = cart.ProductId,
                            OrderId = ShoppingCartVM.OrderHeader.Id,
                            Price = cart.Price,
                            Count = cart.Count
                        };
                        var CreatedOrderDetails = await httpClientHelper.PostAsync("OrderDetails", JsonConvert.SerializeObject(orderDetail));
                        if (CreatedOrderDetails.Success)
                        {
                            orderDetail = JsonConvert.DeserializeObject<OrderDetail>(CreatedOrderDetails.Data);
                        }
                    }

                    #region commented for payment

                    var domain = $"http://gufrankhan5252-001-site1.dtempurl.com/";
                    var options = new SessionCreateOptions
                    {
                        PaymentMethodTypes = new List<string>
                    {
                      "card",
                    },
                        LineItems = new List<SessionLineItemOptions>(),
                        Mode = "payment",
                        SuccessUrl = domain + $"customer/cart/OrderConfirmation?id={ShoppingCartVM.OrderHeader.Id}",
                        CancelUrl = domain + $"customer/cart/index",
                    };

                    foreach (var item in ShoppingCartVM.ListCart)
                    {

                        var sessionLineItem = new SessionLineItemOptions
                        {
                            PriceData = new SessionLineItemPriceDataOptions
                            {
                                UnitAmount = (long)(item.Price * 100),//20.00 -> 2000
                                Currency = "inr",
                                ProductData = new SessionLineItemPriceDataProductDataOptions
                                {
                                    Name = item.Products.Title
                                },

                            },
                            Quantity = item.Count,
                        };
                        options.LineItems.Add(sessionLineItem);

                    }

                    var service = new SessionService();
                    Session session = service.Create(options);
                    var sessionsaveapi = await httpClientHelper.PostAsync("OrderHeader/UpdateStripePaymentID", JsonConvert.SerializeObject(new { id = ShoppingCartVM.OrderHeader.Id, session = session.Id, paymentItentId = session.PaymentIntentId }));
                    if (!sessionsaveapi.Success)
                    {
                        return BadRequest("Some Error Occured.");
                    }
                    Response.Headers.Add("Location", session.Url);
                    return new StatusCodeResult(303);
                    #endregion
                //}
                //else
                //{
                //    return RedirectToAction("Summary");
                //}
               
                


            }
            catch (Exception ex)
            {

                throw ex;
            }
            
            //return RedirectToAction("OrderConfirmation", "Cart", new { id = ShoppingCartVM.OrderHeader.Id });
           
        }

        public async Task<IActionResult> Plus(int cartId)
        {
            var apiresult = await httpClientHelper.GetAsync("ShoppingCart/Plus/" + cartId);
            if(apiresult.Success)
            {
                HttpContext.Session.SetInt32(SD.SessionCart,Convert.ToInt32(apiresult.Data));
            }
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Minus(int cartId)
        {
            var apiresult = await httpClientHelper.GetAsync("ShoppingCart/Minus/" + cartId);
            if (apiresult.Success)
            {
                HttpContext.Session.SetInt32(SD.SessionCart, Convert.ToInt32(apiresult.Data));
            }
            return RedirectToAction(nameof(Index));
           
        }

        public async Task<IActionResult> Remove(int cartId)
        {
            var apiresult = await httpClientHelper.GetAsync("ShoppingCart/Remove/" + cartId);
            if (apiresult.Success)
            {
                HttpContext.Session.SetInt32(SD.SessionCart, Convert.ToInt32(apiresult.Data));
            }
            return RedirectToAction(nameof(Index));
           
        }

        public async Task<IActionResult> OrderConfirmation(int id)
        {
            var apiHeaderresult = await httpClientHelper.GetAsync("OrderHeader/GetFirstOrDefault/" + id);
            OrderHeader orderHeader=new OrderHeader();
            if (apiHeaderresult.Success)
            {
                orderHeader = JsonConvert.DeserializeObject<OrderHeader>(apiHeaderresult.Data);
            }
            if (orderHeader.PaymentStatus != SD.PaymentStatusDelayedPayment)
            {
                var service = new SessionService();
                Session session = service.Get(orderHeader.SessionId);
                //check the stripe status
                if (session.PaymentStatus.ToLower() == "paid")
                {
                    var obj = new { Id = id, PaymentStatus = SD.PaymentStatusApproved, OrderStatus = SD.StatusApproved };
                    var apiHeaderUpdate = await httpClientHelper.PostAsync("OrderHeader/UpdateOrderHeader", JsonConvert.SerializeObject(obj));
                    if (!apiHeaderUpdate.Success)
                    {
                        return BadRequest("Some Error Occured in Update Status.");
                    }
                }

                
            }
            //_emailSender.SendEmailAsync(orderHeader.ApplicationUser.Email, "New Order - Bulky Book", "<p>New Order Created</p>");

            var apiresult = await httpClientHelper.DeleteAsync("ShoppingCart/DeleteShoppingCart/" + orderHeader.ApplicationUserId);
            if (apiresult.Success)
            {
                HttpContext.Session.SetInt32(SD.SessionCart, Convert.ToInt32(0));
            }
            ServiceResult<string> serviceResult = new ServiceResult<string>();
            serviceResult.Data = "Your Order is Successfully Placed. Order Id - " + id;
            TempData["Result"]=JsonConvert.SerializeObject(serviceResult);
            return RedirectToAction("Index", "Home", new { area = "Customer" });
        }


    }
}
