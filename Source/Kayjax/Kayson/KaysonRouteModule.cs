

namespace Kayson
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Text.RegularExpressions;
    using System.Web;
    using System.Web.Caching;
    using Kayson.Configuration;

    /// <summary>
    /// Implements IHttpModule to rewrite Kayson routes to the KaysonHandler.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "Spelling is correct.")]
    public class KaysonRouteModule : IHttpModule
    {
        #region Constants

        private const string RouteCacheKey = "Kayson.Modules.RouteModule.RouteCache";
        private const string TargetItemsKey = "Kayson.Modules.RouteModule.CurrentTargetRoute";
        private static readonly object locker = new object();

        #endregion

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
                    route = (string)(HttpContext.Current.Items[TargetItemsKey] ?? String.Empty);
                }

                return route ?? String.Empty;
            }
        }

        /// <summary>
        /// Disposes of any unmanaged resources.
        /// </summary>
        public void Dispose() { }

        /// <summary>
        /// Gets a Kayson route for a request.
        /// </summary>
        /// <param name="context">The request to get the route for.</param>
        /// <returns>The target of a Kayson route or null if none was found.</returns>
        protected virtual string GetRoute(HttpContext context)
        {
            string routesTo = null;
            string virtualUrl = context.Request.RawUrl.Substring(context.Request.ApplicationPath.Length).ToUpperInvariant();

            if (!virtualUrl.StartsWith("/", StringComparison.Ordinal)) 
            {
                virtualUrl = "/" + virtualUrl;
            }

            virtualUrl = "~" + virtualUrl;

            // We need to syncronize the read/update of the route cache.
            lock (locker)
            {
                // Grab or instantiate the cache.
                Dictionary<string, string> cache = (Dictionary<string, string>)(context.Cache[RouteCacheKey] ?? new Dictionary<string, string>());

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
                    context.Cache[RouteCacheKey] = cache;
                }
            }

            return routesTo;
        }

        /// <summary>
        /// Initializes the module.
        /// </summary>
        /// <param name="context">The HttpApplication that is handling the current request.</param>
        public void Init(HttpApplication context)
        {
            context.BeginRequest += new EventHandler(ContextBeginRequest);
        }

        /// <summary>
        /// Rewrites tthe current request to the Kayson handler.
        /// </summary>
        /// <param name="context">The HttpContext to rewrite.</param>
        /// <param name="routesTo">The target type in the Kayson route.</param>
        protected virtual void Rewrite(HttpContext context, string routesTo)
        {
            context.Items[TargetItemsKey] = routesTo;
            context.RewritePath(KaysonSettings.Section.HandlerUrl);
        }

        /// <summary>
        /// Raises application's BeginRequest event.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void ContextBeginRequest(object sender, EventArgs e)
        {
            HttpContext context = ((HttpApplication)sender).Context;
            string routesTo = GetRoute(context);

            if (!String.IsNullOrEmpty(routesTo))
            {
                Rewrite(context, routesTo);
            }
        }
    }
}
