//-----------------------------------------------------------------------
// <copyright file="JsonRequestReader.cs" company="Tasty Codes">
//     Copyright (c) 2011 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace Kayson
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Web;

    /// <summary>
    /// Implements <see cref="IRequestReader"/> to read JSON-formatted requests.
    /// </summary>
    public class JsonRequestReader : IRequestReader
    {
        /// <summary>
        /// Reads a request from the given HTTP context into the given concrete type.
        /// </summary>
        /// <param name="context">The HTTP context to read the request from.</param>
        /// <param name="requestType">The concrete request type to de-serialize.</param>
        /// <param name="response">The API response to modify of something goes wrong.</param>
        /// <returns>An <see cref="ApiRequest"/>.</returns>
        public virtual ApiRequest ReadRequest(HttpContextBase context, Type requestType, ApiResponse response)
        {
            ApiRequest request = null;
            string json = String.Empty;

            if (context.Request.InputStream != null && context.Request.InputStream.Length > 0)
            {
                using (StreamReader reader = new StreamReader(context.Request.InputStream))
                {
                    json = reader.ReadToEnd();
                }
            }

            try
            {
                request = (ApiRequest)Extensions.FromJson(requestType, json, new List<Type>());
            }
            catch (JsonSerializationException ex)
            {
                response.Success = false;
                response.Reason = ex.Message;
                response.StatusCode = 400;
            }

            return request;
        }
    }
}
