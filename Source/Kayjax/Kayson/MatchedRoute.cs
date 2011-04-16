//-----------------------------------------------------------------------
// <copyright file="MatchedRoute.cs" company="Tasty Codes">
//     Copyright (c) 2011 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace Kayson
{
    using System;

    /// <summary>
    /// Represents a matched Kayson route.
    /// </summary>
    [Serializable]
    public sealed class MatchedRoute
    {
        /// <summary>
        /// Initializes a new instance of the MatchedRoute class.
        /// </summary>
        /// <param name="routeType">The type the route routes to.</param>
        /// <param name="readerType">The <see cref="IRequestReader"/> override to use for the route, if applicable.</param>
        /// <param name="writerType">The <see cref="IResponseWriter"/> oerride to use for the route, if applicable.</param>
        public MatchedRoute(Type routeType, Type readerType, Type writerType)
        {
            if (routeType == null)
            {
                throw new ArgumentNullException("routeType", "routType cannot be null.");
            }

            this.RouteType = routeType;
            this.ReaderType = readerType;
            this.WriterType = writerType;
        }

        /// <summary>
        /// Initializes a new instance of the MatchedRoute class.
        /// </summary>
        /// <param name="route">The existing route to create this instance from.</param>
        public MatchedRoute(MatchedRoute route)
        {
            if (route == null)
            {
                throw new ArgumentNullException("route", "route cannot be null.");
            }

            this.RouteType = route.RouteType;
            this.ReaderType = route.ReaderType;
            this.WriterType = route.WriterType;
        }

        /// <summary>
        /// Gets the type of the <see cref="IRequestReader"/> override
        /// to use for this route, if applicable.
        /// </summary>
        public Type ReaderType { get; private set; }

        /// <summary>
        /// Gets the request type this route routes to.
        /// </summary>
        public Type RouteType { get; private set; }

        /// <summary>
        /// Gets the type of the <see cref="IResponseWriter"/> override
        /// to use for this route, if applicable.
        /// </summary>
        public Type WriterType { get; private set; }
    }
}
