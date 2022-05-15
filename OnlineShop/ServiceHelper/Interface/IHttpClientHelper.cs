using OnlineShop.Models;

namespace OnlineShop.ServiceHelper.Interface
{
    public interface IHttpClientHelper
    {
        public Task<ServiceResult<string>> GetAsync(string actionUrl);
        public Task<ServiceResult<string>> PostAsync(string actionUrl, string data);
        public Task<ServiceResult<string>> PutAsync(string actionUrl, string data);
        public Task<ServiceResult<string>> DeleteAsync(string actionUrl);

        public Task<ServiceResult<string>> PostMultipartAsync(string actionUrl, string data,IFormFile file);

    }
   
}