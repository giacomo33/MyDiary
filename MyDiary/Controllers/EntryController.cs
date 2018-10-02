using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
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
    public class EntryController : TableController<Entry>
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            MobileServiceContext context = new MobileServiceContext();
            DomainManager = new EntityDomainManager<Entry>(context, Request, enableSoftDelete: true);
        }

        // GET tables/Entry
        public IQueryable<Entry> GetAllDiaryEntries()
        {
            string userId = GetUserId();
            return Query()
                .Where(e => e.UserId == userId)
                .OrderByDescending(e => e.CreatedAt);
        }

        // GET tables/Entry/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task<Entry> GetDiaryEntry(string id)
        {
            string userId = GetUserId();
            return Query().SingleOrDefaultAsync(e => e.UserId == userId && e.Id == id);
        }

        // PATCH tables/Entry/48D68C86-6EA6-4C25-AA33-223FC9A27959
        [HttpPatch]
        public Task<Entry> UpdateDiaryEntry(string id, Delta<Entry> patch)
        {
            return UpdateAsync(id, patch);
        }

        // POST tables/Entry
        [HttpPost]
        public async Task<IHttpActionResult> InsertDiaryEntry(Entry item)
        {
            string userId = GetUserId();
            if (userId != item.UserId)
                return this.Unauthorized();

            Entry current = await InsertAsync(item);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        // DELETE tables/Entry/48D68C86-6EA6-4C25-AA33-223FC9A27959
        [HttpDelete]
        public Task DeleteDiaryEntry(string id)
        {
            var item = Lookup(id);
            var diaryEntry = item.Queryable.SingleOrDefault();
            if (diaryEntry?.UserId == GetUserId())
                return DeleteAsync(id);

            throw new HttpResponseException(new HttpResponseMessage { StatusCode = HttpStatusCode.BadRequest, ReasonPhrase = "Id does not exist!" });
    }

        //Get UserId method.
        [NonAction]
        public string GetUserId()
        {
                if (User == null)
                {
                    throw new HttpResponseException(HttpStatusCode.Unauthorized);
                }
                var user = this.User as ClaimsPrincipal;
                Claim nameClaim = user?.FindFirst(
                    c => c.Type == ClaimTypes.NameIdentifier);
                return nameClaim?.Value;
        }
    }
}
