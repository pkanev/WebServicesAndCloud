using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(OnlineShop.Services.Startup))]

namespace OnlineShop.Services
{
    using System.Data.Entity;
    using System.Reflection;
    using System.Web.Http;
    using System.Web.Mvc;
    using Data;
    using Data.Interfaces;
    using Infrastructure;
    using Ninject;
    using Ninject.Web.Common.OwinHost;
    using Ninject.Web.WebApi.OwinHost;

    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);

            var kernel = new StandardKernel();
            kernel.Load(Assembly.GetExecutingAssembly());
            kernel.Bind<IUserIdProvider>().To<AspNetUserIdProvider>();
            kernel.Bind<IOnlineShopData>().To<OnlineShopData>();
            kernel.Bind<DbContext>().To<OnlineShopContext>();

            var httpConfig = new HttpConfiguration();
            WebApiConfig.Register(httpConfig);
            app.UseNinjectMiddleware(() => kernel)
                .UseNinjectWebApi(httpConfig);
        }
    }
}
