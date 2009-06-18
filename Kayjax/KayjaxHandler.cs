using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.SessionState;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Kayson;

namespace Kayjax
{
    /// <summary>
    /// Base class for handling Ajax UI update requests.
    /// </summary>
    public abstract class KayjaxHandler : Page, IRequiresSessionState
    {
        private List<Exception> exceptions = new List<Exception>();

        #region Abstract Members

        /// <summary>
        /// When implemented in a derived class, gets the parameters to use when 
        /// instantiated a control by type.
        /// </summary>
        protected abstract object[] ControlParameters { get; }

        /// <summary>
        /// When implemented in a derived class, gets the type of control to render.
        /// </summary>
        protected abstract string ControlType { get; }

        /// <summary>
        /// When implemented in a derived class, gets the application-relative URL of the control to render.
        /// </summary>
        protected abstract string ControlUrl { get; }

        #endregion

        #region Base Overrides

        /// <summary>
        /// ViewState is dsabled.
        /// </summary>
        public override bool EnableViewState
        {
            get { return false; }
            set { }
        }

        /// <summary>
        /// Raises the page's Error event.
        /// </summary>
        protected override void OnError(EventArgs e)
        {
            exceptions.Add(Context.Error ?? new Exception("An unhandled exception occurred."));
            base.OnError(e);
        }

        /// <summary>
        /// Raises the page's Load event.
        /// </summary>
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
                IPermissionAttribute failedOn;
                if (!Context.EnsurePermitted(this.GetType(), out failedOn))
                {
                    throw new KayjaxCustomException("Access denied.");
                }

                // Make sure we have something to do.
                if (String.IsNullOrEmpty(ControlType) && String.IsNullOrEmpty(ControlUrl))
                {
                    throw new ArgumentException("Either ControlType or ControlUrl must have a value.");
                }

                // Decide what to do.
                if (!String.IsNullOrEmpty(ControlType))
                {
                    form.Controls.Add(LoadControl(Type.GetType(ControlType), ControlParameters));
                }
                else
                {
                    form.Controls.Add(LoadControl(ControlUrl));
                }
            }
            catch (Exception ex)
            {
                exceptions.Add(ex);
            }
        }

        /// <summary>
        /// Renders the page to an HtmlTextWriter.
        /// </summary>
        /// <param name="writer">The HtmlTextWriter to write to.</param>
        protected override void Render(HtmlTextWriter writer)
        {
            try
            {
                if (exceptions.Count == 0)
                {
                    // Re-base all of the URLs on the page.
                    this.AbsolutizeUrls();

                    // Write the page.
                    StringBuilder sb = new StringBuilder();
                    HtmlTextWriter w = new HtmlTextWriter(new StringWriter(sb));
                    RenderChildren(w);

                    // Get rid of the form wrapper.
                    string content = Regex.Replace(sb.ToString(), @"</?form[^>]*?>", String.Empty, RegexOptions.IgnoreCase);

                    // Get rid of the viewstate and eventvalidation inputs.
                    content = Regex.Replace(content, @"(<div>[^<]*?)?</?input.*?__VIEWSTATE[^>]*?>([^<]*?</div>)?", String.Empty, RegexOptions.IgnoreCase);
                    content = Regex.Replace(content, @"</?input.*?__EVENTVALIDATION[^>]*?>", String.Empty, RegexOptions.IgnoreCase);

                    // Write it.
                    writer.Write(content.Trim());

                    // Clear the exceptions.
                    exceptions.Clear();
                }
            }
            catch (Exception ex)
            {
                exceptions.Add(ex);
            }

            // Write an error message?
            if (exceptions.Count > 0)
            {
                string message = String.Empty;

                if (Request.IsLocal)
                {
                    message = String.Join(", ", exceptions.ConvertAll<string>(new Converter<Exception, string>(delegate(Exception ex) { return ex.Message; })).ToArray());
                }
                else
                {
                    Exception custom = exceptions.Find(new Predicate<Exception>(delegate(Exception ex) { return ex is KayjaxCustomException; }));
                    message = custom != null ? custom.Message : "An unspecified error occurred.";
                }

                writer.Write(String.Format("<div class=\"kayjaxerror\">{0}</div>", message));
            }
        }

        #endregion
    }
}
