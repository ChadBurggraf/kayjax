

namespace Kayson
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Serialization;
    using System.Text.RegularExpressions;
    using System.Web;
    using Kayson.Configuration;

    /// <summary>
    /// Handles API requests.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "Spelling is correct.")]
    public class KaysonHandler : IHttpHandler
    {
        private string json;

        /// <summary>
        /// Gets a value indicating whether this instance is reusable for multiple requests.
        /// </summary>
        public bool IsReusable
        {
            get { return false; }
        }

        /// <summary>
        /// Gets the JSON input of the request.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "Spelling is correct.")]
        public virtual string Json
        {
            get
            {
                if (json == null)
                {
                    if (HttpContext.Current.Request.InputStream != null && HttpContext.Current.Request.InputStream.Length > 0)
                    {
                        using (StreamReader reader = new StreamReader(HttpContext.Current.Request.InputStream))
                        {
                            json = reader.ReadToEnd();
                        }
                    }
                    else
                    {
                        json = String.Empty;
                    }
                }

                return json;
            }
        }

        /// <summary>
        /// Gets the type of the request being made.
        /// </summary>
        /// <returns>The type of the request being made.</returns>
        /// <exception cref="Kayson.InvalidRequestTypeException"></exception>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "An exception might be raised.")]
        protected virtual Type GetRequestType()
        {
            try
            {
                return Type.GetType(KaysonRouteModule.CurrentTargetRoute, true, true);
            }
            catch (Exception ex)
            {
                throw new InvalidRequestTypeException(ex.Message, ex);
            }
        }

        /// <summary>
        /// Processes the request.
        /// </summary>
        /// <param name="context">The HttpContext to process the request for.</param>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "By design to ensure JSON is returned.")]
        public virtual void ProcessRequest(HttpContext context)
        {
            ApiResponse response = new ApiResponse(true, String.Empty, null, true);
            int statusCode = 200;
            bool gzip = false;

            // The known types we'll use for serializing the response.
            object[] knownTypes = new object[0];

            if (KaysonSettings.Section.RequireSsl == RequireSslMode.Off ||
                (KaysonSettings.Section.RequireSsl == RequireSslMode.RemoteOnly && context.Request.IsLocal) ||
                Regex.IsMatch(context.Request.Url.Scheme, "^https$", RegexOptions.IgnoreCase))
            {
                try
                {
                    Type requestType = GetRequestType();
                    ApiRequest request = (ApiRequest)Extensions.FromJson(requestType, Json, new List<Type>());

                    // Permitted?
                    IPermission failedOn;
                    if (context.EnsurePermitted(requestType, out failedOn))
                    {
                        knownTypes = requestType.GetCustomAttributes(typeof(KnownTypeAttribute), true);
                        ApiResult valid = request.Validate();

                        if (valid.Success)
                        {
                            // Do it.
                            ApiActionResult result = request.Do();
                            response.Success = result.Success;
                            response.Reason = result.Reason;
                            response.Value = result.Value;

                            gzip = requestType.GetCustomAttributes(typeof(GZipAttribute), true).Length > 0;
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
                        statusCode = 401;
                    }
                }
                catch (KaysonCustomException ex)
                {
                    response.Success = false;
                    response.Reason = ex.Message;
                }
                catch (InvalidRequestTypeException ex)
                {
                    response.Success = false;
                    response.Reason = ex.Message;
                    statusCode = 400;
                }
                catch (JsonSerializationException ex)
                {
                    response.Success = false;
                    response.Reason = ex.Message;
                    statusCode = 400;
                }
                catch (Exception ex)
                {
                    response.Success = false;
                    statusCode = 500;

                    if (context.Request.IsLocal)
                    {
                        response.Reason = ex.Message;
                    }
                    else
                    {
                        response.Reason = "An unspecified error occurred processing your request.";
                    }
                }
            }
            else
            {
                response.Success = false;
                response.Reason = "A secure connection is required when making this request.";
                statusCode = 403;
            }

            string id = context.Request.Headers["X-Request-Id"];
            id = !String.IsNullOrEmpty(id) ? id : "0";

            context.Response.Clear();
            context.Response.ContentType = "application/json";
            context.Response.AppendHeader("X-Response-Id", id);
            context.Response.StatusCode = statusCode;

            List<Type> types = (from t in knownTypes
                                select ((KnownTypeAttribute)t).Type).ToList();

            // Get the response JSON text.
            string respJson = response.ToJson<ApiResponse>(types);

            // Strip the type identifiers.
            respJson = Regex.Replace(respJson, "({)?(,)?\"__type\":\".*?\"(})?(,)?", new MatchEvaluator(delegate(Match match)
            {
                string str = (String.IsNullOrEmpty(match.Groups[1].Value)) ? match.Groups[2].Value : match.Groups[1].Value;

                if (str == "{")
                {
                    if (!String.IsNullOrEmpty(match.Groups[3].Value))
                    {
                        str += "}";
                    }
                }
                else
                {
                    if (!String.IsNullOrEmpty(match.Groups[3].Value))
                    {
                        str = "}";
                    }
                }

                return str;
            }));

            if (gzip && context.AcceptsGZip())
            {
                context.Response.Filter = new GZipStream(context.Response.Filter, CompressionMode.Compress);
                context.Response.AddHeader("content-encoding", "gzip");
            }

            context.Response.Write(respJson);
            context.Response.End();
        }
    }
}