using System;
using Kayson;

namespace AjaxTest
{
    /// <summary>
    /// Summary description for GetDate
    /// </summary>
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

        public override ApiValidationResult Validate()
        {
            return new ApiValidationResult() { Success = true };
        }
    }
}