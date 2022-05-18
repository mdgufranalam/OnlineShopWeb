using Newtonsoft.Json;
using OnlineShop.Models;
using System.Net;
using System.Text;

namespace OnlineShop.ServiceHelper.Interface
{
    public interface IAuth0ClientHelper
    {
        Task<ServiceResult<Auth0>> GetTokenAsync(Auth0 auth);
    }
    public class Auth0ClientHelper : IAuth0ClientHelper
    {
        private readonly IHttpClientFactory httpClientFactory;

        public Auth0ClientHelper(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        public async Task<ServiceResult<Auth0>> GetTokenAsync(Auth0 auth)
        {
            ServiceResult<Auth0> serviceResult = new ServiceResult<Auth0>();
            string response = string.Empty;
            try
            {
                using (var _client = httpClientFactory.CreateClient())
                {
                    //specify to use TLS 1.2 as default connection
                    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                    HttpContent content = new StringContent(JsonConvert.SerializeObject(auth), Encoding.UTF8, "application/json");
                    var result = await _client.PostAsync(auth.url, content);
                    if (result.IsSuccessStatusCode)
                    {
                        serviceResult.Data = JsonConvert.DeserializeObject<Auth0>( await result.Content.ReadAsStringAsync());
                        auth.access_token= serviceResult.Data.access_token;
                        auth.expires_out= DateTime.Now.AddSeconds(serviceResult.Data.expires_in);
                        serviceResult.Data = auth;
                    }
                    else if ((int)result.StatusCode == StatusCodes.Status404NotFound)
                    {
                        serviceResult.Message = await result.Content.ReadAsStringAsync();
                        serviceResult.Success = false;
                    }
                    else if ((int)result.StatusCode == StatusCodes.Status400BadRequest)
                    {
                        serviceResult.Message = await result.Content.ReadAsStringAsync();
                        serviceResult.Success = false;
                    }
                    else if ((int)result.StatusCode == StatusCodes.Status401Unauthorized)
                    {
                        serviceResult.Message = "Unauthorized Access of Auth0.";
                        serviceResult.Success = false;
                    }
                    //var rs2 =await _client.GetFromJsonAsync<List<Quotes>>(actionUrl);
                    return serviceResult;
                }
            }
            catch (HttpRequestException ex)
            {
                serviceResult.Message = "Backend server is down, please connect with admin.";
                serviceResult.Success = false;

            }
            catch (Exception ex)
            {
                serviceResult.Message = ex.Message;
                serviceResult.Success = false;
            }
            return serviceResult;
        }
    }
}
