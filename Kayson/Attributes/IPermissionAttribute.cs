using System;
using System.Web;

namespace Kayson.Attributes
{
    /// <summary>
    /// Interface for permission attributes.
    /// </summary>
    public interface IPermissionAttribute
    {
        /// <summary>
        /// Ensures that the attributes requirements are met.
        /// </summary>
        /// <param name="context">The current HttpContext.</param>
        /// <returns>True if the request is permitted, false otherwise.</returns>
        bool EnsurePermitted(HttpContext context);

        /// <summary>
        /// Gets the operator to use when joining permissions of this type together.
        /// </summary>
        PermissionJoinType Join { get; }
    }
}
