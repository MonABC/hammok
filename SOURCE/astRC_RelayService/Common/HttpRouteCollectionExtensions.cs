using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Routing;

namespace Hammock.AssetView.Platinum.Tools.RC.RelayService.Common
{
    public static class HttpRouteCollectionExtensions
    {
        public static IHttpRoute MapHttpRoute(this HttpRouteCollection routes, string name, string routeTemplate, object defaults, object dataTokens)
        {
            HttpRouteValueDictionary defaultsDictionary = new HttpRouteValueDictionary(defaults);
            HttpRouteValueDictionary constraintsDictionary = new HttpRouteValueDictionary();
            HttpRouteValueDictionary dataTokensDictionary = new HttpRouteValueDictionary(dataTokens);
            IHttpRoute route = routes.CreateRoute(routeTemplate, defaultsDictionary, constraintsDictionary, dataTokens: dataTokensDictionary);
            routes.Add(name, route);
            return route;
        }
    }
}
