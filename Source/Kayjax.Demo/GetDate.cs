//-----------------------------------------------------------------------
// <copyright file="GetDate.cs" company="Tasty Codes">
//     Copyright (c) 2008 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace Kayjax.Demo
{
    using System;
    using System.Runtime.Serialization;
    using Kayson;

    /// <summary>
    /// Implements <see cref="ApiRequest"/> to get a date.
    /// </summary>
    [GZip]
    public class GetDate : ApiRequest
    {
        /// <summary>
        /// Gets or sets the number of days to add to the date.
        /// </summary>
        public int? AddDays { get; set; }

        /// <summary>
        /// When implemented in a derived class, performs an API action.
        /// </summary>
        /// <returns>The result of the action.</returns>
        public override ApiActionResult Do()
        {
            return new ApiActionResult()
            {
                Value = this.AddDays != null ? DateTime.Now.AddDays(this.AddDays.Value) : DateTime.Now,
                Success = true
            };
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