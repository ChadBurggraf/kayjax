//-----------------------------------------------------------------------
// <copyright file="SetDate.cs" company="Tasty Codes">
//     Copyright (c) 2008 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace Kayjax.Demo
{
    using System;
    using Kayson;

    /// <summary>
    /// Implements <see cref="ApiRequest"/> to set a date.
    /// </summary>
    [GZip]
    public class SetDate : ApiRequest
    {
        /// <summary>
        /// Gets or sets the date to set.
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// When implemented in a derived class, performs an API action.
        /// </summary>
        /// <returns>The result of the action.</returns>
        public override ApiActionResult Do()
        {
            return new ApiActionResult() { Success = true, Value = this.Date };
        }

        /// <summary>
        /// When implemented in a derived class, validates the values instantiated into the request object.
        /// </summary>
        /// <returns>The result of the validation.</returns>
        public override ApiResult Validate()
        {
            return new ApiResult() { Success = true };
        }
    }
}