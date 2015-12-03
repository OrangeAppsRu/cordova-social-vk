using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VK.WindowsPhone.SDK.API
{
    public class VKBackendResult<T>
    {
        public VKResultCode ResultCode { get; set; }

        public string ResultString { get; set; }

        public T Data { get; set; }

        public VKError Error { get; set; }
    }
}
