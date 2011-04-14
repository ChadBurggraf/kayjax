//-----------------------------------------------------------------------
// <copyright file="IRequestReader.cs" company="Tasty Codes">
//     Copyright (c) 2011 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace Kayson
{
    using System;
    using System.Web;

    /// <summary>
    /// Defines the interface for reading incoming Kayson requests.
    /// </summary>
    public interface IRequestReader
    {
        /// <summary>
        /// Reads a request from the given HTTP context into the given concrete type.
        /// </summary>
        /// <param name="context">The HTTP context to read the request from.</param>
        /// <param name="requestType">The concrete request type to de-serialize.</param>
        /// <param name="response">The API response to modify of something goes wrong.</param>
        /// <returns>An <see cref="ApiRequest"/>.</returns>
        ApiRequest ReadRequest(HttpContextBase context, Type requestType, ApiResponse response);
    }
}
