using Microsoft.Azure.Mobile.Server.Authentication;
using Microsoft.Azure.Mobile.Server.Config;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace MyDiary.Controllers
{
    //NB. for each providor in azure you may have to specify user claims you require on that service.
    [MobileAppController]
    public class UserInformationController : ApiController
    {
        [Route("api/whoami")]
        public HttpResponseMessage Get()
        {
            var user = this.User as ClaimsPrincipal;
            if (user == null)
                return new HttpResponseMessage { Content = new StringContent("No user.") };

            try
            {
                return GetResponse(user);
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage { Content = new StringContent(ex.Message) };
            }
        }

        private HttpResponseMessage GetResponse(object fromObject)
        {
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JObject.FromObject(fromObject, new JsonSerializer { ReferenceLoopHandling = ReferenceLoopHandling.Ignore })
                    .ToString(Formatting.Indented), Encoding.UTF8, "application/json")
            };
        }

        [Route("api/whoami/twitter")]
        public async Task<HttpResponseMessage> GetTwitter()
        {
            try
            {
                var user = await User.GetAppServiceIdentityAsync<TwitterCredentials>(Request);
                return GetResponse(user);
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage { Content = new StringContent(ex.Message) };
            }
        }

        [Route("api/whoami/facebook")]
        public async Task<HttpResponseMessage> GetFacebook()
        {
            try
            {
                var user = await User.GetAppServiceIdentityAsync<FacebookCredentials>(Request);
                return GetResponse(user);
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage { Content = new StringContent(ex.Message) };
            }
        }

        [Route("api/whoami/google")]
        public async Task<HttpResponseMessage> GetGoogle()
        {
            try
            {
                var user = await User.GetAppServiceIdentityAsync<GoogleCredentials>(Request);
                return GetResponse(user);
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage { Content = new StringContent(ex.Message) };
            }
        }

        [Route("api/whoami/microsoft")]
        public async Task<HttpResponseMessage> GetMicrosoft()
        {
            try
            {
                var user = await User.GetAppServiceIdentityAsync<MicrosoftAccountCredentials>(Request);
                return GetResponse(user);
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage { Content = new StringContent(ex.Message) };
            }
        }

        [Route("api/whoami/azuread")]
        public async Task<HttpResponseMessage> GetAzureAD()
        {
            try
            {
                var user = await User.GetAppServiceIdentityAsync<AzureActiveDirectoryCredentials>(Request);
                return GetResponse(user);
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage { Content = new StringContent(ex.Message) };
            }
        }
    }
}