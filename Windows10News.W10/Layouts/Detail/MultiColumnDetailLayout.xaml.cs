using AppStudio.Uwp.Controls;
using Windows.UI.Xaml.Controls;

namespace Windows10News.Layouts.Detail
{
    public sealed partial class MultiColumnDetailLayout : BaseDetailLayout
    {
        public MultiColumnDetailLayout()
        {
            InitializeComponent();
        }
        public override async void UpdateFontSize()
        {
            if (mainPivot != null && mainPivot.SelectedIndex != -1)
            {
                var container = mainPivot.ContainerFromItem(mainPivot.Items[mainPivot.SelectedIndex]);
                if (container != null)
                {
                    var children = AllChildren(container);
                    if (children != null)
                    {
                        var readingWebView = children.Find(x => x.Name.Equals("readingWebView")) as ReadingWebView;
                        if (readingWebView != null)
                        {
                            await readingWebView?.TryApplyFontSizes(BodyFontSize);
                        }
                    }
                }
            }
        }

        private async void readingWebView_ReadingWebViewNavigationCompleted(object sender, ReadingWebViewNavigationCompletedEventArgs e)
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
