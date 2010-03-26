

namespace Kayson
{
    using System;

    /// <summary>
    /// Identifies that a Kayson response should use GZip compression if the
    /// user agent identifies that it can accept it.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class GZipAttribute : Attribute
    {
    }
}
