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
        public MainPage() => InitializeComponent();

        string AppCenterSecret
        {
            get => ApplicationData.Current.LocalSettings.Values["AppCenterSecret"] as string;
            set
            {
                value = string.IsNullOrWhiteSpace(value) ? null : value;

                ApplicationData.Current.LocalSettings.Values["AppCenterSecret"] = value;
            }
        }

        string UserId
        {
            get => ApplicationData.Current.LocalSettings.Values["UserId"] as string;
            set
            {
                value = string.IsNullOrWhiteSpace(value) ? null : value;

                ApplicationData.Current.LocalSettings.Values["UserId"] = value;

                AppCenter.SetUserId(value);
            }
        }

        string EventName
        {
            get => ApplicationData.Current.LocalSettings.Values["EventName"] as string;
            set => ApplicationData.Current.LocalSettings.Values["EventName"] = value;
        }

        bool UseEventProperty
        {
            get => ApplicationData.Current.LocalSettings.Values["UseEventProperty"] as bool? ?? false;
            set => ApplicationData.Current.LocalSettings.Values["UseEventProperty"] = value;
        }

        string EventPropertyName
        {
            get => ApplicationData.Current.LocalSettings.Values["EventPropertyName"] as string;
            set => ApplicationData.Current.LocalSettings.Values["EventPropertyName"] = value;
        }

        string EventPropertyValue
        {
            get => ApplicationData.Current.LocalSettings.Values["EventPropertyValue"] as string;
            set => ApplicationData.Current.LocalSettings.Values["EventPropertyValue"] = value;
        }

        bool UseErrorProperty
        {
            get => ApplicationData.Current.LocalSettings.Values["UseErrorProperty"] as bool? ?? false;
            set => ApplicationData.Current.LocalSettings.Values["UseErrorProperty"] = value;
        }

        string ErrorPropertyName
        {
            get => ApplicationData.Current.LocalSettings.Values["ErrorPropertyName"] as string;
            set => ApplicationData.Current.LocalSettings.Values["ErrorPropertyName"] = value;
        }

        string ErrorPropertyValue
        {
            get => ApplicationData.Current.LocalSettings.Values["ErrorPropertyValue"] as string;
            set => ApplicationData.Current.LocalSettings.Values["ErrorPropertyValue"] = value;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            AppCenter.SetUserId(UserId);

            AnalyticsSwitch.IsOn = await Analytics.IsEnabledAsync();

            CrashesSwitch.IsOn = await Crashes.IsEnabledAsync();
        }

        async void StartButton_Click(object sender, RoutedEventArgs e)
        {
            AppCenterSecretTextBox.IsReadOnly = true;

            StartButton.IsEnabled = false;

            AppCenter.Start(AppCenterSecret, typeof(Analytics), typeof(Crashes));
            
            if (await Crashes.HasCrashedInLastSessionAsync())
            {
                HasCrashedInLastSessionIndicator.Visibility = Visibility.Visible;
            }
        }

        async void CrashesSwitch_Toggled(object _, RoutedEventArgs __)
        {
            await Crashes.SetEnabledAsync(CrashesSwitch.IsOn);
        }

        async void AnalyticsSwitch_Toggled(object _, RoutedEventArgs __)
        {
            await Analytics.SetEnabledAsync(AnalyticsSwitch.IsOn);
        }

        void GanerateTestCrush()
        {
            Crashes.GenerateTestCrash();
        }

        void ThrowNewTestCrashException()
        {
            throw new TestCrashException();
        }

        void TrackError()
        {
            if (UseErrorProperty)
            {
                var props = new Dictionary<string, string>
                {
                    { ErrorPropertyName, ErrorPropertyValue }
                };

                Crashes.TrackError(new TestCrashException(), props);
            }
            else
            {
                Crashes.TrackError(new TestCrashException());
            }
        }

        void TrackEvent()
        {
            if (UseEventProperty)
            {
                var props = new Dictionary<string, string>
                {
                    { EventPropertyName, EventPropertyValue }
                };

                Analytics.TrackEvent(EventName, props);
            }
            else
            {
                Analytics.TrackEvent(EventName);
            }
        }
    }
}
