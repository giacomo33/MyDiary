using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using Microsoft.WindowsAzure.MobileServices.Sync;
using MyDiary.App.Interfaces;
using MyDiary.App.Models;
using MyDiary.App.Models.Authentication;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Plugin.Connectivity;
using Plugin.SecureStorage;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

//server control flow sample
//https://docs.microsoft.com/en-us/azure/app-service-mobile/app-service-mobile-xamarin-forms-get-started-users
//https://meyerweb.com/eric/tools/dencoder/
//https://jwt.io/
//https://docs.microsoft.com/en-us/azure/app-service-mobile/app-service-mobile-dotnet-how-to-use-client-library
//https://adrianhall.gitlab.io/develop-mobile-apps-with-csharp-and-azure/


namespace MyDiary.App.Services
{
    public partial class AzureService : IMyDiaryService
    {
        private const string localDatabaseFile = "diary.db";
        private readonly MobileServiceClient azureClient;
        private const string AzureEndpoint = "https://mydiary20180823123632.azurewebsites.net";
        //localhost endpoint for testing
        //private const string AzureEndpoint = "http://localhost:53175";

        IMobileServiceSyncTable<DiaryEntry> entriesTable;
        IMobileServiceSyncTable<User> userTable;
        private MobileServiceSQLiteStore store;
        private string LastEntryId;

        public AzureService()
        {
            azureClient = new MobileServiceClient(AzureEndpoint);
        }

        public async Task InitializeAsync()
        {
            if (azureClient?.SyncContext?.IsInitialized ?? false)
                return;

            //if (entriesTable != null)
            //    return;

            //InitialzeDatabase for path
            var path = localDatabaseFile;
            path = Path.Combine(MobileServiceClient.DefaultDatabasePath, path);

            //setup our local sqlite store and intialize our table
            store = new MobileServiceSQLiteStore(path);

            //Define Table
            store.DefineTable<DiaryEntry>();
            store.DefineTable<User>();

            try
            {
                //Initialize SyncContext
                await azureClient.SyncContext.InitializeAsync(store, new AzureSyncHandler(azureClient));

                //Get our sync table that will call out to azure
                entriesTable = azureClient.GetSyncTable<DiaryEntry>();
                userTable = azureClient.GetSyncTable<User>();

                //Clean out local table
                //await entriesTable.PurgeAsync(true);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Got exception: {0}", ex.Message);
            }


        }

        bool SetUpAzureUser()
        {
            if (CrossSecureStorage.Current.HasKey(App.UserIdKey) && CrossSecureStorage.Current.HasKey(App.TokenKey))
            {
                string userId = CrossSecureStorage.Current.GetValue(App.UserIdKey);
                string token = CrossSecureStorage.Current.GetValue(App.TokenKey);

                azureClient.CurrentUser = new MobileServiceUser(userId)
                {
                    MobileServiceAuthenticationToken = token
                };
                return true;
            }
            return false;
        }

        public async Task LoginAsync()
        {
            if (SetUpAzureUser())
            {
                //Check for an expired token
                if (!IsTokenExpired(azureClient.CurrentUser.MobileServiceAuthenticationToken))
                {
                    return;
                }

                //Expired token so refresh it
                try
                {
                    await azureClient.RefreshUserAsync();
                }
                catch (Exception)
                {
                    //failed - clear local user cache
                    await azureClient.LogoutAsync();
                }

            }

            if (App.Authenticator != null && azureClient.CurrentUser == null)
            {
                switch (App.AuthenticatorProvidor)
                {
                    case Enum.AuthenticatorProvidor.Facebook:
                        await App.Authenticator.PlatformLoginAsync(this.azureClient, MobileServiceAuthenticationProvider.Facebook);
                        break;
                    case Enum.AuthenticatorProvidor.Google:
                        await App.Authenticator.PlatformLoginAsync(this.azureClient, MobileServiceAuthenticationProvider.Google);
                        break;
                    case Enum.AuthenticatorProvidor.Microsoft:
                        await App.Authenticator.PlatformLoginAsync(this.azureClient, MobileServiceAuthenticationProvider.MicrosoftAccount);
                        break;
                    case Enum.AuthenticatorProvidor.Twitter:
                        await App.Authenticator.PlatformLoginAsync(this.azureClient, MobileServiceAuthenticationProvider.Twitter);
                        break;
                    default:
                        break;
                }
            }



            var user = azureClient.CurrentUser;
            if (user != null)
            {
                CrossSecureStorage.Current.SetValue(App.UserIdKey, user.UserId);
                CrossSecureStorage.Current.SetValue(App.TokenKey, user.MobileServiceAuthenticationToken);
                //await ViewAuthMe(user);

                switch (App.AuthenticatorProvidor)
                {
                    case Enum.AuthenticatorProvidor.Facebook:
                        await SaveAuth(user, "facebook");
                        break;
                    case Enum.AuthenticatorProvidor.Google:
                        await SaveAuth(user, "google");
                        break;
                    case Enum.AuthenticatorProvidor.Microsoft:
                        await SaveAuth(user, "microsoft");
                        break;
                    case Enum.AuthenticatorProvidor.Twitter:
                        await SaveAuth(user, "twitter");
                        break;
                    default:
                        break;
                }
            }


        }

