//-----------------------------------------------------------------------
// <copyright file="BinaryPlistRequestReader.cs" company="Tasty Codes">
//     Copyright (c) 2011 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace Kayson
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Runtime.Serialization.Plists;
    using System.Web;

    /// <summary>
    /// Implements <see cref="IRequestReader"/> to read binary plist-formatted requests.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "Spelling is correct.")]
    public class BinaryPlistRequestReader : IRequestReader
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

            if (context.Request.InputStream != null && context.Request.InputStream.Length > 0)
            {
                request = (ApiRequest)new DataContractBinaryPlistSerializer(requestType).ReadObject(context.Request.InputStream);
            }
            else
            {
                request = (ApiRequest)Activator.CreateInstance(requestType);
            }

            return request;
        }
    }
}
