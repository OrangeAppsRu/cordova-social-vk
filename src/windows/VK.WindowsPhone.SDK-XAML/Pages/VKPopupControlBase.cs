using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace VK.WindowsPhone.SDK_XAML.Pages
{
    public class VKPopupControlBase  : UserControl
    {
        private Popup _parentPopup;

        private static List<VKPopupControlBase> _currentlyShownInstances = new List<VKPopupControlBase>();

        public static List<VKPopupControlBase> CurrentlyShownInstances
        {
            get
            {
                return _currentlyShownInstances;
            }
        }


        public bool IsShown
        {
            get
            {
                return _parentPopup.IsOpen;
            }
            set
            {
                _parentPopup.IsOpen = value;

                if (!value)
                {
                    OnClosing();
                    _currentlyShownInstances.Remove(this);
                }
            }
        }



        public void ShowInPopup(double? width = null, double? height = null)
        {
            var popup = new Popup();
            if (width.HasValue)
            {
                popup.Width = width.Value;
                this.Width = width.Value;
            }
            if (height.HasValue)
            {
                popup.Height = height.Value;
                this.Height = height.Value;
            }

            this._parentPopup = popup;

            popup.Child = this;

            popup.IsOpen = true;

            _currentlyShownInstances.Add(this);

            this.PrepareForLoad();
          
        }

        protected virtual void OnClosing()
        {

        }

        protected virtual void PrepareForLoad()
        {
        }
    }
}
