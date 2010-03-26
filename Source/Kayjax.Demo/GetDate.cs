using System;
using Kayson;

namespace Kayjax.Demo
{
    /// <summary>
    /// Summary description for GetDate
    /// </summary>
    [GZip]
    public class GetDate : ApiRequest
    {
        public int? AddDays { get; set; }

        public override ApiActionResult Do()
        {
            return new ApiActionResult()
            {
                Value = AddDays != null ? DateTime.Now.AddDays(AddDays.Value) : DateTime.Now,
                Success = true
            };
        }

        public override ApiResult Validate()
        {
            return new ApiResult() { Success = true };
        }
    }
}