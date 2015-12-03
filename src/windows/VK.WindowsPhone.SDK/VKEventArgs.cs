using VK.WindowsPhone.SDK.API;

namespace VK.WindowsPhone.SDK
{
    public class VKCaptchaReceivedEventArgs
    {
        /// <summary>
        /// Error returned from API. You can load captcha image from CaptchaImg property.
        /// After user answered current captcha, call AnswerCaptcha method with user entered answer. 
        /// </summary>
        public VKError Error;
    }

    public class VKAccessTokenExpiredEventArgs
    {
        /// <summary>
        /// Old token that has expired
        /// </summary>
        public VKAccessToken ExpiredToken;
    }

    public class VKAccessDeniedEventArgs
    {
        /// <summary>
        /// Error describing authorization error
        /// </summary>
        public VKError AuthorizationError;
    }

    public class VKAccessTokenReceivedEventArgs
    {
        /// <summary>
        /// New token for API requests
        /// </summary>
        public VKAccessToken NewToken;
    }

    public class VKAccessTokenAcceptedEventArgs
    {
        /// <summary>
        /// Used token for API requests
        /// </summary>
        public VKAccessToken Token;
    }

    public class AccessTokenRenewedEventArgs
    {
        /// <summary>
        /// Used token for API requests
        /// </summary>
        public VKAccessToken Token;
    }
  
    public class VKRequestErrorEventArgs
    {
        /// <summary>
        /// Error for VKRequest
        /// </summary>
        public VKError Error;
    }

    public class VKRequestProgressEventArgs
    {
        //todo
    }
}
