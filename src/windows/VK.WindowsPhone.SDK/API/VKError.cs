using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VK.WindowsPhone.SDK.API
{
    public class VKError
    {
        public const int USER_AUTHORIZATION_FAILED = 5;
        public const int CAPTCHA_REQUIRED = 14;
        public const int VALIDATION_REQUIRED = 17;

        public int error_code { get; set; }
        public string error_msg { get; set; }
        public List<VKRequestParam> request_params { get; set; }
        public string captcha_sid { get; set; }
        public string captcha_img { get; set; }
        public string redirect_uri { get; set; }
    }
}
