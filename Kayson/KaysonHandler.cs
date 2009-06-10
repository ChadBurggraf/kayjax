using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Web;
using Kayson.Attributes;
using Kayson.Configuration;

namespace Kayson
{
    /// <summary>
    /// Handles API requests.
    /// </summary>
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
        public virtual string JSON
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
        /// <returns>The current request type.</returns>
        /// <exception cref="Kayson.InvalidRequestTypeException"></exception>
        protected virtual Type GetRequestType()
        {
            try
            {
                return Type.GetType(KaysonRouteModule.CurrentTargetRoute, true, true);
            }
            catch
            {
                throw new InvalidRequestTypeException();
            }
        }

        /// <summary>
        /// Processes the request.
        /// </summary>
        /// <param name="context">The HttpContext to process the request for.</param>
        public virtual void ProcessRequest(HttpContext context)
        {
            // Empty response.
            ApiResponse response = new ApiResponse(true, String.Empty, null, true);
            int statusCode = 200;

            // The know types we'll use for serializing the response.
            object[] knownTypes = new object[0];

            if (KaysonSettings.Section.RequireSsl == RequireSSLMode.Off ||
                (KaysonSettings.Section.RequireSsl == RequireSSLMode.RemoteOnly && context.Request.IsLocal) ||
                Regex.IsMatch(context.Request.Url.Scheme, "^https$", RegexOptions.IgnoreCase))
            {
                try
                {
                    // Instantiate the request.
                    Type requestType = GetRequestType();
                    ApiRequest request = (ApiRequest)Extensions.FromJson(requestType, JSON, new List<Type>());

                    // Permitted?
                    IPermissionAttribute failedOn;
                    if (context.EnsurePermitted(requestType, out failedOn))
                    {
                        // Get the known types.
                        knownTypes = requestType.GetCustomAttributes(typeof(KnownTypeAttribute), true);

                        // Do it.
                        string reason;
                        object output = null;
                        response.Success = request.Validate(out reason) && request.Do(out output, out reason);
                        response.Reason = reason;
                        response.Value = output;
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

            // Clear and prepare the response.
            context.Response.Clear();
            context.Response.ContentType = "application/json";
            context.Response.AppendHeader("X-Response-Id", (!String.IsNullOrEmpty(id) ? id : "0"));
            context.Response.StatusCode = statusCode;

            // Create the known type list.
            List<Type> types = new List<Type>();

            foreach (object knownType in knownTypes)
            {
                types.Add(((KnownTypeAttribute)knownType).Type);
            }

            // Get the JSON text.
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

            // Write and flush it.
            context.Response.Write(respJson);
            context.Response.End();
        }
    }
}