using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VK.WindowsPhone.SDK.Util;

namespace VK.WindowsPhone.SDK.API.Model
{
    public partial class VKChat
    {
        public string type { get; set; }
    

        public long id { get; set; }

        private string _title = "";
        public string title
        {
            get { return _title; }
            set
            {
                _title = (value ?? "").ForUI();
            }
        }
        public long admin_id { get; set; }
        public List<long> users { get; set; }

        public string photo_100 { get; set; }

        public string photo_200 { get; set; }
    }
}
