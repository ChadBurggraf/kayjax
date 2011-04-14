//-----------------------------------------------------------------------
// <copyright file="IResponseWriter.cs" company="Tasty Codes">
//     Copyright (c) 2011 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace Kayson
{
    using System;
    using System.Collections.Generic;
    using System.Web;

    /// <summary>
    /// Defines the interface for writing outgoing API responses.
    /// </summary>
    public interface IResponseWriter
    {
        /// <summary>
        /// Writes the given API response to the given HTTP context's response.
        /// </summary>
        /// <param name="context">The HTTP context whose response should be written to.</param>
        /// <param name="request">The de-serialized API request that generated the current response. May be null.</param>
        /// <param name="response">The API response to write.</param>
        /// <param name="knownTypes">A collection of known types that may exist in the response object graph.</param>
        void WriteResponse(HttpContextBase context, ApiRequest request, ApiResponse response, IEnumerable<Type> knownTypes);
    }
}
