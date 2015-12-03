using System;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace VK.WindowsPhone.SDK.API.Model
{  
    /// <summary>
    /// https://vk.com/dev/privacy_setting
    /// </summary>
    public partial class VKPrivacy
    {
        public string type {get;set;}

        public List<long> users {get;set;}

        public List<long> lists {get;set;}

        public List<long> except_lists {get;set;}        

        public List<long> except_users {get;set;}
    }

}