        async Task SaveAuth(MobileServiceUser user, string authType)
        {
            try
            {

                var client = new HttpClient();
                client.DefaultRequestHeaders.Add("ZUMO-API-VERSION", "2.0.0");
                client.DefaultRequestHeaders.Add("X-ZUMO-AUTH", user.MobileServiceAuthenticationToken);

                var mResponse = await client.GetAsync(AzureEndpoint + "/api/whoami/" + authType);
                var content = await mResponse.Content.ReadAsStringAsync();
                var newUser = new User { UserId = user.UserId };
                MicrosoftAuthRootObject rootObject = JsonConvert.DeserializeObject<MicrosoftAuthRootObject>(content);
                foreach (MicrosoftAuth userClaims in rootObject.UserClaims)
                {
                    if (userClaims.Type.Contains("givenname"))
                    {
                        newUser.FirstName = userClaims.Value;
                    }
                    if (userClaims.Type.Contains("surname"))
                    {
                        newUser.LastName = userClaims.Value;
                    }
                    if (userClaims.Type.Contains("emailaddress"))
                    {
                        newUser.Email = userClaims.Value;
                    }
                    newUser.AuthenticationProvidor = App.AuthenticatorProvidor.ToString();
                }
                await userTable.InsertAsync(newUser);
            }
            catch (Exception ex)
            {
                throw;
            }
            await InitializeAsync();
            await SynchronizeEntriesAsync();
           
        }

        async Task ViewAuthMe(MobileServiceUser user)
        {
            try
            {

                var client = new HttpClient();
                client.DefaultRequestHeaders.Add("ZUMO-API-VERSION", "2.0.0");
                client.DefaultRequestHeaders.Add("X-ZUMO-AUTH", user.MobileServiceAuthenticationToken);

                var mResponse = await client.GetAsync(AzureEndpoint + "/.auth/me");
                var content = await mResponse.Content.ReadAsStringAsync();
                var newUser = new User();
                List<AuthMeRootObject> rootObject = JsonConvert.DeserializeObject<List<AuthMeRootObject>>(content);
                foreach (AuthMe userClaims in rootObject[0].user_claims)
                {
                    if (userClaims.typ.Contains("givenname"))
                    {
                        newUser.FirstName = userClaims.val;
                    }
                    if (userClaims.typ.Contains("surname"))
                    {
                        newUser.LastName = userClaims.val;
                    }
                    if (userClaims.typ.Contains("email"))
                    {
                        newUser.Email = userClaims.val;
                    }
                }

            }
            catch (Exception)
            {

                throw;
            }

            await userTable.PullAsync(null, userTable.CreateQuery().Where(u => u.UserId == azureClient.CurrentUser.UserId));

        }

