using System;
using Kayjax;

namespace Kayjax.Demo
{
    /// <summary>
    /// Summary description for HelloWorld
    /// </summary>
    public class HelloWorld : KayjaxHandler
    {
        protected override object[] ControlParameters
        {
            get { return null; }
        }

        protected override string ControlType
        {
            get { return null; }
        }

        protected override string ControlUrl
        {
            get { return "~/Controls/HelloWorld.ascx"; }
        }
    }
}