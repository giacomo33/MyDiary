using Microsoft.Azure.Mobile.Server;
using Microsoft.Azure.Mobile.Server.Tables;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Text;
using System.Web.Http.Controllers;

namespace MyDiary.Controllers
{
    //This is an abstract class so we can apply inheritance to scalfolded tablecontrollers<T>.
    public abstract class BaseController<TModel, TDbContext> : TableController<TModel> where TModel : class, ITableData where TDbContext : DbContext, new()
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            var context = new TDbContext();
            SetDomainManager(new EntityDomainManager<TModel>(context, Request, enableSoftDelete: true));
        }

        public void SetDomainManager(EntityDomainManager<TModel> domainManager)
        {
            DomainManager = domainManager;
        }
    }
}
