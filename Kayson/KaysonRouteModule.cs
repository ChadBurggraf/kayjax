using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Caching;
using Kayson.Configuration;

namespace Kayson
{
    /// <summary>
    /// Implements IHttpModule to rewrite Kayson routes to the KaysonHandler.
    /// </summary>
    public class KaysonRouteModule : IHttpModule
    {
        #region Constants

        private const string ROUTE_CACHE_KEY = "Kayson.Modules.RouteModule.RouteCache";
        private const string TARGET_ITEMS_KEY = "Kayson.Modules.RouteModule.CurrentTargetRoute";
        private static readonly object locker = new object();

        #endregion

        #region IHttpModule Members

        /// <summary>
        /// Disposes of any unmanaged resources.
        /// </summary>
        public void Dispose() { }

        /// <summary>
        /// Initializes the module.
        /// </summary>
        /// <param name="application">The HttpApplication that is handling the current request.</param>
        public void Init(HttpApplication application)
        {
            application.BeginRequest += new EventHandler(application_BeginRequest);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the target route currently being handled.
        /// </summary>
        public static string CurrentTargetRoute
        {
            get
            {
                string route = null;

                if (HttpContext.Current != null)
                {
                    route = (string)(HttpContext.Current.Items[TARGET_ITEMS_KEY] ?? String.Empty);
                }

                return route ?? String.Empty;
            }
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Gets a Kayson route for a request.
        /// </summary>
        /// <param name="context">The request to get the route for.</param>
        /// <returns>The target of a Kayson route or null if none was found.</returns>
        protected virtual string GetRoute(HttpContext context)
        {
            string routesTo = null;
            string virtualUrl = context.Request.RawUrl.Substring(context.Request.ApplicationPath.Length).ToLower();

            if (!virtualUrl.StartsWith("/")) 
            {
                virtualUrl = "/" + virtualUrl;
            }

            virtualUrl = "~" + virtualUrl;

            // We need to syncronize the read/update of the route cache.
            lock (locker)
            {
                // Grab or instantiate the cache.
                Dictionary<string, string> cache = (Dictionary<string, string>)(context.Cache[ROUTE_CACHE_KEY] ?? new Dictionary<string, string>());

                // If we have something, we're done.
                if (cache.ContainsKey(virtualUrl))
                {
                    routesTo = cache[virtualUrl];
                }
                else
                {
                    // Search for a matching route.
                    foreach (RouteElement route in KaysonSettings.Section.Routes)
                    {
                        Match match = Regex.Match(virtualUrl, route.Pattern, RegexOptions.IgnoreCase);

                        if (match.Success)
                        {
                            routesTo = match.Result(route.RoutesTo);

                            if (!routesTo.Contains(","))
                            {
                                routesTo += ", App_Code";
                            }

                            break;
                        }
                    }

                    // Update the cache.
                    cache[virtualUrl] = routesTo;
                    context.Cache[ROUTE_CACHE_KEY] = cache;
                }
            }

            return routesTo;
        }

        /// <summary>
        /// Rewrites tthe current request to the Kayson handler.
        /// </summary>
        /// <param name="context">The HttpContext to rewrite.</param>
        /// <param name="routesTo">The target type in the Kayson route.</param>
        protected virtual void Rewrite(HttpContext context, string routesTo)
        {
            context.Items[TARGET_ITEMS_KEY] = routesTo;
            context.RewritePath(KaysonSettings.Section.HandlerUrl);
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Raises application's BeginRequest event.
        /// </summary>
        private void application_BeginRequest(object sender, EventArgs e)
        {
            HttpContext context = ((HttpApplication)sender).Context;
            string routesTo = GetRoute(context);

            if (!String.IsNullOrEmpty(routesTo))
            {
                Rewrite(context, routesTo);
            }
        }

        #endregion
    }
}
