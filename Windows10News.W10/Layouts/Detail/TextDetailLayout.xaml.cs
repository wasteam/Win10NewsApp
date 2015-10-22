using AppStudio.Uwp.Controls;
using Windows.UI.Xaml.Controls;

namespace Windows10News.Layouts.Detail
{
    public sealed partial class TextDetailLayout : BaseDetailLayout
    {
        public TextDetailLayout()
        {
            InitializeComponent();
        }

        public override async void UpdateFontSize()
        {
			await readingWebView?.TryApplyFontSizes(BodyFontSize);
        }

        private async void readingWebView_ReadingWebViewNavigationCompleted(object sender, ReadingWebViewNavigationCompletedEventArgs args)
        {
            var readingWebView = sender as ReadingWebView;
            await readingWebView?.TryApplyFontSizes(BodyFontSize);
        }

        private void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateFontSize();
        }
    }
}
