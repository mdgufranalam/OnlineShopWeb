
using OnlineShop.ServiceHelper.Interface;
using System.Net;
using System.Net.Http.Json;
using System.Text;
namespace OnlineShop.ServiceHelper
{
    public class HttpClientHelper : IHttpClientHelper
    {
        private readonly IHttpClientFactory httpClientFactory;

        public HttpClientHelper(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;

        }

        //action Url : Category/1,  only action url except Base url
        public async Task<string> GetAsync(string actionUrl)
        {
            string response=string.Empty;
            using (var _client=  httpClientFactory.CreateClient("ShopApi"))
            {
                //specify to use TLS 1.2 as default connection
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                var result = await _client.GetAsync(actionUrl);
                if (result.IsSuccessStatusCode)
                {
                    response = await result.Content.ReadAsStringAsync();
                }
                //var rs2 =await _client.GetFromJsonAsync<List<Quotes>>(actionUrl);
                return response;
            }
        }

        public async Task<string> PostAsync(string actionUrl, string data)
        {
            string response = string.Empty;
            using (var _client = httpClientFactory.CreateClient("ShopApi"))
            {
                //specify to use TLS 1.2 as default connection
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                HttpContent content=new StringContent(data,Encoding.UTF8,"application/json");
               var result = await _client.PostAsync(actionUrl,content);
                if (result.IsSuccessStatusCode)
                {
                    response = await result.Content.ReadAsStringAsync();
                }
                //var rs2 =await _client.GetFromJsonAsync<List<Quotes>>(actionUrl);
                return response;
            }
        }

        public  async Task<string> DeleteAsync(string actionUrl)
        {
            try
            {
                string response = string.Empty;
                using (var _client = httpClientFactory.CreateClient("ShopApi"))
                {
                    //specify to use TLS 1.2 as default connection
                    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                    //HttpContent content = new StringContent(data, Encoding.UTF8, "application/json");
                    var result = await _client.DeleteAsync(actionUrl);
                    if (result.IsSuccessStatusCode)
                    {
                        response = await result.Content.ReadAsStringAsync();
                    }
                    else
                    {
                        throw new Exception(response);
                    }
                    //var rs2 =await _client.GetFromJsonAsync<List<Quotes>>(actionUrl);
                    return response;
                }
            }
            catch (Exception )
            {

                throw;
            }

        }
        public async Task<string> PutAsync(string actionUrl,string data)
        {
            try
            {
                string response = string.Empty;
                using (var _client = httpClientFactory.CreateClient("ShopApi"))
                {
                    //specify to use TLS 1.2 as default connection
                    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                    HttpContent content = new StringContent(data, Encoding.UTF8, "application/json");
                    var result = await _client.PutAsync(actionUrl,content);
                    if (result.IsSuccessStatusCode)
                    {
                        response = await result.Content.ReadAsStringAsync();
                    }
                    else
                    {
                        throw new Exception(response);
                    }
                    //var rs2 =await _client.GetFromJsonAsync<List<Quotes>>(actionUrl);
                    return response;
                }
            }
            catch (Exception)
            {

                throw;
            }

        }

    }
}
