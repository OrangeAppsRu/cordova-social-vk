using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VK.WindowsPhone.SDK.Util;

namespace VK.WindowsPhone.SDK.API.Model
{
    public partial class VKComment
    {
        public long id { get; set; }

        public long from_id { get; set; }

        public long date { get; set; }

        private string _text { get; set; }
        public string text
        {
            get { return _text; }
            set { _text = (value ?? "").ForUI(); }
        }

        public long reply_to_user { get; set; }

        public long reply_to_comment { get; set; }

        public List<VKAttachment> attachments { get; set; }
    }
}
