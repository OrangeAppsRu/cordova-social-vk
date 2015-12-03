    using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using VK.WindowsPhone.SDK.API;
using VK.WindowsPhone.SDK.Util;

namespace VK.WindowsPhone.SDK.Pages
{
    public partial class VKLoginPage
    {    
        private const String REDIRECT_URL = "https://oauth.vk.com/blank.html";

        private bool _isInitialized = false;

        private string _scopes;
        private bool _revoke;
        private string _urlToLoad;
        private Action<VKValidationResponse> _validationCallback;
        private bool _isValidating = false;
        private bool _processedResult = false;

        internal static PhoneApplicationFrame RootFrame
        {
            get { return (PhoneApplicationFrame)Application.Current.RootVisual; }
        }

        public VKLoginPage()
        {
            InitializeComponent();          
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (!_isInitialized)
            {
                if (NavigationContext.QueryString.ContainsKey("ValidationUri"))
                {
                    _validationCallback = VKParametersRepository.GetParameterForIdAndReset("ValidationCallback") as Action<VKValidationResponse>;

                    _urlToLoad = NavigationContext.QueryString["ValidationUri"];

                    _isValidating = true;
                }
                else
                {
                    _scopes = NavigationContext.QueryString["Scopes"];

                    _revoke = NavigationContext.QueryString["Revoke"] == Boolean.TrueString;
                }

                InitializeWebBrowser();

                _isInitialized = true;
            }
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);
            if (e.NavigationMode == NavigationMode.Back && !_processedResult)
            {
                VKSDK.ProcessLoginResult(null, _isValidating, _validationCallback);  
            }
        }

        private void InitializeWebBrowser()
        {           
            var urlToLoad = _urlToLoad ??  string.Format(VKSDK.VK_AUTH_STR_FRM,
               VKSDK.Instance.CurrentAppID,
               _scopes, 
               REDIRECT_URL,
               VKSDK.API_VERSION, 
               _revoke ? 1 : 0);

            webBrowser.NavigationFailed += BrowserOnNavigationFailed;
            webBrowser.Navigating += BrowserOnNavigating;
            webBrowser.LoadCompleted += BrowserOnLoadCompleted;

            webBrowser.Navigate(new Uri(urlToLoad));
        }

        private void BrowserOnLoadCompleted(object sender, NavigationEventArgs navigationEventArgs)
        {
            webBrowser.LoadCompleted -= BrowserOnLoadCompleted;
            webBrowser.Visibility = Visibility.Visible;
            progressBar.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void BrowserOnNavigating(object sender, NavigatingEventArgs args)
        {
            var url = args.Uri.AbsoluteUri;
            if (url.StartsWith(REDIRECT_URL) && !_processedResult)
            {
                var result  = url.Substring(url.IndexOf('#') + 1);

                _processedResult = true;
                VKSDK.ProcessLoginResult(result, _isValidating, _validationCallback);
                RootFrame.GoBack();
            }
        }
       
        private void BrowserOnNavigationFailed(object sender, NavigationFailedEventArgs navigationFailedEventArgs)
        {
            progressBar.Visibility = System.Windows.Visibility.Collapsed;
            errorTextBlock.Visibility = System.Windows.Visibility.Visible;
        }
    }
}