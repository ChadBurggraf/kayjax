﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.Web;

namespace Kayson
{
    /// <summary>
    /// Interface for permission attributes.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "Most appropriate name.")]
    public interface IPermission
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