using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VK.WindowsPhone.SDK.Util;

namespace VK.WindowsPhone.SDK.API.Model
{
    public partial class VKMessage
    {

        public long id { get; set; }

        public long user_id { get; set; }

        public long date { get; set;}

        public int read_state { get; set; }

        public int @out { get; set; }

        private string _title = "";
        public string title
        {
            get { return _title; }
            set
            {
                _title = (value ?? "").ForUI();
            }
        }

        private string _body = "";

        public string body
        {
            get { return _body; }
            set
            {
                _body = (value ?? "").ForUI();
            }
        }

        public List<VKAttachment> attachments { get; set; }
        public VKGeo geo { get; set; }
        public List<VKMessage> fwd_messages { get; set; }
    
        public int emoji { get; set; }
        public int important { get; set; }

        public int deleted { get; set; }
        
        public long chat_id { get; set; }

        public List<long> chat_active { get; set; }

        public int users_count { get; set; }
        public long admin_id { get; set; }

        public VKPushSettings push_settings
        {
            get;
            set;
        }
       
        private string _action = "";
        public string action
        {
            get { return _action; }

            set
            {
                _action = (value ?? "");
            }
        }

        public long action_mid
        {
            get;
            set;
        }

        private string _action_email = "";
        public string action_email
        {
            get { return _action_email; }
            set
            {
                _action_email = (value ?? "").ForUI();
            }
        }

        private string _action_text = "";
        public string action_text
        {
            get { return _action_text; }
            set
            {
                _action_text = (value ?? "").ForUI();
            }
        }

        public string photo_50 { get; set; }

        public string photo_100 { get; set; }

        public string photo_200 { get; set; }
    }

    public partial class VKPushSettings
    {
        public int disabled_until { get; set; }

        public int sound { get; set; }      
    }
}
