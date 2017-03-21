using System.Configuration;
using System.Net;

namespace This_sol_on_Mars
{
    public class Proxy
    {
        public static void Connect()
        {
            IWebProxy proxy = WebRequest.GetSystemWebProxy();

            NetworkCredential nc = new NetworkCredential(GetConfig("proxyLogin"), GetConfig("proxyPassword"), GetConfig("proxyDomain"));
            proxy.Credentials = nc;

            WebRequest.DefaultWebProxy = proxy;
        }

        /// <summary>
        /// Pobiera wartość parametru z pliku App.config.
        /// </summary>
        /// <param name="key">Klucz parametru.</param>
        /// <returns>Wartość parametru.</returns>
        private static string GetConfig(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }
    }
}
