using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace VK.WindowsPhone.SDK.Util
{
    public class VKExecute
    {
        public static void ExecuteOnUIThread(Action action)
        {
#if SILVERLIGHT
            if (Deployment.Current.Dispatcher.CheckAccess())
            {
                action();
            }
            else
            {
                Deployment.Current.Dispatcher.BeginInvoke(action);
            }
#else
            var d = Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher;

            if (d.HasThreadAccess)
            {
                action();
            }
            else
            {
                d.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal,
                    () =>
                    {
                        action();
                    });
            }
#endif
        }
    }
}
