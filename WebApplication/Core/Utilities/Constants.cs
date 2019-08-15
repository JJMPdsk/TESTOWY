using System.Web;

namespace Core.Utilities
{
    public static class Constants
    {
        public static readonly string Home = $"http://{HttpContext.Current.Request.Url.Authority}";
    }
}