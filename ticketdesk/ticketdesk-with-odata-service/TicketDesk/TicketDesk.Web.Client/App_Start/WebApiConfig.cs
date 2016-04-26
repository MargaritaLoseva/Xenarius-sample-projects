using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.OData.Builder;
using System.Web.Http.OData.Extensions;
using TicketDesk.Domain.Model;
using System.Web.Http.Dependencies;
using System.Web.Http.Cors;

namespace TicketDesk.Web.Client
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            var corsAttr = new EnableCorsAttribute(origins: "*", headers: "*", methods: "*​");
            corsAttr.SupportsCredentials = true;
            config.EnableCors(corsAttr);

            ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
            builder.EntitySet<Ticket>("ODataTickets");
            builder.EntitySet<Project>("Projects");
            builder.EntitySet<TicketEvent>("TicketEvents");
            builder.EntitySet<TicketSubscriber>("TicketSubscribers");
            builder.EntitySet<TicketTag>("TicketTags");
            config.Routes.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());

            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }

    }

    public class WebApiResolver : IDependencyResolver
    {
        System.Web.Mvc.IDependencyResolver mvcResolver;
        public WebApiResolver(System.Web.Mvc.IDependencyResolver mvcResolver)
        {
            this.mvcResolver = mvcResolver;
        }

        public IDependencyScope BeginScope()
        {
            return new WebApiResolverScope(mvcResolver);
        }

        public void Dispose()
        {
            // do nothing
        }

        public object GetService(Type serviceType)
        {
            return mvcResolver.GetService(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return mvcResolver.GetServices(serviceType);
        }
    }

    public class WebApiResolverScope : IDependencyScope
    {
        System.Web.Mvc.IDependencyResolver mvcResolver;
        public WebApiResolverScope(System.Web.Mvc.IDependencyResolver mvcResolver)
        {
            this.mvcResolver = mvcResolver;
        }

        public void Dispose()
        {
            // do nothing
        }

        public object GetService(Type serviceType)
        {
            return mvcResolver.GetService(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return mvcResolver.GetServices(serviceType);
        }
    }
}
