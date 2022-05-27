using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OnlineShop.Models;
using OnlineShop.Models.ViewModel;
using OnlineShop.ServiceHelper.Interface;
using OnlineShop.Utility;
using System.Security.Claims;

namespace OnlineShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IHttpClientHelper httpClientHelper;
        private readonly Auth0 auth;
        private readonly IAuth0ClientHelper auth0ClientHelper;


        [BindProperty]
        public OrderVM OrderVM { get; set; }

        public OrderController(IHttpClientHelper httpClientHelper, Auth0 auth, IAuth0ClientHelper auth0ClientHelper)
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
        }
        ServiceResult<string> serviceResult = new ServiceResult<string>();

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Details(int orderId)
        {
            try
            {
                if (TempData.ContainsKey("Result"))
                    ViewData["Result"] = TempData["Result"];
                var OrderHeaderDetails = await httpClientHelper.GetAsync("Order/" + orderId);
                if (OrderHeaderDetails.Success)
                {
                    OrderVM = JsonConvert.DeserializeObject<OrderVM>(OrderHeaderDetails.Data);
                }
                else
                {
                    return BadRequest("Some Error Occured");

                }

                return View(OrderVM);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            
        }

        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateOrderDetail()
        {
            serviceResult = new ServiceResult<string>();
            try
            {
                OrderHeader orderHEaderFromDb = null;
                var OrderHeaderDtls = await httpClientHelper.GetAsync("OrderHeader/GetFirstOrDefaultNoTrack" + OrderVM.OrderHeader.Id);
                if (OrderHeaderDtls.Success)
                {
                    orderHEaderFromDb = JsonConvert.DeserializeObject<OrderHeader>(OrderHeaderDtls.Data);
                    orderHEaderFromDb.Name = OrderVM.OrderHeader.Name;
                    orderHEaderFromDb.PhoneNumber = OrderVM.OrderHeader.PhoneNumber;
                    orderHEaderFromDb.StreetAddress = OrderVM.OrderHeader.StreetAddress;
                    orderHEaderFromDb.City = OrderVM.OrderHeader.City;
                    orderHEaderFromDb.State = OrderVM.OrderHeader.State;
                    orderHEaderFromDb.PostalCode = OrderVM.OrderHeader.PostalCode;
                }
                if (OrderVM.OrderHeader.Carrier != null)
                {
                    orderHEaderFromDb.Carrier = OrderVM.OrderHeader.Carrier;
                }
                if (OrderVM.OrderHeader.TrackingNumber != null)
                {
                    orderHEaderFromDb.TrackingNumber = OrderVM.OrderHeader.TrackingNumber;
                }
                var updateorderheader = await httpClientHelper.PostAsync("OrderHeader/Update", JsonConvert.SerializeObject(orderHEaderFromDb));
                if (updateorderheader.Success)
                {
                    serviceResult.Success = true;
                    serviceResult.Data = "Order Details Updated Successfully.";
                }
                else
                {
                    serviceResult.Success = false;
                    serviceResult.Message = "Some Error Occured.";
                }
                TempData["Result"] = JsonConvert.SerializeObject(serviceResult);
                return RedirectToAction("Details", "Order", new { orderId = orderHEaderFromDb.Id });
            }
            catch (Exception ex) 
            {
                serviceResult.Success = false;
                serviceResult.Message = ex.Message;
                TempData["Result"] = JsonConvert.SerializeObject(serviceResult);
                return RedirectToAction(nameof(Index));
            }
           

            
        }

        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StartProcessing()
        {
            serviceResult = new ServiceResult<string>();
            try
            {
                var obj = new { Id = OrderVM.OrderHeader.Id, OrderStatus = SD.StatusInProcess };
                var apiHeaderUpdate = await httpClientHelper.PostAsync("OrderHeader/UpdateOrderHeader", JsonConvert.SerializeObject(obj));
                if (apiHeaderUpdate.Success)
                {
                    serviceResult.Success = true;
                    serviceResult.Data = "Order Status Updated Successfully.";
                }
                else
                {
                    serviceResult.Success = false;
                    serviceResult.Message = "Some Error Occured.";
                }
            }
            catch(Exception ex)
            {
                serviceResult.Success = false;
                serviceResult.Message = ex.Message;
            }
            TempData["Result"] = JsonConvert.SerializeObject(serviceResult);
            return RedirectToAction("Details", "Order", new { orderId = OrderVM.OrderHeader.Id });
        }

        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ShipOrder()
        {
            serviceResult = new ServiceResult<string>();
            try
            {
                //var orderHeader = new OrderHeader();
                //orderHeader.TrackingNumber = OrderVM.OrderHeader.TrackingNumber;
                //orderHeader.Carrier = OrderVM.OrderHeader.Carrier;
                //orderHeader.OrderStatus = SD.StatusShipped;
                //orderHeader.ShippingDate = DateTime.Now;

                OrderVM.OrderHeader.OrderStatus = SD.StatusShipped;
                OrderVM.OrderHeader.ApplicationUserId= OrderVM.OrderHeader.ApplicationUser.Id;
                var apiHeaderUpdate = await httpClientHelper.PostAsync("OrderHeader/ShipOrder", JsonConvert.SerializeObject(OrderVM.OrderHeader));
                if (apiHeaderUpdate.Success)
                {
                    serviceResult.Success = true;
                    serviceResult.Data = "Order Shipped Successfully.";
                }
                else
                {
                    serviceResult.Success = true;
                    serviceResult.Message = "Some error occured.";
                }
                TempData["Result"] = JsonConvert.SerializeObject(serviceResult);
              
            }
            catch (Exception ex)
            {
                serviceResult.Success = false;
                serviceResult.Message = ex.Message;
                TempData["Result"] = JsonConvert.SerializeObject(serviceResult);
            }
            return RedirectToAction("Details", "Order", new { orderId = OrderVM.OrderHeader.Id });
        }

        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelOrder()
        {
            serviceResult = new ServiceResult<string>();
            try
            {
                string PaymentsStatus = "";
                var OrderHeaderDtls = await httpClientHelper.GetAsync("OrderHeader/GetFirstOrDefaultNoTrack" + OrderVM.OrderHeader.Id);
                if (OrderHeaderDtls.Success)
                {
                    var orderHeader=JsonConvert.DeserializeObject<OrderHeader>(OrderHeaderDtls.Data);   
                    if (orderHeader.PaymentStatus == SD.PaymentStatusApproved)
                    {
                        //var options = new RefundCreateOptions
                        //{
                        //    Reason = RefundReasons.RequestedByCustomer,
                        //    PaymentIntent = orderHeader.PaymentIntentId
                        //};

                        //var service = new RefundService();
                        //Refund refund = service.Create(options);

                        PaymentsStatus= SD.StatusRefunded;
                    }
                    else
                    {
                        PaymentsStatus=SD.StatusCancelled;
                    }
                    var obj = new { Id = OrderVM.OrderHeader.Id, OrderStatus = SD.StatusCancelled ,PaymentStatus= PaymentsStatus };
                    var apiHeaderUpdate = await httpClientHelper.PostAsync("OrderHeader/UpdateOrderHeader", JsonConvert.SerializeObject(obj));
                    if (apiHeaderUpdate.Success)
                    {
                        serviceResult.Success = true;
                        serviceResult.Data = "Order Cancelled Successfully.";
                    }
                    else
                    {
                        serviceResult.Success = false;
                        serviceResult.Message = "Some Error Occured.";
                    }
                }
            }
            catch (Exception ex)
            {
                serviceResult.Success = false;
                serviceResult.Message = ex.Message;
                TempData["Result"] = JsonConvert.SerializeObject(serviceResult);
            }
            TempData["Result"] = JsonConvert.SerializeObject(serviceResult);

            return RedirectToAction("Details", "Order", new { orderId = OrderVM.OrderHeader.Id });
        }

        #region API CALLS
        [HttpGet]
        public async Task<IActionResult> GetAll(string status)
        {
            IEnumerable<OrderHeader> orderHeaders;

            if (User.IsInRole(SD.Role_Admin) || User.IsInRole(SD.Role_Employee))
            {
                var apiresult =await httpClientHelper.GetAsync("OrderHeader/GetAll");
                if (apiresult.Success)
                {
                    orderHeaders = JsonConvert.DeserializeObject<IEnumerable<OrderHeader>>(apiresult.Data);
                }
                else
                {
                    return BadRequest("Some Error Occured");
                }         
            }
            else
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                var apiresult =await httpClientHelper.GetAsync("OrderHeader/GetApplicationUserOrderHeader/"+ claim.Value);
                if (apiresult.Success)
                {
                    orderHeaders = JsonConvert.DeserializeObject<IEnumerable<OrderHeader>>(apiresult.Data);
                }
                else
                {
                    return BadRequest("Some Error Occured");
                }  
            }

            switch (status)
            {
                case "pending":
                    orderHeaders = orderHeaders.Where(u => u.PaymentStatus == SD.PaymentStatusDelayedPayment);
                    break;
                case "inprocess":
                    orderHeaders = orderHeaders.Where(u => u.OrderStatus == SD.StatusInProcess);
                    break;
                case "completed":
                    orderHeaders = orderHeaders.Where(u => u.OrderStatus == SD.StatusShipped);
                    break;
                case "approved":
                    orderHeaders = orderHeaders.Where(u => u.OrderStatus == SD.StatusApproved);
                    break;
                default:
                    break;
            }


            return Json(new { data = orderHeaders });
        }
        #endregion
    }
}
