namespace OnlineShop.ServiceHelper.Interface
{
    public interface IHttpClientHelper
    {
        public  Task<string> GetAsync(string actionUrl);
        public  Task<string> PostAsync(string actionUrl, string data);
        public  Task<string> PutAsync(string actionUrl, string data);
        public  Task<string> DeleteAsync(string actionUrl);
    }
}