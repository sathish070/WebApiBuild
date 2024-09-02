using Microsoft.VisualBasic;

namespace Core_Arca.Helpers
{
    public static class UrlUtil
    {
        internal static string GetRequestUrl()
        {
                return $"{GetBaseUrl()}api/now/table/u_common_interface";
        }

        internal static string GetRequestUrl(int Id)
        {
                return $"{GetBaseUrl()}api/now/table/u_common_interface" + Id;
        }

        internal static string GetBaseUrl()
        {
            IConfiguration config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .Build();
            string baseUrl = config[$"servicenow:baseUrl"];
            return baseUrl;
        }
    }
}
