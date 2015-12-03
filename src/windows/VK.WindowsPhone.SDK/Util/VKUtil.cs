using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

#if SILVERLIGHT

using System.IO.IsolatedStorage;
using Microsoft.Phone.Controls;

#else

using Windows.Storage;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Controls;
using Windows.Web.Http;

#endif

using System.Net;
using VK.WindowsPhone.SDK.API.Model;


namespace VK.WindowsPhone.SDK.Util
{
    public static class VKUtil
    {
        /// <summary>
        /// Breaks key=value&key=value string to map
        /// </summary>
        /// <param name="queryString">string to explode</param>
        /// <returns>Key-value map of passed string</returns>
        public static Dictionary<String, String> ExplodeQueryString(String queryString)
        {
            var keyValuePairs = queryString.Split('&');
            var parameters = new Dictionary<String, String>(keyValuePairs.Length);

            foreach (var keyValueString in keyValuePairs)
            {
                var keyValueArray = keyValueString.Split('=');
                if (keyValueArray.Length == 2)
                {
                    parameters.Add(keyValueArray[0], keyValueArray[1]);
                }
            }

            return parameters;
        }

        public static string GetParamsOfQueryString(string queryString)
        {
            int indOfQ = queryString.IndexOf('?');

            if (indOfQ >= 0 && indOfQ < queryString.Length - 1)
            {
                var paramsString = queryString.Substring(indOfQ + 1);

                return paramsString;
            }

            return "";
        }

        /// <summary>
        /// Join parameters to map into string, usually query string
        /// </summary>
        /// <param name="queryArgs">Map to join</param>
        /// <param name="isUri">Indicates that value parameters must be url-encoded</param>
        /// <returns>Result query string, like k=v&k1=v=1</returns>
        /// <summary>
        /// Join parameters to map into string, usually query string
        /// </summary>
        /// <param name="queryArgs">Map to join</param>
        /// <param name="isUri">Indicates that value parameters must be url-encoded</param>
        /// <returns>Result query string, like k=v&k1=v=1</returns>
        public static String JoinParams(Dictionary<String, Object> queryArgs, bool isUri = false)
        {
            var args = new List<String>(queryArgs.Count);
            foreach (var entry in queryArgs)
            {
                var value = entry.Value;

                args.Add(String.Format("{0}={1}", entry.Key, isUri ? WebUtility.UrlEncode(value.ToString()) : value.ToString()));
            }

            return String.Join("&", args);
        }


        public static Dictionary<String, string> DictionaryFrom(params string[] args)
        {
            if (args.Length % 2 != 0)
            {
                throw new Exception("Args must be paired. Last one is ignored");
            }

            var result = new Dictionary<String, string>();
            for (int i = 0; i + 1 < args.Length; i += 2)
            {
                if (!string.IsNullOrEmpty(args[i + 1]))
                {
                    result.Add((String)args[i], args[i + 1]);
                }
            }

            return result;
        }

        /// <summary>
        /// Reads content of file and returns result as string
        /// </summary>
        /// <param name="filename">File name in IsolatedStorage</param>
        /// <returns>Contents of file</returns>
        public static String FileToString(String filename)
        {
#if SILVERLIGHT

            String text;

            using (var iso = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (!iso.FileExists(filename))
                    return null;

                var reader = new StreamReader(new IsolatedStorageFileStream(filename, FileMode.Open, iso));
                text = reader.ReadToEnd();
                reader.Close();
            }

            return text;
#else
       
            String text = "";

            Task.Run(async () =>
                {
                    var file = await Windows.Storage.ApplicationData.Current.LocalFolder.GetFileAsync(filename);
                    text = await Windows.Storage.FileIO.ReadTextAsync(file);
                })
                .Wait();

            return text;

#endif
        }

        /// <summary>
        /// Saves passed string to file in IsolatedStorage.
        /// </summary>
        /// <param name="filename">Filename in IsolatedStorage</param>
        /// <param name="stringToWrite">String to save</param>
        public static void StringToFile(String filename, String stringToWrite)
        {
#if SILVERLIGHT
            using (var iso = IsolatedStorageFile.GetUserStoreForApplication())
            {
                using (IsolatedStorageFileStream file = iso.OpenFile(filename, FileMode.Create, FileAccess.Write))
                {
                    using (StreamWriter writer = new StreamWriter(file))
                    {
                        writer.WriteLineAsync(stringToWrite);
                    }
                }
            }
#else

            Task.Run(async () =>
                {
                    var file = await ApplicationData.Current.LocalFolder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);

                    await Windows.Storage.FileIO.WriteTextAsync(file, stringToWrite);

                }).Wait();
#endif

        }

        private static readonly DateTime Jan1St1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        /// <summary>
        /// Helper method to retreive current time from Jan 1st 1970 in milliseconds.
        /// </summary>
        /// <returns>Current time from Jan 1st 1970 in milliseconds</returns>
        public static long CurrentTimeMillis()
        {
            return (long)(DateTime.UtcNow - Jan1St1970).TotalMilliseconds;
        }


        public static void ClearCookies()
        {
#if SILVERLIGHT

            var webBrowser = new WebBrowser();

            webBrowser.ClearCookiesAsync();

#else

            Windows.Web.Http.Filters.HttpBaseProtocolFilter myFilter = new Windows.Web.Http.Filters.HttpBaseProtocolFilter();
            var cookieManager = myFilter.CookieManager;
            HttpCookieCollection myCookieJar = cookieManager.GetCookies(new Uri("https://vk.com"));
            foreach (HttpCookie cookie in myCookieJar)
            {
                cookieManager.DeleteCookie(cookie);
            }

            myCookieJar = cookieManager.GetCookies(new Uri("https://login.vk.com"));
            foreach (HttpCookie cookie in myCookieJar)
            {
                cookieManager.DeleteCookie(cookie);
            }
#endif
        }
    }
}
