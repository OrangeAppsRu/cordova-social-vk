using System;
using Newtonsoft.Json.Linq;

namespace VK.WindowsPhone.SDK.Util
{
    public static class VKJTokenExtensions
    {
        /// <summary>
        /// Parse bool from JToken with given name.
        /// </summary>
        /// <param name="json">Server response with format - field: 1</param>
        /// <param name="name">Name of field to read</param>
        /// <returns></returns>
        public static bool ValueVKBool(this JToken json, String name)
        {
            return json != null && json.Value<int>(name) == 1;
        }
    }
}
