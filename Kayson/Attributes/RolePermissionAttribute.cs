﻿using System;
using System.Web;

namespace Kayson.Attributes
{
    /// <summary>
    /// Describes a role-based permission requirement.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class RolePermissionAttribute : Attribute, IPermissionAttribute
    {
        private string allow;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="allow">The name of a role that is allowed to access the resource.
        /// ? means only authenticated users are allowed; * means all users are allowed.
        /// </param>
        public RolePermissionAttribute(string allow)
        {
            this.allow = allow;
        }

        /// <summary>
        /// Gets the name of a role that is allowed to access the resource.
        /// </summary>
        public string Allow { get { return allow ?? "*"; } }

        /// <summary>
        /// Gets the operator to use when joining permissions of this type together.
        /// </summary>
        public PermissionJoinType Join { get { return PermissionJoinType.Or; } }

        /// <summary>
        /// Ensures that the attributes requirements are met.
        /// </summary>
        /// <param name="context">The current HttpContext.</param>
        /// <returns>True if the request is permitted, false otherwise.</returns>
        public bool EnsurePermitted(HttpContext context)
        {
            return (Allow == "*") ||
                (Allow == "?" && context.User.Identity.IsAuthenticated) ||
                (context.User.Identity.IsAuthenticated && context.User.IsInRole(Allow));
        }
    }
}