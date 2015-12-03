using System;
using System.Collections.Generic;
#if SILVERLIGHT
using System.IO.IsolatedStorage;
#endif
using VK.WindowsPhone.SDK.Util;

namespace VK.WindowsPhone.SDK
{
    public class VKAccessToken
    {
        public const String ACCESS_TOKEN = "access_token";
        public const String EXPIRES_IN = "expires_in";
        public const String USER_ID = "user_id";
        public const String SECRET = "secret";
        public const String HTTPS_REQUIRED = "https_required";
        public const String CREATED = "created";
        public const String SUCCESS = "success";

        /// <summary>
        /// String token for use in request parameters
        /// </summary>
        public String AccessToken = null;

        /// <summary>
        /// Time when token expires
        /// </summary>
        public int ExpiresIn = 0;

        /// <summary>
        /// Current user id for this token
        /// </summary>
        public String UserId = null;

        /// <summary>
        /// User secret to sign requests (if nohttps used)
        /// </summary>
        public String Secret = null;

        /// <summary>
        /// If user sets "Always use HTTPS" setting in his profile, it will be true
        /// </summary>
        public bool IsHttpsRequired = false;

        /// <summary>
        /// Indicates time of token creation
        /// </summary>
        public long Created = 0;

        /// <summary>
        /// Save token into Isolated Storage with key
        /// </summary>
        /// <param name="tokenKey">Your key for saving settings</param>
        public void SaveTokenToIsolatedStorage(String tokenKey)
        {
#if SILVERLIGHT
            var iso = IsolatedStorageSettings.ApplicationSettings;
            
            var tokenData = SerializeTokenData();

            iso[tokenKey] = tokenData;

            iso.Save();
#else

            Windows.Storage.ApplicationData.Current.LocalSettings.Values[tokenKey] = SerializeTokenData();
#endif
        }

        /// <summary>
        /// Removes token from Isolated Storage with specified key
        /// </summary>
        /// <param name="tokenKey">Your key for saving settings</param>
        public static void RemoveTokenInIsolatedStorage(String tokenKey)
        {
#if SILVERLIGHT
            var iso = IsolatedStorageSettings.ApplicationSettings;
            iso.Remove(tokenKey);
#else
            Windows.Storage.ApplicationData.Current.LocalSettings.Values.Remove(tokenKey);
#endif
        }


        /// <summary>
        /// Serialize token into string
        /// </summary>
        /// <returns></returns>
        protected String SerializeTokenData()
        {
            var args = new Dictionary<String, Object>
            {
                {ACCESS_TOKEN, AccessToken},
                {EXPIRES_IN, ExpiresIn},
                {USER_ID, UserId},
                {CREATED, Created}
            };

            if (Secret != null)
                args.Add(SECRET, Secret);

            if (IsHttpsRequired)
                args.Add(HTTPS_REQUIRED, "1");

            return VKUtil.JoinParams(args);
        }

        public VKAccessToken()
        {
        }

        /// <summary>
        /// Retreive token from key-value query string
        /// </summary>
        /// <param name="urlString">String that contains URL-query part with token. E.g. access_token=eee&expires_in=0..</param>
        /// <returns>parsed token</returns>
        public static VKAccessToken FromUrlString(String urlString)
        {
            if (urlString == null)
                return null;

            var args = VKUtil.ExplodeQueryString(urlString);

            return TokenFromParameters(args);
        }

        /// <summary>
        /// Retreive token from key-value map
        /// </summary>
        /// <param name="args">Dictionary containing token info</param>
        /// <returns>Parsed token</returns>
        public static VKAccessToken TokenFromParameters(Dictionary<String, String> args)
        {
            if (args == null || args.Count == 0)
                return null;

            try
            {
                var token = new VKAccessToken();
                args.TryGetValue(ACCESS_TOKEN, out token.AccessToken);

                string expiresValue;
                args.TryGetValue(EXPIRES_IN, out expiresValue);
                int.TryParse(expiresValue, out token.ExpiresIn);

                args.TryGetValue(USER_ID, out token.UserId);
                args.TryGetValue(SECRET, out token.Secret);

                if (args.ContainsKey(HTTPS_REQUIRED))
                    token.IsHttpsRequired = args[HTTPS_REQUIRED] == "1";
                else if (token.Secret == null)
                    token.IsHttpsRequired = true;

                if (args.ContainsKey(CREATED))
                    long.TryParse(args[CREATED], out token.Created);
                else
                    token.Created = VKUtil.CurrentTimeMillis();

                return token;
            }
            catch { }

            return null;
        }

        /// <summary>
        /// Retreives token from Isolated Storage
        /// </summary>
        /// <param name="tokenKey">Your key for saving settings</param>
        /// <returns>Previously saved token or null</returns>
        public static VKAccessToken TokenFromIsolatedStorage(String tokenKey)
        {
#if SILVERLIGHT
            var iso = IsolatedStorageSettings.ApplicationSettings;
            if (!iso.Contains(tokenKey)) return null;

            String tokenString = iso[tokenKey].ToString();

            return FromUrlString(tokenString);
#else
            if (!Windows.Storage.ApplicationData.Current.LocalSettings.Values.ContainsKey(tokenKey))
            {
                return null;
            }

            String tokenString = Windows.Storage.ApplicationData.Current.LocalSettings.Values[tokenKey].ToString();

            return FromUrlString(tokenString);
#endif
        }

        /// <summary>
        /// Retreive token from file. Token must be saved into file via VKAccessToken.SaveToTokenFile()
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static VKAccessToken TokenFromFile(String filename)
        {
            try
            {
                String data = VKUtil.FileToString(filename);
                return FromUrlString(data);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Checks token expiration time
        /// </summary>
        /// <returns>true if token has expired</returns>
        public bool IsExpired
        {
            get { return ExpiresIn > 0 && ExpiresIn * 1000 + Created < VKUtil.CurrentTimeMillis(); }
        }
    }
}
