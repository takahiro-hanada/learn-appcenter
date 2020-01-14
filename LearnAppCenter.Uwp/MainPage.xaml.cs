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

        public MainPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            RefleshControls();
        }

        void UpdateAppCenterSecretButton_Click(object sender, RoutedEventArgs e)
        {
            AppCenterSecret = AppCenterSecretTextBox.Text;

            UpdateAppCenterSecretFlyout.Hide();

            RefleshControls();
        }

        void RefleshControls()
        {
            var appCenterSecret = AppCenterSecret ?? string.Empty;

            AppSecretSecretText.Text = appCenterSecret;

            AppCenterSecretTextBox.Text = appCenterSecret;

            AppCenterStartButton.IsEnabled = !string.IsNullOrWhiteSpace(appCenterSecret);
        }

        async void AppCenterStartButton_Click(object sender, RoutedEventArgs e)
        {
            ShowAppCenterSecretFlyoutButton.IsEnabled = false;

            AppCenterStartButton.IsEnabled = false;

            AppCenter.Start(AppCenterSecret, typeof(Analytics), typeof(Crashes));

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
    }
}
