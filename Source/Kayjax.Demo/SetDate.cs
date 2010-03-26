using System;
using Kayson;

namespace Kayjax.Demo
{
    public class SetDate : ApiRequest
    {
        public DateTime Date { get; set; }

        public override ApiActionResult Do()
        {
            return new ApiActionResult() { Success = true, Value = Date };
        }

        public override ApiResult Validate()
        {
            return new ApiResult() { Success = true };
        }
    }
}