using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VK.WindowsPhone.SDK.Util;

namespace VK.WindowsPhone.SDK.API.Model
{
    /// <summary>
    /// https://vk.com/dev/video_object
    /// </summary>
    public partial class VKVideo
    {
        public long id
        {
            get;
            set;
        }

        public long owner_id { get; set; }

        private string _title = "";
        public string title
        {
            get { return _title; }
            set { _title = (value ?? "").ForUI(); }
        }
        public int duration { get; set; }

        private string _description = "";

        public string description
        {
            get { return _description; }
            set { _description = (value ?? "").ForUI(); }
        }
        public int date { get; set; }
        public int views { get; set; }

        public string photo_130
        {
            get;
            set;
        }

        public string photo_320
        {
            get;
            set;
        }

        public string photo_640
        {
            get;
            set;
        }

        public string player { get; set; }
    }
}
