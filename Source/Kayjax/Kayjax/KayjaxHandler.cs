//-----------------------------------------------------------------------
// <copyright file="KayjaxHandler.cs" company="Tasty Codes">
//     Copyright (c) 2008 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace Kayjax
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Web.SessionState;
    using System.Web.UI;
    using System.Web.UI.HtmlControls;
    using Kayson;
    
    /// <summary>
    /// Base class for handling Ajax UI update requests.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "Spelling is correct.")]
    public abstract class KayjaxHandler : Page, IRequiresSessionState
    {
        private List<Exception> exceptions = new List<Exception>();

        /// <summary>
        /// Gets or sets a value indicating whether ViewState is enabled.
        /// Overridden to always return false.
        /// </summary>
        public override bool EnableViewState
        {
            get { return false; }
            set { }
        }

        /// <summary>
        /// Gets the parameters to use when instantiating a control by type.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "By design for usability.")]
        protected abstract object[] ControlParameters { get; }

        /// <summary>
        /// Gets the type of control to render.
        /// </summary>
        protected abstract string ControlType { get; }

        /// <summary>
        /// Gets the application-relative URL of the control to render.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings", Justification = "By design for usability.")]
        protected abstract string ControlUrl { get; }

        /// <summary>
        /// Raises the page's Error event.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        protected override void OnError(EventArgs e)
        {
            this.exceptions.Add(Context.Error ?? new InvalidOperationException("An unhandled exception occurred."));
            base.OnError(e);
        }

        /// <summary>
        /// Raises the page's Load event.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "We want to control the output to the client.")]
        protected override void OnLoad(EventArgs e)
        {
            // Set the response ID.
            string id = Request.Headers["X-Request-Id"];
            Response.AppendHeader("X-Response-Id", (!String.IsNullOrEmpty(id) ? id : "0"));

            // Ensure an empty canvas.
            Controls.Clear();

            // Add the form so ASP.NET doesn't have nightmares.
            HtmlForm form = new HtmlForm();
            Controls.Add(form);

            try
            {
                // Ensure we're allowed.
                IPermission failedOn;
                if (!Context.EnsurePermitted(this.GetType(), out failedOn))
                {
                    throw new KayjaxCustomException("Access denied.");
                }

                // Make sure we have something to do.
                if (String.IsNullOrEmpty(this.ControlType) && String.IsNullOrEmpty(this.ControlUrl))
                {
                    throw new ArgumentException("Either ControlType or ControlUrl must have a value.");
                }

                // Decide what to do.
                if (!String.IsNullOrEmpty(this.ControlType))
                {
                    form.Controls.Add(LoadControl(Type.GetType(this.ControlType), this.ControlParameters));
                }
                else
                {
                    form.Controls.Add(LoadControl(this.ControlUrl));
                }
            }
            catch (Exception ex)
            {
                this.exceptions.Add(ex);
            }
        }

        /// <summary>
        /// Renders the page to an HtmlTextWriter.
        /// </summary>
        /// <param name="writer">The HtmlTextWriter to write to.</param>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "We want to control the output to the client.")]
        protected override void Render(HtmlTextWriter writer)
        {
            try
            {
                if (this.exceptions.Count == 0)
                {
                    // Re-base all of the URLs on the page.
                    this.AbsolutizeUrls();

                    // Write the page.
                    StringBuilder sb = new StringBuilder();
                    HtmlTextWriter w = new HtmlTextWriter(new StringWriter(sb, CultureInfo.InvariantCulture));
                    RenderChildren(w);

                    // Get rid of the form wrapper.
                    string content = Regex.Replace(sb.ToString(), @"</?form[^>]*?>", String.Empty, RegexOptions.IgnoreCase);

                    // Get rid of the viewstate and eventvalidation inputs.
                    content = Regex.Replace(content, @"(<div>[^<]*?)?</?input.*?__VIEWSTATE[^>]*?>([^<]*?</div>)?", String.Empty, RegexOptions.IgnoreCase);
                    content = Regex.Replace(content, @"</?input.*?__EVENTVALIDATION[^>]*?>", String.Empty, RegexOptions.IgnoreCase);

                    // Write it.
                    writer.Write(content.Trim());

                    // Clear the exceptions.
                    this.exceptions.Clear();
                }
            }
            catch (Exception ex)
            {
                this.exceptions.Add(ex);
            }

            // Write an error message?
            if (this.exceptions.Count > 0)
            {
                string message = String.Empty;

                if (Request.IsLocal)
                {
                    message = String.Join(", ", this.exceptions.ConvertAll<string>(new Converter<Exception, string>(delegate(Exception ex) { return ex.Message; })).ToArray());
                }
                else
                {
                    Exception custom = this.exceptions.Find(new Predicate<Exception>(delegate(Exception ex) { return ex is KayjaxCustomException; }));
                    message = custom != null ? custom.Message : "An unspecified error occurred.";
                }

                writer.Write(String.Format(CultureInfo.InvariantCulture, "<div class=\"kayjaxerror\">{0}</div>", message));
            }
        }
    }
}
