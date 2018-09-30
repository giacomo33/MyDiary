using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.Sync;
using MyDiary.App.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace MyDiary.App.Services
{
   public class AzureSyncHandler :   IMobileServiceSyncHandler
    {
        MobileServiceClient client;

        public AzureSyncHandler(MobileServiceClient client)
        {
            this.client = client;
        }

        /// handle conflicts one at a time, and retry the operations as the conflict handling choice is made
        public virtual async Task<JObject> ExecuteTableOperationAsync(IMobileServiceTableOperation operation)
        {
            Func<Task<JObject>> tryOperation = operation.ExecuteAsync;

            do
            {
                try
                {
                    JObject result = await tryOperation();
                    return result;
                }
                catch (Exception ex) when (ex is MobileServiceConflictException || ex is MobileServicePreconditionFailedException)
                {
                    var error = (MobileServiceInvalidOperationException)ex;
                    var localItem = operation.Item.ToObject<DiaryEntry>();
                    var serverValue = error.Value;

                    if (serverValue == null)
                    { // 409 doesn't return the server item
                        serverValue = await operation.Table.LookupAsync(localItem.Id) as JObject;
                    }

                    var serverItem = serverValue.ToObject<DiaryEntry>();

                    if (serverItem.Title == localItem.Title && serverItem.Description == localItem.Description)
                    {
                        // items are same so we can ignore the conflict
                        return serverValue;
                    }

                    var userAction = await App.Current.MainPage.DisplayAlert(
                        "Conflict", $"Local version: {localItem}\nServer version: {serverItem}", "Use server", "Use client");

                    if (userAction)
                    {
                        return serverValue;
                    }
                    else
                    {
                        // Overwrite the server version and try the operation again by continuing the loop
                        operation.Item[MobileServiceSystemColumns.Version] = serverValue[MobileServiceSystemColumns.Version];

                        if (error is MobileServiceConflictException)
                        {
                            // change operation from Insert to Update
                            tryOperation = async () => await operation.Table.UpdateAsync(operation.Item) as JObject;
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                    throw e;
                }
            } while (true);
        }

        /// bulk conflict handling
        public virtual Task OnPushCompleteAsync(MobileServicePushCompletionResult result)
        {
            return Task.FromResult(0);
        }

    }
}
