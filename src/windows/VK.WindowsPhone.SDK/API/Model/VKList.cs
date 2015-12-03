using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VK.WindowsPhone.SDK.API.Model
{
    public class VKList<T>
    {
        public int count { get; set; }

        public List<T> items { get; set; }
    }
}
