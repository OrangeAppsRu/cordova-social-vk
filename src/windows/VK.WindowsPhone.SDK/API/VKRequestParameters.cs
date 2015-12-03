using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VK.WindowsPhone.SDK.Util;

namespace VK.WindowsPhone.SDK.API
{
    public class VKRequestParameters
    {
        public Dictionary<string, string> Parameters { get; private set; }
        public string MethodName { get; private set; }

        public VKRequestParameters(string methodName, Dictionary<string, string> parameters = null)
        {
            InitializeWith(methodName, parameters);
        }

        public VKRequestParameters(string methodName, params string[] parameters)
        {
            var dictParameters = VKUtil.DictionaryFrom(parameters);

            InitializeWith(methodName, dictParameters);
        }

        private void InitializeWith(string methodName, Dictionary<string, string> parameters)
        {
            if (string.IsNullOrEmpty(methodName))
            {
                throw new ArgumentException("methodName");
            }

            MethodName = methodName;
            Parameters = parameters;
        }
    }

    public class VKRequestParam
    {
        public string key { get; set; }
        public string value { get; set; }
    }    
}
