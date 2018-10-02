using Microsoft.Azure.Mobile.Server;
using Microsoft.Azure.Mobile.Server.Tables;
using MyDiary.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Text;
using System.Web.Http.Controllers;

namespace MyDiary.Controllers
{
    //This is an abstract class so we can apply inheritance to scalfolded tablecontrollers<T>.
    public abstract class BaseController<TModel> : TableController<TModel> where TModel : class, ITableData
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            MobileServiceContext context = new MobileServiceContext();
            SetDomainManager(new EntityDomainManager<TModel>(context, Request));
        }

        public void SetDomainManager(EntityDomainManager<TModel> domainManager)
        {
            DomainManager = domainManager;
        }
    }
}
