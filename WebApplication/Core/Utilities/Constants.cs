using System.Web;

namespace Core.Utilities
{
    public static class Constants
    {
        // http dla dev, https dla deploy[!]
        public static readonly string Home = $"https://{HttpContext.Current.Request.Url.Authority}";
        public const string AppName = "webapp-template";
        public const string AdminUsername = "TemplateAdmin";
        public const int MaxProfileImageSizeInKb = 25000; // 25MB

        public const bool IsInProduction = true;
        public const string DeployedHomeAddress = "https://www.google.com";
    }
}