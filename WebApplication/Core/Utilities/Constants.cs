using System.Web;

namespace Core.Utilities
{
    public static class Constants
    {
        // http dla dev, https dla deploy[!]
        public static readonly string Home = $"https://{HttpContext.Current.Request.Url.Authority}";
        public const string AppName = "webapp-template";
    }
}