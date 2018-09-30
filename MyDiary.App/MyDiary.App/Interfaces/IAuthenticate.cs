using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MyDiary.App.Interfaces
{
    public interface IAuthenticate
    {
        //2 Methods to authenticate, one returns bool and shows message
        Task<bool> Authenticate(MobileServiceClient client, MobileServiceAuthenticationProvider provider);
        Task PlatformLoginAsync(MobileServiceClient client, MobileServiceAuthenticationProvider provider);
    }
}
