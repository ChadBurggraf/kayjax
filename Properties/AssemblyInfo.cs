using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

#region Code Analysis Suppressions

[assembly: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", 
    Justification = "Spelling is correct.",
    Scope = "Assembly",
    Target = "Kayjax.dll")]

[assembly: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly",
    Justification = "Spelling is correct.",
    Scope = "Namespace",
    Target = "Kayjax")]

[assembly: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes",
    Justification = "Makes more logical sense to keep functionalities separate.",
    Scope = "Namespace",
    Target = "Kayjax")]

[assembly: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly",
    Justification = "Spelling is correct.",
    Scope = "Namespace",
    Target = "Kayson")]

[assembly: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly",
    Justification = "Spelling is correct.",
    Scope = "Namespace",
    Target = "Kayson.Configuration")]

#endregion

[assembly: CLSCompliant(true)]
[assembly: AssemblyTitle("Kayjax")]
[assembly: AssemblyDescription("Ajax JSON API framework.")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Tasty Codes")]
[assembly: AssemblyProduct("Kayjax")]
[assembly: AssemblyCopyright("Copyright © Chad Burggraf 2008")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: ComVisible(false)]
[assembly: Guid("64687beb-630f-43f4-a979-4e8832a51349")]
[assembly: AssemblyVersion("1.2.1.0")]
[assembly: AssemblyFileVersion("1.2.1.0")]
