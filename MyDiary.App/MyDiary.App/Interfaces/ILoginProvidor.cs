using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MyDiary.App.Interfaces
{
    public interface ILoginProvidor
    {
        Task LoginAsync(MobileServiceClient client, MobileServiceAuthenticationProvider provider);
    }
}
