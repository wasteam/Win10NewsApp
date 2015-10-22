using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Windows10News.Navigation
{
    public class NavigationTemplateSelector : DataTemplateSelector
    {
        public DataTemplate ItemTemplate { get; set; }
        public DataTemplate GroupTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            var navNode = item as NavigationNode;
            if (navNode.IsContainer)
            {
                return GroupTemplate;
            }
            else if (!navNode.IsContainer)
            {
                return ItemTemplate;
            }

            return base.SelectTemplateCore(item);
        }
    }
}
