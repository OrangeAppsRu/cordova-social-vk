using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using VK.WindowsPhone.SDK.API;
using System.Windows.Media.Imaging;
using System.Windows.Input;

namespace VK.WindowsPhone.SDK.Pages
{
    public partial class VKCaptchaRequestControl : UserControl
    {
       private VKCaptchaUserRequest _captchaUserRequest;
        private Action<VKCaptchaUserResponse> _callback;


        public VKCaptchaRequestControl()
        {
            InitializeComponent();
            Visibility = Visibility.Collapsed;
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            ValidateCaptcha();
        }

        private void ValidateCaptcha()
        {
            _callback.Invoke(new VKCaptchaUserResponse()
            {
                EnteredString = textBoxCaptcha.Text,
                IsCancelled = false,
                Request = _captchaUserRequest
            });
            Visibility = Visibility.Collapsed;
        }

        public void ShowCaptchaRequest(VKCaptchaUserRequest captchaUserRequest, Action<VKCaptchaUserResponse> callback)
        {
            Visibility = Visibility.Visible;
            textBoxCaptcha.Text = string.Empty;
            imageCaptcha.Source = new BitmapImage(new Uri(captchaUserRequest.Url));
            _captchaUserRequest = captchaUserRequest;
            _callback = callback;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            _callback.Invoke(new VKCaptchaUserResponse()
                                 {
                                     IsCancelled =  true,
                                     Request = _captchaUserRequest
                                 });

            Visibility = Visibility.Collapsed;
        }

        private void textBoxCaptcha_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                ValidateCaptcha();
        }
    }
}
