using Newtonsoft.Json;
using OfflineMessaging.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace OfflineMessaging.Web.Controllers
{
    public class MessageController : Controller
    {
        // GET: Message
        public async Task<ActionResult> Index()
        {
            if (Session["Token"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                ApiClient _client = new ApiClient();
                HttpClient client = _client.GetApiClient();
                client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "Bearer " + Session["Token"].ToString());

                HttpResponseMessage response = await client.GetAsync("api/accounts/user/" + Session["Username"]);
                var result = response.Content.ReadAsStringAsync().Result;
                User user = JsonConvert.DeserializeObject<User>(result);
                return View("Index", user);
            }
            
        }
        public PartialViewResult MessagedUsers()
        {
            ApiClient _client = new ApiClient();
            HttpClient client = _client.GetApiClient();
            client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "Bearer " + Session["Token"].ToString());

            HttpResponseMessage response = client.GetAsync("api/message/messagedusers/").Result;
            var result = response.Content.ReadAsStringAsync().Result;
            IEnumerable<MessagedUser> users = JsonConvert.DeserializeObject<IEnumerable<MessagedUser>>(result);
            return PartialView(users.GroupBy(e => e.UserName));
        }

        [HttpPost]
        public PartialViewResult GetMessages(string userName)
        {
            
            if (userName!=null) { 
                ApiClient _client = new ApiClient();
                HttpClient client = _client.GetApiClient();
                client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "Bearer " + Session["Token"].ToString());

                HttpResponseMessage response = client.GetAsync("api/message/getusermessage/" + userName).Result;
                var result = response.Content.ReadAsStringAsync().Result;
                IEnumerable<Message> messages = JsonConvert.DeserializeObject<IEnumerable<Message>>(result);
                return PartialView(messages);
            }
            return PartialView();
        }
    }
}