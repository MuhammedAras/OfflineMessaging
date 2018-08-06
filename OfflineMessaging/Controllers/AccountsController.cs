using System;
using System.Linq;
using System.Web.Http;
using System.Threading.Tasks;
using OfflineMessaging.Models;
using OfflineMessaging.Infrastructure;
using Microsoft.AspNet.Identity;
using System.Net.Http;
using System.Web.Script.Serialization;

using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Collections.Generic;
using System.Web;

namespace OfflineMessaging.Controllers
{
    [RoutePrefix("api/accounts")]
    public class AccountsController : BaseApiController
    {
        HttpRequest request;

        [Authorize]
        [Route("users")]
        public IHttpActionResult GetUsers()
        {
            return Ok(this.AppUserManager.Users.ToList().Select(u => this.TheModelFactory.Create(u)));
        }

        [Authorize]
        [Route("user/{id:guid}", Name = "GetUserById")]
        public async Task<IHttpActionResult> GetUser(string Id)
        {
            var user = await this.AppUserManager.FindByIdAsync(Id);
            if (user != null)
            {
                return Ok(this.TheModelFactory.Create(user));
            }
            return NotFound();
        }

        [Authorize]
        [Route("user/{username}")]
        public async Task<IHttpActionResult> GetUserNameByName(string username)
        {
            var user = await AppUserManager.FindByNameAsync(username);
            if (user != null)
            {
                return Ok(this.TheModelFactory.Create(user));
            }
            return NotFound();
        }

        [AllowAnonymous]
        [Route("create")]
        public async Task<IHttpActionResult> CreateUser(CreateUserBindingModel createUserModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var user = new ApplicationUser()
            {
                UserName = createUserModel.Username,
                Email = createUserModel.Email,
                FirstName = createUserModel.FirstName,
                LastName = createUserModel.LastName,
                Level = 3,
                JoinDate = DateTime.Now.Date,
            };
            IdentityResult addUserResult = await this.AppUserManager.CreateAsync(user, createUserModel.Password);
            if (!addUserResult.Succeeded)
            {
                return GetErrorResult(addUserResult);
            }

            //string code = await this.AppUserManager.GenerateEmailConfirmationTokenAsync(user.Id);
            //var callbackUrl = new Uri(Url.Link("ConfirmEmailRoute",new { userId=user.Id,code=code}));
            //await this.AppUserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking < a href =\"" + callbackUrl + "\">here</a>");
            Uri locationHeader = new Uri(Url.Link("GetUserById", new { id = user.Id }));

            return Created(locationHeader, TheModelFactory.Create(user));
        }

        

        [Authorize]
        [Route("changepassword")]
        public async Task<IHttpActionResult> ChangePassword(ChangePasswordBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            IdentityResult result = await this.AppUserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }
            return Ok();
        }


        [Authorize]
        [Route("user/{id:guid}")]
        public async Task<IHttpActionResult> DeleteUser(string id)
        {
            var appUser = await this.AppUserManager.FindByIdAsync(id);
            if (appUser != null)
            {
                IdentityResult result = await this.AppUserManager.DeleteAsync(appUser);
                if (!result.Succeeded)
                {
                    return GetErrorResult(result);
                }
                return Ok();
            }
            return NotFound();
        }

        [AllowAnonymous]
        [Route("login")]
        public async Task<IHttpActionResult> Login(LoginBindingModel LoginModel)
        {
            var ctx = ApplicationDbContext.Create();

            if (string.IsNullOrWhiteSpace(LoginModel.username) || string.IsNullOrWhiteSpace(LoginModel.password))
            {
                return NotFound();
            }

            var user = await AppUserManager.FindByNameAsync(LoginModel.username);
            if (user == null)
            {
                return NotFound();
            }

            var passwordSignInResult = await AppUserManager.CheckPasswordAsync(user, LoginModel.password);
            if (!passwordSignInResult)
            {

                ctx.Login_Logs.Add(new Login_Logs()
                {
                    Ip_Address = request.UserHostAddress,
                    Status = NotFound().ToString(),
                    Time=DateTime.Now,
                    UserId=user.Id,
                    User=user
                });
                await ctx.SaveChangesAsync();

                return NotFound();
            }
            
            //Get Token from ~//oauth/token
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri (Startup.address);
            client.DefaultRequestHeaders
              .Accept
              .Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");


            var json = new JavaScriptSerializer().Serialize(LoginModel);

            HttpResponseMessage response = await client.PostAsJsonAsync("/oauth/token",json);
            
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                ctx.Login_Logs.Add(new Login_Logs()
                {
                    Ip_Address = request.UserHostAddress,
                    Status = Ok().ToString(),
                    Time = DateTime.Now,
                    UserId = user.Id,
                    User = user
                });
                await ctx.SaveChangesAsync();
                return Ok(response.Content);
            }
            else
                return BadRequest(response.ReasonPhrase);   
        }

        [Authorize]
        [Route("block/{userName}")]
        public async Task<IHttpActionResult> Block(string userName)
        {
            var activeUser = User.Identity.GetUserId();
            var BlockedUser = await this.AppUserManager.FindByNameAsync(userName);
            if (BlockedUser == null)
            {
                return NotFound();
            }

            using (var ctx = ApplicationDbContext.Create())
            {
                ctx.Blocks.Add(
                    new Block()
                    {
                        userId = activeUser,
                        BlockedUserId = BlockedUser.Id,
                        BlockedTime = DateTime.Now
                    }
                );
                await ctx.SaveChangesAsync();
            }
            return Ok(userName + " is Blocked!");
        }

        [Authorize]
        [Route("getblockedlist")]
        public async Task<IHttpActionResult> GetBlockedList()
        {
            var userId = User.Identity.GetUserId();

            using (var ctx = ApplicationDbContext.Create())
            {
                var BlockedList = ctx.Blocks
                                  .Where(e => e.userId == userId).ToList();
                return Ok(BlockedList);
            }
        }

        [Authorize]
        [Route("block/{userName}")]
        [HttpDelete]
        public async Task<IHttpActionResult> RemoveBlock(string userName)
        {
            var activeUser = User.Identity.GetUserId();
            var BlockedUser = await this.AppUserManager.FindByNameAsync(userName);
            if (BlockedUser != null)
            {
                using (var ctx = ApplicationDbContext.Create())
                {
                    var query = ctx.Blocks
                        .Where(e => e.userId == activeUser && BlockedUser.Id==e.BlockedUserId).SingleOrDefault();
                    ctx.Blocks.Remove(query);
                   await ctx.SaveChangesAsync();

                    return Ok("Block Removed!");
                }
            }
            return NotFound();
        }


        /*
        [AllowAnonymous]
        [HttpGet]
        [Route("ConfirmEmail", Name = "ConfirmEmailRoute")]
        public async Task<IHttpActionResult> ConfirmEmail(string userId = "", string code = "")
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(code))
            {
                ModelState.AddModelError("", "User Id and Code are required");
                return BadRequest(ModelState);
            }

            IdentityResult result = await this.AppUserManager.ConfirmEmailAsync(userId, code);

            if (result.Succeeded)
            {
                return Ok();
            }
            else
            {
                return GetErrorResult(ModelState);
            }
            
        }*/

    }
}