        async Task SaveUserDetails(MobileServiceUser user)
        {
            try
            {

                var client = new HttpClient();
                client.DefaultRequestHeaders.Add("ZUMO-API-VERSION", "2.0.0");
                client.DefaultRequestHeaders.Add("X-ZUMO-AUTH", user.MobileServiceAuthenticationToken);

                var response = await client.GetAsync(AzureEndpoint + "/api/user/SaveUserInformation");
                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadAsStringAsync().Result;
                }
            }
            catch (Exception)
            {

                throw;
            }

            await userTable.PullAsync(null, userTable.CreateQuery().Where(u => u.UserId == azureClient.CurrentUser.UserId));

        }

        /// <summary>
        /// Check to see whether a JWT token has expired.
        /// See original code from https://github.com/jwt-dotnet/jwt/blob/master/src/JWT/JWT.cs
        /// </summary>
        /// <param name="token">Encoded JWT token</param>
        /// <returns>True/False</returns>
        private bool IsTokenExpired(string token)
        {
            // No token == expired.
            if (string.IsNullOrEmpty(token))
                return true;

            // Split the string apart; we want the JSON payload.
            string[] parts = token.Split('.');
            if (parts.Length != 3)
                throw new ArgumentException("Token must consist from 3 delimited by dot parts.");

            string jwt = parts[1]
                .Replace('-', '+')  // 62nd char of encoding
                .Replace('_', '/'); // 63rd char of encoding
            switch (jwt.Length % 4) // Pad with trailing '='s
            {
                case 0: break; // No pad chars in this case
                case 2: jwt += "=="; break; // Two pad chars
                case 3: jwt += "="; break;  // One pad char
                default:
                    throw new ArgumentException("Token is not a valid Base64 string.");
            }

            // Convert to a JSON string (std. Base64 decode)
            string json = Encoding.UTF8.GetString(Convert.FromBase64String(jwt));

            // Get the expiration date from the JSON object.
            var jsonObj = JObject.Parse(json);
            var exp = Convert.ToDouble(jsonObj["exp"].ToString());

            // JWT expiration is an offset from 1/1/1970 UTC
            var expire = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(exp);
            return expire < DateTime.UtcNow;
        }

        /// <summary>
        ///     Method to synchronize our local DB store with the Azure remote DB.
        /// </summary>
        /// <returns></returns>
        private async Task SynchronizeEntriesAsync(bool reSyncAllEntries = false)
        {
            if (!CrossConnectivity.Current.IsConnected)
                return;
            try
            {
                try
                {
                    SetUpAzureUser();
                    await azureClient.SyncContext.PushAsync();
                    if (reSyncAllEntries)
                    {
                        await entriesTable.PullAsync(null, entriesTable.CreateQuery());
                        await entriesTable.PullAsync(null, userTable.CreateQuery());
                    }
                    else
                    {
                        await entriesTable.PullAsync("allEntries", entriesTable.CreateQuery());
                        await entriesTable.PullAsync("allUsers", userTable.CreateQuery());
                    }
                }
                catch (MobileServicePushFailedException ex)
                {
                    if (ex.PushResult.Status == MobileServicePushStatus.CancelledByAuthenticationError)
                    {
                        await LoginAsync();
                        await SynchronizeEntriesAsync();
                        return;
                    }

                    if (ex.PushResult != null)
                        foreach (var result in ex.PushResult.Errors)
                            await ResolveErrorAsync(result);
                }
                catch (MobileServiceInvalidOperationException ex)
                {
                    if (ex.Response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        await LoginAsync();
                        await SynchronizeEntriesAsync();
                        return;
                    }

                    throw;
                }
            }
            catch (Exception ex)
            {
                

                throw;
            }
        }

        async Task SynchronizeEntryAsync(string Id)
        {
            if (!CrossConnectivity.Current.IsConnected)
                return;

            try
            {
                try
                {

                }
                catch (MobileServicePushFailedException ex)
                {
                    if (ex.PushResult.Status == MobileServicePushStatus.CancelledByAuthenticationError)
                    {
                        await LoginAsync();
                        await SynchronizeEntryAsync(Id);
                        return;
                    }

                    if (ex.PushResult != null)
                    {
                        foreach (var result in ex.PushResult.Errors)
                        {
                            await ResolveErrorAsync(result);
                        }
                        await azureClient.SyncContext.PushAsync();
                    }
                }
                await entriesTable.PullAsync("syncEntry" + Id, entriesTable
                    .Where(e => e.Id == Id));
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Got exception: {0}", ex.Message);
            }
        }

