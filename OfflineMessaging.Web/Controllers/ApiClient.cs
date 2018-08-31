using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;

namespace OfflineMessaging.Web.Controllers
{
    public class ApiClient 
    {
        private HttpClient _client;


        public HttpClient GetApiClient()
        {

            if (_client == null)
            {
                _client = new HttpClient()
                {
                    BaseAddress = new Uri("http://localhost:5147/")
                };
                _client.DefaultRequestHeaders.Accept.Clear();
                _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                _client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/x-www-form-urlencoded");
            }
            return _client;
        }
    }
}