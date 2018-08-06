using OfflineMessaging.Infrastructure;
using OfflineMessaging.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Routing;

namespace OfflineMessaging.Controllers
{
    [RoutePrefix("api/message")]
    public class MessageController : BaseApiController
    {
        

        [Authorize]
        [Route("sentmessage")]
        public async Task<IHttpActionResult> SentMessage(MessageBindingModel messageModel)
        {
            using (var ctx = ApplicationDbContext.Create()) { 
                var ReceiverUser = await AppUserManager.FindByNameAsync(messageModel.ReceiverUserName);
                var Sender = await AppUserManager.FindByIdAsync(User.Identity.GetUserId());

                var message = new Message() {
                    ReceiverId = ReceiverUser.Id,
                    SenderId = User.Identity.GetUserId(),
                    Content= messageModel.Message,
                    Time=DateTime.Now
                    
                };
                ctx.Messages.Add(message);
                await ctx.SaveChangesAsync();
                return Ok();
            }   
        }

        [Authorize]
        [Route("getallmessage")]
        public async Task< IHttpActionResult > GetAllMessage(int page = 0, int pageSize = 10) {

            using (var ctx = ApplicationDbContext.Create())
            {
                var userId = User.Identity.GetUserId();

                var BlockedList = ctx.Blocks
                                  .Where(e => e.userId == userId);
                                                  
                //Just nonblocked users messages
                IQueryable<Message> query;
                query = ctx.Messages
                    .Where(s => !(BlockedList.Any(a => a.BlockedUserId == s.SenderId)) && (s.SenderId == userId || s.ReceiverId == userId) )
                    .OrderByDescending(c => c.Time).AsQueryable();

                var totalCount = query.Count();
                var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

                var urlHelper = new UrlHelper(Request);
                //var prevLink = page > 0 ? urlHelper.Link("message/getallmessage", new { page = page - 1, pageSize = pageSize }) : "";
                //var nextLink = page < totalPages - 1 ? urlHelper.Link("message/getallmessage", new { page = page + 1, pageSize = pageSize }) : "";

                var paginationHeader = new
                {
                    TotalCount = totalCount,
                    TotalPages = totalPages,
                   // PrevPageLink = prevLink,
                    //NextPageLink = nextLink
                };

                System.Web.HttpContext.Current.Response.Headers.Add("X-Pagination",
                Newtonsoft.Json.JsonConvert.SerializeObject(paginationHeader));

                var results = query
                              .Skip(pageSize * page)
                              .Take(pageSize)
                              .ToList();

                return Ok(results.Select(s => this.TheModelFactory.Create(s)));
            }
        }

        [Authorize]
        [Route("getusermessage/{userName}")]
        [HttpGet]
        public async Task<IHttpActionResult> GetUserMessage(string userName)
        {
            //Active User's userId
            var userId = User.Identity.GetUserId();

            using (var ctx = ApplicationDbContext.Create())
            {
                var friend = await AppUserManager.FindByNameAsync(userName);
                if (friend == null)
                {
                    return NotFound();
                }

                var BlockedList = ctx.Blocks
                                   .Where(e => e.userId == userId);

                //Just nonblocked users messages

                IQueryable<Message> allMessages = ctx.Messages
                            .Where(s => BlockedList.Any(a => a.BlockedUserId != s.SenderId) && (s.SenderId == userId || s.ReceiverId == userId))
                            .OrderByDescending(c => c.Time).AsQueryable();

                var query = allMessages
                            .Where(c => c.ReceiverId == friend.Id || c.SenderId == friend.Id)
                            .ToList();

                return Ok(query.Select(c => this.TheModelFactory.Create(c)));
            }
        }
    }
}
