using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Windows10News.Layouts.Controls
{
    public sealed partial class SectionListItemHeader : UserControl
    {
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(SectionListItemHeader), new PropertyMetadata(string.Empty));

        public SectionListItemHeader()
        {
            this.InitializeComponent();
        }

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }
    }
}
