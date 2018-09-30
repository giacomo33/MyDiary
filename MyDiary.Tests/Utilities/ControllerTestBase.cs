using Microsoft.Azure.Mobile.Server;
using Microsoft.Azure.Mobile.Server.Tables;
using Moq;
using MyDiary.Controllers;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace MyDiary.Tests.Utilities
{
    public abstract class ControllerTestBase<TController, TModel, TDbContext> 
        where TController : BaseController<TModel, TDbContext>, new()
        where TModel : class, ITableData
        where TDbContext : DbContext, new()
    {
        protected readonly TController Controller;

        protected ControllerTestBase()
        {
            Controller = new TController();
            Controller.Configuration = new HttpConfiguration();
            Controller.Request = new HttpRequestMessage();
        }

        protected void SetUpDomainManager(DbContext dbContext)
        {
            Controller.SetDomainManager(new EntityDomainManager<TModel>(dbContext, Controller.Request));
        }
    }
}
