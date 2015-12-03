using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;

namespace VK.WindowsPhone.SDK_XAML.Strings
{
    public class Resources
    {
        /// <summary>
        ///   Looks up a localized string similar to cancel.
        /// </summary>
        public static string Captcha_Cancel
        {
            get
            {
                return GetLocalizedString("Captcha_Cancel");
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Enter the code from the picture.
        /// </summary>
        public static string Captcha_RequiredText
        {
            get
            {
                return GetLocalizedString("Captcha_RequiredText");
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to send.
        /// </summary>
        public static string Captcha_Send
        {
            get
            {
                return GetLocalizedString("Captcha_Send");
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Error.
        /// </summary>
        public static string Error
        {
            get
            {
                return GetLocalizedString("Error");
            }
        }

        public static string GetLocalizedString(string key)
        {
            var rl = ResourceLoader.GetForCurrentView("VK.WindowsPhone.SDK-XAML/Resources");
           return rl.GetString(key);
        }

    }
}
