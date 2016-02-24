using AppStudio.Uwp.Controls;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;

namespace Windows10News.Layouts.Detail
{
    public sealed partial class RelatedContentDetailLayout : BaseDetailLayout
    {
        public static readonly DependencyProperty RelatedContentTemplateProperty =
            DependencyProperty.Register("RelatedContentTemplate", typeof(DataTemplate), typeof(BaseDetailLayout), new PropertyMetadata(null));		

        public DataTemplate RelatedContentTemplate
        {
            get { return (DataTemplate)GetValue(RelatedContentTemplateProperty); }
            set { SetValue(RelatedContentTemplateProperty, value); }
        }

        public RelatedContentDetailLayout()
        {
            InitializeComponent();
        }
    }
}
