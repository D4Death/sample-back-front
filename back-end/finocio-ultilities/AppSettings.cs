using System;
namespace sample_ultilities
{
    public class AppSettings
    {
        public static ConnectionStrings ConnectionStrings { get; set; }
        public static WebApiServer WebApiServer { get; set; }
        public static string JwtKey { get; set; }
        public static string Issuer { get; set; }
        public static string AppRequestKey { get; set; }
    }
}
