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

        public override bool Do(out object output, out string reason)
        {
            output = AddDays != null ? DateTime.Now.AddDays(AddDays.Value) : DateTime.Now;
            reason = String.Empty;
            return true;
        }

        public override bool Validate(out string reason)
        {
            reason = String.Empty;
            return true;
        }
    }
}