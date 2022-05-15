
using OnlineShop.Models;
using OnlineShop.ServiceHelper.Interface;
using System.Net;
using System.Net.Http.Json;
using System.Text;
namespace OnlineShop.ServiceHelper
{
    public class HttpClientHelper: IHttpClientHelper
    {
        private readonly IHttpClientFactory httpClientFactory;

        public HttpClientHelper(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;

        }

        ////action Url : Category/1,  only action url except Base url
        public async Task<ServiceResult<string>> GetAsync(string actionUrl)
        {
            ServiceResult<string> serviceResult = new ServiceResult<string>();
            string response = string.Empty;
            try
            {
                using (var _client = httpClientFactory.CreateClient("ShopApi"))
                {
                    //specify to use TLS 1.2 as default connection
                    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                    var result = await _client.GetAsync(actionUrl);
                    if (result.IsSuccessStatusCode)
                    {
                        serviceResult.Data = await result.Content.ReadAsStringAsync();
                    }
                    else if((int)result.StatusCode == StatusCodes.Status404NotFound)
                    {
                        serviceResult.Message = await result.Content.ReadAsStringAsync();
                        serviceResult.Success = false;
                    }
                    else if ((int)result.StatusCode == StatusCodes.Status400BadRequest)
                    {
                        serviceResult.Message = await result.Content.ReadAsStringAsync();
                        serviceResult.Success = false;
                    }
                    //var rs2 =await _client.GetFromJsonAsync<List<Quotes>>(actionUrl);
                    return serviceResult;
                }
            }
            catch (HttpRequestException ex)
            {
                serviceResult.Message = "Error occured while connecting to API";
                serviceResult.Success = false;
               
            }
            catch (Exception ex)
            {
                serviceResult.Message = ex.Message;
                serviceResult.Success = false;
            }
            return serviceResult;
        }

        public async Task<ServiceResult<string>> PostAsync(string actionUrl, string data)
        {
            ServiceResult<string> serviceResult = new ServiceResult<string>();
            string response = string.Empty;
            try
            {
                using (var _client = httpClientFactory.CreateClient("ShopApi"))
                {
                    //specify to use TLS 1.2 as default connection
                    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                    HttpContent content = new StringContent(data, Encoding.UTF8, "application/json");
                    var result = await _client.PostAsync(actionUrl, content);
                    if (result.IsSuccessStatusCode)
                    {
                        serviceResult.Data = await result.Content.ReadAsStringAsync();
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
                    //var rs2 =await _client.GetFromJsonAsync<List<Quotes>>(actionUrl);
                    return serviceResult;
                }
            }
            catch (HttpRequestException ex)
            {
                serviceResult.Message = "Error occured while connecting to API";
                serviceResult.Success = false;

            }
            catch (Exception ex)
            {
                serviceResult.Message = ex.Message;
                serviceResult.Success = false;
            }
            return serviceResult;

        }

        public async Task<ServiceResult<string>> DeleteAsync(string actionUrl)
        {
            ServiceResult<string> serviceResult = new ServiceResult<string>();
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
                        serviceResult.Data = await result.Content.ReadAsStringAsync();
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
                    //var rs2 =await _client.GetFromJsonAsync<List<Quotes>>(actionUrl);
                    return serviceResult;
                }
            }
            catch (HttpRequestException)
            {
                serviceResult.Message = "Error Occured while connecting to API";
                serviceResult.Success = false;

            }
            catch (Exception ex)
            {
                serviceResult.Message = ex.Message;
                serviceResult.Success = false;
            }
            return serviceResult;

        }
        public async Task<ServiceResult<string>> PutAsync(string actionUrl, string data)
        {
            ServiceResult<string> serviceResult = new ServiceResult<string>();
            try
            {
                string response = string.Empty;
                using (var _client = httpClientFactory.CreateClient("ShopApi"))
                {
                    //specify to use TLS 1.2 as default connection
                    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                    HttpContent content = new StringContent(data, Encoding.UTF8, "application/json");
                    var result = await _client.PutAsync(actionUrl, content);
                    if (result.IsSuccessStatusCode)
                    {
                        serviceResult.Data = await result.Content.ReadAsStringAsync();
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
                    //var rs2 =await _client.GetFromJsonAsync<List<Quotes>>(actionUrl);
                    return serviceResult;
                }
            }
            catch (HttpRequestException)
            {
                serviceResult.Message = "Error Occured while connecting to API";
                serviceResult.Success = false;

            }
            catch (Exception ex)
            {
                serviceResult.Message = ex.Message;
                serviceResult.Success = false;
            }
            return serviceResult;

        }

        public async Task<ServiceResult<string>> PostMultipartAsync(string actionUrl, string data, IFormFile file)
        {
            ServiceResult<string> serviceResult = new ServiceResult<string>();
            try
            {
               
                if (file != null && file.Length > 0)
                {
                    using (var client = httpClientFactory.CreateClient("ShopApi"))
                    {
                        try
                        {
                            //client.BaseAddress = new Uri(currentPrivateBackendAddress);                            
                            byte[] byteData;
                            using (var br = new BinaryReader(file.OpenReadStream()))
                                byteData = br.ReadBytes((int)file.OpenReadStream().Length);

                            ByteArrayContent bytes = new ByteArrayContent(byteData);
                            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                            
                           
                            //HttpContent datacontent = new StringContent(data, Encoding.UTF8, "application/json");                           
                            MultipartFormDataContent multiContent = new MultipartFormDataContent();
                           
                            
                            multiContent.Add(bytes, "file", file.FileName);                           
                            //multiContent.Add(datacontent, "product");

                            var result = await client.PostAsync(actionUrl, multiContent);
                            if (result.IsSuccessStatusCode)
                            {
                                serviceResult.Data = await result.Content.ReadAsStringAsync();
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
                            //var rs2 =await _client.GetFromJsonAsync<List<Quotes>>(actionUrl);

                            return serviceResult;

                        }
                        catch (Exception ex)
                        {
                            serviceResult.Success = false;
                            serviceResult.Message = ex.Message;                           
                        }
                    }
                }
                return serviceResult;
            }
            catch (Exception ex)
            {
                serviceResult.Success = false;
                serviceResult.Message = ex.Message;
                return serviceResult;
            }
        }
    }
}
