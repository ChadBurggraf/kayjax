//-----------------------------------------------------------------------
// <copyright file="ApiRequest.cs" company="Tasty Codes">
//     Copyright (c) 2008 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace Kayson
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.Serialization;

    /// <summary>
    /// Base class for API requests.
    /// </summary>
    [DataContract()]
    public abstract class ApiRequest
    {
        /// <summary>
        /// When implemented in a derived class, performs an API action.
        /// </summary>
        /// <returns>The result of the action.</returns>
        [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", Justification = "I like the name.")]
        public abstract ApiActionResult Do();

        /// <summary>
        /// When implemented in a derived class, validates the values instantiated into the request object.
        /// </summary>
        /// <returns>The result of the validation.</returns>
        public abstract ApiResult Validate();
    }
}
