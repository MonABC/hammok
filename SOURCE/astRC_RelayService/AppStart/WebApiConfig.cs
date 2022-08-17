using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Hammock.AssetView.Platinum.Tools.RC.RelayService.Common;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.Web.Http.Dispatcher;

namespace Hammock.AssetView.Platinum.Tools.RC.RelayService.AppStart
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration configuration)
        {
            configuration.Formatters.JsonFormatter.SerializerSettings.Converters.Add(new IsoDateTimeConverter() { DateTimeFormat = "yyyy/MM/ddTHH:mm:ssZ" });
            configuration.Formatters.JsonFormatter.SerializerSettings.Converters.Add(new StringEnumConverter());

            // Version 1
            {
                // Client
                {
                    configuration.Routes.MapHttpRoute(
                        name: "ClientApi",
                        routeTemplate: "api/v1/client/{controller}/{action}/{id}",
                        defaults: new { id = RouteParameter.Optional, action = RouteParameter.Optional },
                        dataTokens: new
                        {
                            Namespaces = new[] { typeof(Controllers.V1.Client.SessionsController).Namespace }
                        });
                }

                // Server
                {
                    configuration.Routes.MapHttpRoute(
                        name: "ServerApi",
                        routeTemplate: "api/v1/server/{controller}/{action}/{id}",
                        defaults: new { id = RouteParameter.Optional, action = RouteParameter.Optional },
                        dataTokens: new
                        {
                            Namespaces = new[] { typeof(Controllers.V1.Server.SessionsController).Namespace }
                        });
                }
            }

            configuration.Services.Replace(typeof(IHttpControllerSelector), new NamespaceHttpControllerSelector(configuration));
        }
    }
}
