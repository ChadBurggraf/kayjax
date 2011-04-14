//-----------------------------------------------------------------------
// <copyright file="JsonResponseWriter.cs" company="Tasty Codes">
//     Copyright (c) 2011 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace Kayson
{
    using System;
    using System.Collections.Generic;
    using System.IO.Compression;
    using System.Text.RegularExpressions;
    using System.Web;

    /// <summary>
    /// Implements <see cref="IResponseWriter"/> to write JSON-formatted responses.
    /// </summary>
    public class JsonResponseWriter : IResponseWriter
    {
        /// <summary>
        /// Writes the given API response to the given HTTP context's response.
        /// </summary>
        /// <param name="context">The HTTP context whose response should be written to.</param>
        /// <param name="request">The de-serialized API request that generated the current response. May be null.</param>
        /// <param name="response">The API response to write.</param>
        /// <param name="knownTypes">A collection of known types that may exist in the response object graph.</param>
        public virtual void WriteResponse(HttpContextBase context, ApiRequest request, ApiResponse response, IEnumerable<Type> knownTypes)
        {
            string json = response.ToJson(knownTypes);
            json = Regex.Replace(json, "({)?(,)?\"__type\":\".*?\"(})?(,)?", new MatchEvaluator(TypeIdentifiersEvaluator));
            
            if (context.AcceptsGZip() && request != null && request.GetType().GetCustomAttributes(typeof(GZipAttribute), true).Length > 0)
            {
                context.Response.Filter = new GZipStream(context.Response.Filter, CompressionMode.Compress);
                context.Response.AddHeader("content-encoding", "gzip");
            }

            context.Response.ContentType = "application/json";
            context.Response.Write(json);
        }

        /// <summary>
        /// Evaluates the given regular epression match for stripping type identifiers.
        /// </summary>
        /// <param name="match">The match to evaluate.</param>
        /// <returns>The results of the evaluation.</returns>
        private static string TypeIdentifiersEvaluator(Match match)
        {
            string str = String.IsNullOrEmpty(match.Groups[1].Value) ? match.Groups[2].Value : match.Groups[1].Value;

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
        }
    }
}
