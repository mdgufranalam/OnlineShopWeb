using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShop.Models
{
    public sealed class Auth0
    {
        Auth0() { }
        private static readonly object lock1 = new object();  
        private static Auth0 instance = null;
        public static Auth0 Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (lock1)
                        {
                            if (instance == null)
                            {
                                instance = new Auth0();
                            }
                        }
                }
                return instance;
            }
        }
        public  string client_id { get; set; }
        public  string client_secret { get; set; }
        public  string audience { get; set; }
        public  string grant_type { get; set; }
        public  string access_token { get; set; }
        public  double expires_in { get; set; }
        public  string url { get; set; }
        public  DateTime current_time { get { return DateTime.Now; } set { } }

        public DateTime expires_out { get; set; }
           

        //Timer timer { get; set; }   
    }
}
