using System.Net;
using System.Web;
using Core.Controllers;

namespace Core.Utilities
{
    public static class Constants
    {
        public static readonly string Home = $"https://{HttpContext.Current.Request.Url.Authority}";
    }
}