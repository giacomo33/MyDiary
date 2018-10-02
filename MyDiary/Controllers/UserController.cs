using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using Microsoft.Azure.Mobile.Server;
using MyDiary.DataObjects;
using MyDiary.Models;


namespace MyDiary.Controllers
{
    [Authorize]
    public class UserController : TableController<User>
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            MobileServiceContext context = new MobileServiceContext();
            DomainManager = new EntityDomainManager<User>(context, Request);
        }

        //Save UserInformation
        //GET tables/user
        [Route("api/user/SaveUserInformation")]
        [HttpGet]
        public async Task<IHttpActionResult> SaveUserInformation()
        {
            var user = this.User as ClaimsPrincipal;
            string UserId = user?.FindFirst(
                c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (UserId !=null && Lookup(UserId) != null)
            {
                var newUser = new User
                {
                    FirstName = user?.FindFirst(c => c.Type == ClaimTypes.GivenName).Value,
                    LastName = user?.FindFirst(c => c.Type == ClaimTypes.Surname).Value,
                    Email = user?.FindFirst(c => c.Type == ClaimTypes.Email).Value,
                };

                User current = await InsertAsync(newUser);
                return CreatedAtRoute("Tables", new { id = current.Id }, current);
            }
            else
                return Ok();
        }

        // GET tables/User
        public IQueryable<User> GetAllUser()
        {
            return Query();
        }

        // GET tables/User/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public SingleResult<User> GetUser(string id)
        {
            return Lookup(id);
        }

        // PATCH tables/User/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task<User> PatchUser(string id, Delta<User> patch)
        {
            return UpdateAsync(id, patch);
        }

        // POST tables/User
        public async Task<IHttpActionResult> PostUser(User item)
        {
            User current = await InsertAsync(item);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        // DELETE tables/User/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeleteUser(string id)
        {
            return DeleteAsync(id);
        }
    }
}
