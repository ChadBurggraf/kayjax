using System;
using Kayson;

namespace AjaxTest
{
    public class SetDate : ApiRequest
    {
        public DateTime Date { get; set; }

        public override ApiActionResult Do()
        {
            return new ApiActionResult() { Success = true, Value = Date };
        }

        public override ApiValidationResult Validate()
        {
            return new ApiValidationResult() { Success = true };
        }
    }
}