//-----------------------------------------------------------------------
// <copyright file="RolePermissionAttribute.cs" company="Tasty Codes">
//     Copyright (c) 2008 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace Kayson
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Web;

    /// <summary>
    /// Describes a role-based permission requirement.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    [SuppressMessage("Microsoft.Performance", "CA1813:AvoidUnsealedAttributes", Justification = "By design.")]
    public class RolePermissionAttribute : Attribute, IPermission
    {
        private string allow;

        /// <summary>
        /// Initializes a new instance of the RolePermissionAttribute class.
        /// </summary>
        /// <param name="allow">The name of a role that is allowed to access the resource.
        /// ? means only authenticated users are allowed; * means all users are allowed.
        /// </param>
        public RolePermissionAttribute(string allow)
        {
            this.allow = allow;
        }

        /// <summary>
        /// Gets or sets the name of a role that is allowed to access the resource.
        /// </summary>
        public string Allow 
        { 
            get { return this.allow ?? "*"; }
            protected set { this.allow = value; }
        }

        /// <summary>
        /// Gets the operator to use when joining permissions of this type together.
        /// </summary>
        public PermissionJoinType Join 
        { 
            get { return PermissionJoinType.Or; } 
        }

        /// <summary>
        /// Ensures that the attributes requirements are met.
        /// </summary>
        /// <param name="context">The current HttpContext.</param>
        /// <returns>True if the request is permitted, false otherwise.</returns>
        public virtual bool EnsurePermitted(HttpContextBase context)
        {
            return this.Allow == "*" ||
                (this.Allow == "?" && context.User.Identity.IsAuthenticated) ||
                (context.User.Identity.IsAuthenticated && context.User.IsInRole(this.Allow));
        }
    }
}
