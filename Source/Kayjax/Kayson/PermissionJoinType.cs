

namespace Kayson
{
    using System;

    /// <summary>
    /// Defines the possible permission join types.
    /// </summary>
    public enum PermissionJoinType
    {
        /// <summary>
        /// Identifies that permissions of the same type will be joined with an "AND" operator.
        /// </summary>
        And,

        /// <summary>
        /// Identifies that permissions of the same type will be joined with an "OR" operator.
        /// </summary>
        Or
    }
}