        private async Task ResolveErrorAsync(MobileServiceTableOperationError error)
        {
            //if (result.Result == null || result.Item == null)
            //    return;

            //var serverItem = result.Result.ToObject<DiaryEntry>();
            //var localItem = result.Item.ToObject<DiaryEntry>();

            //if (serverItem.Id == localItem.Id
            //    && serverItem.Title ==localItem.Title)
            //{
            //    await result.CancelAndDiscardItemAsync();
            //}
            //else
            //{
            //    localItem.AzureVersion = serverItem.AzureVersion;
            //    await result.UpdateOperationAsync(JObject.FromObject(localItem));
            //}

            if (error.OperationKind == MobileServiceTableOperationKind.Update && error.Result != null)
            {
                //Update failed, reverting to server's copy.
                await error.CancelAndUpdateItemAsync(error.Result);
            }
            else
            {
                // Discard local change.
                await error.CancelAndDiscardItemAsync();
            }

            Debug.WriteLine(@"Error executing sync operation. Item: {0} ({1}). Operation discarded.", error.TableName, error.Item["id"]);
        }

        public async Task<IEnumerable<DiaryEntry>> GetAllEntry()
        {
            await InitializeAsync();
            await SynchronizeEntriesAsync();

            return await entriesTable.ToEnumerableAsync();
        }


        public async Task PostEntry(DiaryEntry entry)
        {
            await InitializeAsync();
            await entriesTable.InsertAsync(entry);
            await SynchronizeEntriesAsync();
        }


        public async Task PatchEntry(string Id, DiaryEntry entry)
        {
            await InitializeAsync();
            if (!string.IsNullOrEmpty(entry.Id))
            {
                await this.entriesTable.UpdateAsync(entry);
                await SynchronizeEntriesAsync();
                return;
            }
            throw new ArgumentException("entry id is missing");
        }

        public async Task<DiaryEntry> GetEntry(string Id)
        {
            await InitializeAsync();
            if (LastEntryId != Id)
            {
                await SynchronizeEntryAsync(Id);
                LastEntryId = Id;
            }

            return (await entriesTable.Where(e => e.Id == Id)
                    .ToEnumerableAsync())
                    .FirstOrDefault();
        }

        public async Task DeleteEntry(DiaryEntry entry)
        {
            await InitializeAsync();
            await entriesTable.DeleteAsync(entry);
            await SynchronizeEntriesAsync();
        }

        public async Task LogOffAsync()
        {
            // Delete the local cache
            CrossSecureStorage.Current.DeleteKey(App.UserIdKey);
            CrossSecureStorage.Current.DeleteKey(App.TokenKey);

            if (azureClient.CurrentUser == null)
                return;

            // Throw away token cache on server
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("X-ZUMO-AUTH", azureClient.CurrentUser.MobileServiceAuthenticationToken);
                await client.GetAsync(azureClient.MobileAppUri + "/.auth/logout");
            }

            // Throw away token in client library

            await azureClient.LogoutAsync();
            Debug.Assert(azureClient.CurrentUser == null);

            // Finally, purge the local data.
            await entriesTable.PurgeAsync(true);

            if (azureClient.CurrentUser != null)
                azureClient.CurrentUser = null;
        }

        public async Task ReSyncAllEntires()
        {
            await SynchronizeEntriesAsync(true);
        }

        public async Task<User> GetUser()
        {
            SetUpAzureUser();
            await InitializeAsync();
            return (await userTable.Where(u => u.UserId == azureClient.CurrentUser.UserId)
                .ToEnumerableAsync())
                .FirstOrDefault();
        }

        public async Task<IEnumerable<DiaryEntry>> Search(string searchText, object cancellationToken)
        {
            await InitializeAsync();
            await SynchronizeEntriesAsync();

            return await entriesTable.Where(e => e.Title.Contains(searchText) ||
e.Description.Contains(searchText)).ToEnumerableAsync();
        }
    }
}
