using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VK.WindowsPhone.SDK.API.Model
{
    public partial class VKGeo
    {
        public string type { get; set; }
        public string coordinates { get; set; }

        public Place place { get; set; }
    }
}
