using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace LearnAppCenter.Uwp
{
    public sealed partial class MainPage : Page
    {
        static string AppCenterSecret
        {
            get => ApplicationData.Current.LocalSettings.Values["AppCenterSecret"] as string;
            set => ApplicationData.Current.LocalSettings.Values["AppCenterSecret"] = value;
        }

        static string UserId
        {
            get => ApplicationData.Current.LocalSettings.Values["UserId"] as string;
            set => ApplicationData.Current.LocalSettings.Values["UserId"] = value;
        }

        string ErrorPropertyValue
        {
            get => ApplicationData.Current.LocalSettings.Values["ErrorPropertyValue"] as string;
            set => ApplicationData.Current.LocalSettings.Values["ErrorPropertyValue"] = value;
        }

        public MainPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            RefleshControls();
        }

        void UpdateConfigurationButton_Click(object sender, RoutedEventArgs e)
        {
            AppCenterSecret = AppCenterSecretTextBox.Text;
            
            UserId = UserIdTextBox.Text;

            ConfigurationFlyout.Hide();

            RefleshControls();
        }

        void RefleshControls()
        {
            var appCenterSecret = AppCenterSecret ?? string.Empty;
            var userId = UserId ?? string.Empty;

            AppCenterSecretText.Text = appCenterSecret;
            AppCenterSecretTextBox.Text = appCenterSecret;

            UserIdText.Text = userId;
            UserIdTextBox.Text = userId;
            
            AppCenterStartButton.IsEnabled = !string.IsNullOrWhiteSpace(appCenterSecret);
        }

        async void AppCenterStartButton_Click(object sender, RoutedEventArgs e)
        {
            var appCenterSecret = AppCenterSecret;
            var userId = UserId;

            ShowConfigurationFlyoutButton.IsEnabled = false;

            AppCenterStartButton.IsEnabled = false;

            AppCenter.Start(appCenterSecret, typeof(Analytics), typeof(Crashes));

            if (!string.IsNullOrWhiteSpace(userId))
            {
                AppCenter.SetUserId(UserId);
            }

            if (await Crashes.HasCrashedInLastSessionAsync())
            {
                HasCrashedInLastSessionIndicator.Visibility = Visibility.Visible;
            }
        }

        void GanerateTestCrushButton_Click(object sender, RoutedEventArgs e)
        {
            Crashes.GenerateTestCrash();
        }

        void ThrowNewTestCrashExceptionButton_Click(object sender, RoutedEventArgs e)
        {
            throw new TestCrashException();
        }

        void TrackErrorButton_Click(object sender, RoutedEventArgs e)
        {
            var errorPropertyValue = ErrorPropertyValue;

            if (string.IsNullOrWhiteSpace(errorPropertyValue))
            {
                Crashes.TrackError(new TestCrashException());
            }
            else
            {
                var props = new Dictionary<string, string>
                {
                    { "Prop1", ErrorPropertyValue }
                };

                Crashes.TrackError(new TestCrashException(), props);
            }
        }
    }
}
