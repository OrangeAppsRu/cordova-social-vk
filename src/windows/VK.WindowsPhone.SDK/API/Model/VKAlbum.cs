using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VK.WindowsPhone.SDK.Util;

namespace VK.WindowsPhone.SDK.API.Model
{
    public partial class VKAlbum
    {

        public string id { get; set; }

        public string thumb_id { get; set; }

        public string owner_id { get; set; }

        private string _title = "";
        public string title
        {
            get { return _title; }
            set
            {
                _title = (value ?? "").ForUI();
            }
        }


        private string _description = "";
        public string description
        {
            get { return _description; }
            set
            {
                _description = (value ?? "").ForUI();
            }
        }

        public string created { get; set; }

        public string updated { get; set; }

        public int size { get; set; }

        public string thumb_src { get; set; }

        public VKPrivacy privacy_view { get; set; }

        public VKPrivacy privacy_comment { get; set; }
        
    }
}
