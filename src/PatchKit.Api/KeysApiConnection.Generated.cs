using PatchKit.Api.Models;
using System.Collections.Generic;

namespace PatchKit.Api
{
    public partial class KeysApiConnection
    {
        
        /// <summary>
        /// Gets key info. Required providing an app secret. Will find only key that matches given app_secret.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="appSecret"></param>
        public LicenseKey GetKeyInfo(string key, string appSecret)
        {
            string path = "/1/keys/{key}";
            List<string> queryList = new List<string>();
            path = path.Replace("{key}", key.ToString());
            queryList.Add("app_secret="+appSecret);
            string query = string.Join("&", queryList.ToArray());
            var response = GetResponse(path, query);
            return ParseResponse<LicenseKey>(response);
        }
        
        
    }
}
