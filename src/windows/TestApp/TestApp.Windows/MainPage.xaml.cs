using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace TestApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void button_Click(object sender, RoutedEventArgs e) {
            Social.SocialVk vk = new Social.SocialVk();
            vk.callback += Vk_callback;
            vk.init("5027289", 1);
            //vk.login("[\"wall\", \"offline\", \"friends\", \"audio\", \"video\", \"photos\"]", 1);
            vk.test1("", 1);
        }

        private void Vk_callback(object sender, Social.EventArgs e) {
            Debug.WriteLine("Callback: " + e.result + ", " + e.error);
        }
    }
}
