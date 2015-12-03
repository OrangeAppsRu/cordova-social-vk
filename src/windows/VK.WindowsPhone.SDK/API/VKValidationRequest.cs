using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VK.WindowsPhone.SDK.API
{
    class VKValidationRequest
    {
        public string ValidationUri { get; set; }
    }

    class VKValidationResponse
    {
        public bool IsSucceeded { get; set; }
    }
}
