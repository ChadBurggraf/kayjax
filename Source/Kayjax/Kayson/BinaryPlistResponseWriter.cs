//-----------------------------------------------------------------------
// <copyright file="BinaryPlistResponseWriter.cs" company="Tasty Codes">
//     Copyright (c) 2011 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace Kayson
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.Serialization.Plists;
    using System.Web;

    /// <summary>
    /// Implements <see cref="IResponseWriter"/> to write binary plist-formatted responses.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "Spelling is correct.")]
    public class BinaryPlistResponseWriter : IResponseWriter
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
            context.Response.ContentType = "application/x-bplist";
            new DataContractBinaryPlistSerializer(response.GetType()).WriteObject(context.Response.OutputStream, response);
        }
    }
}
