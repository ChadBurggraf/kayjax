//-----------------------------------------------------------------------
// <copyright file="KaysonHandler.cs" company="Tasty Codes">
//     Copyright (c) 2008 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace Kayson
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Web;
    using Kayson.Configuration;

    /// <summary>
    /// Handles API requests.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "Spelling is correct.")]
    public class KaysonHandler : IHttpHandler
    {
        private static IDictionary<string, Type> readers;
        private static IDictionary<string, Type> writers;

        /// <summary>
        /// Initializes static members of the KaysonHandler class.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = "There is no way to do this initialization inline without method calls. This is cleaner.")]
        static KaysonHandler()
        {
            readers = new Dictionary<string, Type>();

            foreach (ReaderElement reader in KaysonSettings.Section.Readers)
            {
                readers[reader.ContentType.ToUpperInvariant()] = Type.GetType(reader.ReaderType, true);
            }

            writers = new Dictionary<string, Type>();

            foreach (WriterElement writer in KaysonSettings.Section.Writers)
            {
                writers[writer.AcceptType.ToUpperInvariant()] = Type.GetType(writer.WriterType, true);
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is reusable for multiple requests.
        /// </summary>
        public virtual bool IsReusable
        {
            get { return true; }
        }

        /// <summary>
        /// Processes the request.
        /// </summary>
        /// <param name="context">The HttpContext to process the request for.</param>
        public virtual void ProcessRequest(HttpContext context)
        {
            this.ProcessRequest(new HttpContextWrapper(context));
        }

        /// <summary>
        /// Processes the request.
        /// </summary>
        /// <param name="context">The HttpContext to process the request for.</param>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "By design to ensure JSON is returned.")]
        public virtual void ProcessRequest(HttpContextBase context)
        {
            ApiRequest request = null;
            MatchedRoute route = null;
            ApiResponse response = new ApiResponse();
            Type[] knownTypes = new Type[0];

            if (RequestPassesSslCheck(context))
            {
                try
                {
                    route = this.GetRequestRoute(context);
                    request = this.GetRequestReader(context, route, readers).ReadRequest(context, route.RouteType, response);

                    // Permitted?
                    IPermission failedOn;
                    if (context.EnsurePermitted(route.RouteType, out failedOn))
                    {
                        knownTypes = route.RouteType.GetCustomAttributes(typeof(KnownTypeAttribute), true)
                            .Cast<KnownTypeAttribute>()
                            .Select(a => a.Type)
                            .ToArray();

                        ApiResult valid = request.Validate();

                        if (valid.Success)
                        {
                            // Do it.
                            ApiActionResult result = request.Do();
                            response.Success = result.Success;
                            response.Reason = result.Reason;
                            response.Value = result.Value;
                        }
                        else
                        {
                            response.Success = false;
                            response.Reason = valid.Reason;
                        }
                    }
                    else
                    {
                        response.Success = false;
                        response.Reason = "Access denied.";
                        response.Allowed = false;
                        response.StatusCode = 401;
                    }
                }
                catch (InvalidRequestTypeException ex)
                {
                    response.Success = false;
                    response.Reason = ex.Message;
                    response.StatusCode = 400;
                }
                catch (Exception ex)
                {
                    response.Success = false;
                    response.StatusCode = 500;

                    if (context.Request.IsLocal)
                    {
                        response.Reason = String.Concat(ex.Message, "\n", ex.StackTrace);
                    }
                    else
                    {
                        response.Reason = "An internal server error occurred while processing your request.";
                    }
                }
            }
            else
            {
                response.Success = false;
                response.Reason = "A secure connection is required when making this request.";
                response.StatusCode = 403;
            }

            string id = context.Request.Headers["X-Request-Id"];
            id = !String.IsNullOrEmpty(id) ? id : "0";

            context.Response.AppendHeader("X-Response-Id", id);
            context.Response.StatusCode = response.StatusCode;

            this.GetResponseWriter(context, route, writers).WriteResponse(context, request, response, knownTypes);
        }

        /// <summary>
        /// Gets the <see cref="IRequestReader"/> to use when reading the given HTTP context's request.
        /// </summary>
        /// <param name="context">The HTTP context to get the reader for.</param>
        /// <param name="route">The Kayson route to get the reader for.</param>
        /// <param name="configuredReaders">A dictionary of readers defined in the current configuration.</param>
        /// <returns>An <see cref="IRequestReader"/> that can read the given HTTP context's request.</returns>
        /// <exception cref="System.InvalidOperationException"></exception>
        protected virtual IRequestReader GetRequestReader(HttpContextBase context, MatchedRoute route, IDictionary<string, Type> configuredReaders)
        {
            if (route != null && route.ReaderType != null)
            {
                return (IRequestReader)Activator.CreateInstance(route.ReaderType);
            }
            else
            {
                string ct = context.Request.ContentType.ToUpperInvariant();
                int separator = ct.IndexOf(';');

                if (separator > 0)
                {
                    ct = ct.Substring(0, separator);
                }

                if (configuredReaders.ContainsKey(ct))
                {
                    return (IRequestReader)Activator.CreateInstance(configuredReaders[ct]);
                }

                throw new InvalidOperationException(String.Format(CultureInfo.InvariantCulture, "The request specifies a Content-Type ({0}) not in the configured reader list.", context.Request.ContentType));
            }
        }

        /// <summary>
        /// Gets the <see cref="IResponseWriter"/> to use when writing the given HTTP context's response.
        /// </summary>
        /// <param name="context">The HTTP context to get the writer for.</param>
        /// <param name="route">The Kayson route to get the writer for.</param>
        /// <param name="configuredWriters">A dictionary of writers defined in the current configuration.</param>
        /// <returns>An <see cref="IResponseWriter"/> that can write the given HTTP context's response.</returns>
        /// <exception cref="System.InvalidOperationException"></exception>
        protected virtual IResponseWriter GetResponseWriter(HttpContextBase context, MatchedRoute route, IDictionary<string, Type> configuredWriters)
        {
            if (route != null && route.WriterType != null)
            {
                return (IResponseWriter)Activator.CreateInstance(route.WriterType);
            }
            else
            {
                foreach (string acceptType in context.Request.AcceptTypes)
                {
                    string at = acceptType.ToUpperInvariant();
                    int separator = at.IndexOf(';');

                    if (separator > 0)
                    {
                        at = at.Substring(0, separator);
                    }

                    if (at.Equals("*/*", StringComparison.Ordinal))
                    {
                        return new JsonResponseWriter();
                    }
                    else if (configuredWriters.ContainsKey(at))
                    {
                        return (IResponseWriter)Activator.CreateInstance(configuredWriters[at]);
                    }
                }

                throw new InvalidOperationException(String.Format(CultureInfo.InvariantCulture, "The request does not specify an Accept type ({0}) that is in the configured writer list.", String.Join("; ", context.Request.AcceptTypes)));
            }
        }
        
        /// <summary>
        /// Gets the route for the given <see cref="HttpContextBase"/> context's request.
        /// </summary>
        /// <param name="context">The context to get the route for.</param>
        /// <returns>A route.</returns>
        /// <exception cref="Kayson.InvalidRequestTypeException"></exception>
        protected virtual MatchedRoute GetRequestRoute(HttpContextBase context)
        {
            MatchedRoute route = KaysonRouteModule.GetTargetRoute(context);

            if (route != null)
            {
                return route;
            }
            else
            {
                throw new InvalidRequestTypeException();
            }
        }

        /// <summary>
        /// Gets a value indicating whether the request identified in the given HTTP context passes SSL checks.
        /// </summary>
        /// <param name="context">The HTTP context to check.</param>
        /// <returns>True if the request passes SSL checks, false otherwise.</returns>
        private static bool RequestPassesSslCheck(HttpContextBase context)
        {
            return KaysonSettings.Section.RequireSsl == RequireSslMode.Off
                || (KaysonSettings.Section.RequireSsl == RequireSslMode.RemoteOnly && context.Request.IsLocal)
                || context.Request.Url.Scheme.Equals("https", StringComparison.OrdinalIgnoreCase);
        }
    }
}