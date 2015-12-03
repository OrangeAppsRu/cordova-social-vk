using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VK.WindowsPhone.SDK.Util;

namespace VK.WindowsPhone.SDK.API.Model
{
    public partial class VKDocument
    {
        public long id { get; set; }

        public long owner_id { get; set; }

        private string _title = "";
        public string title
        {
            get { return _title; }
            set
            {
                _title = (value ?? "").ForUI();
            }
        }
        public long size { get; set; }
        public string ext { get; set; }
        public string url { get; set; }

        public string photo_100 { get; set; }
        public string photo_130 { get; set; }
    }
}
