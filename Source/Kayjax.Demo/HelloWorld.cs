//-----------------------------------------------------------------------
// <copyright file="HelloWorld.cs" company="Tasty Codes">
//     Copyright (c) 2008 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace Kayjax.Demo
{
    using System;
    using Kayjax;

    /// <summary>
    /// Implements <see cref="KayjaxHandler"/> for the HelloWorld user control.
    /// </summary>
    public class HelloWorld : KayjaxHandler
    {
        /// <summary>
        /// Gets the parameters to use when instantiating a control by type.
        /// </summary>
        protected override object[] ControlParameters
        {
            get { return null; }
        }

        /// <summary>
        /// Gets the type of control to render.
        /// </summary>
        protected override string ControlType
        {
            get { return null; }
        }

        /// <summary>
        /// Gets the application-relative URL of the control to render.
        /// </summary>
        protected override string ControlUrl
        {
            get { return "~/Controls/HelloWorld.ascx"; }
        }
    }
}