using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VK.WindowsPhone.SDK.API
{
    public class VKCaptchaUserRequest
    {
        public string CaptchaSid { get; set; }
        public string Url { get; set; }
    }

    public class VKCaptchaUserResponse
    {
        public VKCaptchaUserRequest Request { get; set; }

        public bool IsCancelled { get; set; }

        public string EnteredString { get; set; }
    }
}
