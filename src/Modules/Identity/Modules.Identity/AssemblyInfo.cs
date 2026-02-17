using System.Runtime.CompilerServices;
using ERA.Framework.Web.Modules;

[assembly: FshModule(typeof(ERA.Modules.Identity.IdentityModule), 100)]
[assembly: InternalsVisibleTo("Identity.Tests")]
