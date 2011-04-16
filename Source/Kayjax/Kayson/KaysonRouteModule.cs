//-----------------------------------------------------------------------
// <copyright file="KaysonRouteModule.cs" company="Tasty Codes">
//     Copyright (c) 2008 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

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
        private const string RouteCacheKey = "Kayson.Modules.RouteModule.RouteCache";
        private const string TargetItemsKey = "Kayson.Modules.RouteModule.CurrentTargetRoute";
        private static readonly object locker = new object();

        /// <summary>
        /// Gets the target route for the given HTTP context.
        /// </summary>
        /// <param name="context">The HTTP context to get the target route for.</param>
        /// <returns>A target route, or null if none was found.</returns>
        public static MatchedRoute GetTargetRoute(HttpContextBase context)
        {
            if (context != null)
            {
                MatchedRoute route = context.Items[TargetItemsKey] as MatchedRoute;

                if (route != null)
                {
                    return new MatchedRoute(route);
                }
            }

            return null;
        }

        /// <summary>
        /// Disposes of any unmanaged resources.
        /// </summary>
        public void Dispose() 
        { 
        }

        /// <summary>
        /// Initializes the module.
        /// </summary>
        /// <param name="context">The HttpApplication that is handling the current request.</param>
        public void Init(HttpApplication context)
        {
            context.BeginRequest += new EventHandler(this.ContextBeginRequest);
        }

        /// <summary>
        /// Gets a Kayson route for a request.
        /// </summary>
        /// <param name="context">The request to get the route for.</param>
        /// <returns>The target of a Kayson route or null if none was found.</returns>
        protected virtual MatchedRoute GetRoute(HttpContext context)
        {
            MatchedRoute routesTo = null;
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
                Dictionary<string, MatchedRoute> cache = (Dictionary<string, MatchedRoute>)(context.Cache[RouteCacheKey] ?? new Dictionary<string, MatchedRoute>());

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
                            Type routeType = CreateTypeFromRouteString(match.Result(route.RoutesTo));

                            if (routeType != null)
                            {
                                routesTo = new MatchedRoute(
                                    routeType,
                                    CreateTypeFromRouteString(route.ReaderType),
                                    CreateTypeFromRouteString(route.WriterType));
                            }
                            else
                            {
                                throw new InvalidRequestTypeException();
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
        /// Rewrites the current request to the Kayson handler.
        /// </summary>
        /// <param name="context">The HttpContext to rewrite.</param>
        /// <param name="routesTo">The target Kayson route.</param>
        protected virtual void Rewrite(HttpContext context, MatchedRoute routesTo)
        {
            context.Items[TargetItemsKey] = routesTo;
            context.RewritePath(KaysonSettings.Section.HandlerUrl);
        }

        /// <summary>
        /// Creates a <see cref="Type"/> from a type name found in a Kayson route.
        /// </summary>
        /// <param name="typeName">The name of the type to create.</param>
        /// <returns>The created type.</returns>
        private static Type CreateTypeFromRouteString(string typeName)
        {
            Type type = null;

            if (!String.IsNullOrEmpty(typeName))
            {
                if (!typeName.Contains(","))
                {
                    typeName += ", App_Code";
                }

                type = Type.GetType(typeName, false, true);
            }

            return type;
        }

        /// <summary>
        /// Raises application's BeginRequest event.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void ContextBeginRequest(object sender, EventArgs e)
        {
            HttpContext context = ((HttpApplication)sender).Context;
            MatchedRoute routesTo = this.GetRoute(context);

            if (routesTo != null)
            {
                this.Rewrite(context, routesTo);
            }
        }
    }
}
