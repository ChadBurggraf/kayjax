using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Kayjax
{
    /// <summary>
    /// Provides helper object extensions for the Kayjax library.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Absolutizes an URL properties in a control's child control hierarchy.
        /// </summary>
        /// <param name="control">The control whose child URLs should be absolutized.</param>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "I like the name.")]
        public static void AbsolutizeUrls(this Control control)
        {
            HtmlControl hc = control as HtmlControl;

            if (hc != null)
            {
                // Replace "src" attributes with absolute URLs.
                if (!String.IsNullOrEmpty(hc.Attributes["src"]) && hc.Attributes["src"].IndexOf("~", StringComparison.Ordinal) == 0)
                {
                    hc.Attributes["src"] = HttpContext.Current.CreateAbsoluteUrl(hc.Attributes["src"]);
                }

                // Replace "href" attributes with absolute URLs.
                if (!String.IsNullOrEmpty(hc.Attributes["href"]) && hc.Attributes["href"].IndexOf("~", StringComparison.Ordinal) == 0)
                {
                    hc.Attributes["href"] = HttpContext.Current.CreateAbsoluteUrl(hc.Attributes["href"]);
                }
            }
            else
            {
                WebControl wc = control as WebControl;

                if (wc != null)
                {
                    // Get the public instance properties of the control.
                    PropertyInfo[] props = control.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

                    // Loop through the properties.
                    foreach (PropertyInfo prop in props)
                    {
                        // If it is a URL property that is readable and writeable.
                        if (prop.Name.Contains("Url") && prop.CanRead && prop.CanWrite)
                        {
                            string value = prop.GetValue(control, null) as string;

                            // If the property has a value.
                            if (value != null)
                            {
                                // If the value is a resolveable URL.
                                if (value.IndexOf("~", StringComparison.Ordinal) == 0)
                                {
                                    // Replace it with an absolute URL.
                                    prop.SetValue(control, HttpContext.Current.CreateAbsoluteUrl(value), null);
                                }
                            }
                        }
                    }
                }
            }

            // Walk down the control's child tree.
            foreach (Control child in control.Controls)
            {
                child.AbsolutizeUrls();
            }
        }

        /// <summary>
        /// Creates an absolute URL.
        /// </summary>
        /// <param name="context">The HttpContext to create the URL from.</param>
        /// <param name="url">The URL to absolutize.</param>
        /// <returns>The absolutized URL.</returns>
        [SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", Justification = "A replacement for Page.ResolveClientUrl().")]
        [SuppressMessage("Microsoft.Design", "CA1055:UriReturnValuesShouldNotBeStrings", Justification = "A replacement for Page.ResolveClientUrl().")]
        public static string CreateAbsoluteUrl(this HttpContext context, string url)
        {
            string value = url;

            if (!Regex.IsMatch(url, "^(https?)|(ftps?)", RegexOptions.IgnoreCase))
            {
                string uri = context.Request.Url.AbsoluteUri;
                string path = context.Request.Url.AbsolutePath;
                string appPath = context.Request.ApplicationPath;

                value = uri.Substring(0, uri.LastIndexOf(path, StringComparison.Ordinal)) + url.Replace("~", (appPath[appPath.Length - 1] == '/' ? appPath.Substring(0, appPath.Length - 1) : appPath));
            }

            return value;
        }
    }
}
