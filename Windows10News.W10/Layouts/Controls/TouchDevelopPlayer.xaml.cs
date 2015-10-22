using System;
using AppStudio.Uwp.Navigation;
using Windows.Graphics.Display;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Windows10News.Layouts.Controls
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TouchDevelopPlayer : Page
    {
        private DisplayOrientations currentDisplayOrientations;
        public TouchDevelopPlayer()
        {
            this.InitializeComponent();
            this.webView.Navigate(new Uri("about:blank"));
            this.Loaded += (s, a) =>
            {
                this.webView.ScriptNotify += (sender, args) =>
                {
                    var v = args.Value;
                    if (v == "exit")
                    {
                        NavigationService.GoBack();
                    }
                };
            };
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.currentDisplayOrientations = DisplayInformation.AutoRotationPreferences;
            DisplayInformation.AutoRotationPreferences = DisplayOrientations.Portrait;
            string id = e.Parameter as string;
            this.OpenScript(id);

            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.webView.Navigate(new Uri("about:blank"));
            DisplayInformation.AutoRotationPreferences = currentDisplayOrientations;
            base.OnNavigatedFrom(e);
        }

        private void OpenScript(string id)
        {
            this.webView.Navigate(new Uri(string.Format("ms-appx-web:///Assets/TouchDevelop/{0}/index.html?ignoreAgent", id)));
        }
    }
}
