using Newtonsoft.Json;
using OfflineMessaging.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Security;

namespace OfflineMessaging.Web.Controllers
{
    public class AccountController : Controller
    {


        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<JsonResult> Login(LoginModel model)
        {
            LoginStatus status = new LoginStatus();
            if (ModelState.IsValid)
            {
                ApiClient _client = new ApiClient();
                HttpClient client = _client.GetApiClient();
               
                HttpContent content = new StringContent(string.Format("grant_type=password&username={0}&password={1}",
            HttpUtility.UrlEncode(model.Username),
            HttpUtility.UrlEncode(model.Password)), Encoding.UTF8,
            "application/x-www-form-urlencoded");

                HttpResponseMessage response = await client.PostAsync("oauth/token", content);
                var result= response.Content.ReadAsStringAsync().Result;

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    status.Success = true;
                    status.TargetURL = Request.Url.Authority + "/Account/Home";
                    status.Message = "Login Successful";
                    TokenModel tokenModel = JsonConvert.DeserializeObject<TokenModel>(result);
                    Session["Token"] = tokenModel.access_token;
                    Session["Username"] = model.Username;
                    FormsAuthentication.SetAuthCookie(model.Username, true);
                    
                }
                else
                {
                    status.Message = "Invalid UserID or Password!";
                    status.Success = false;
                    
                }
                    
            }
            else {
                status.Success = false;
                status.Message = "Model is Invalid!";   
            }
            return Json(status);

        }


        public async Task<ActionResult> Home()
        {
            ApiClient _client = new ApiClient();
            HttpClient client = _client.GetApiClient();
            client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", Session["Token"].ToString());

            HttpResponseMessage response = await client.GetAsync("api/accounts/user/"+Session["Username"]);
            var result = response.Content.ReadAsStringAsync().Result;
            User user=JsonConvert.DeserializeObject<User>(result);
            return View("Home",user);
        }
    }
}