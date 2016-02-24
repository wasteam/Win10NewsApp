using System;
using System.Collections.Generic;
using AppStudio.DataProviders;
using AppStudio.Uwp.Navigation;
using Windows10News.ViewModels;

namespace Windows10News.Config
{
    public abstract class PageConfigBase
    {
        public string Title { get; set; }
    }

    public class ListPageConfig<T> : PageConfigBase where T : SchemaBase
    {
        public string PageTitle { get; set; }
        public NavigationInfo ListNavigationInfo { get; set; }
        public Func<T, NavigationInfo> DetailNavigation { get; set; }
        public Action<ItemViewModel, T> LayoutBindings { get; set; }
        public OrderType OrderType { get; set; }
    }

    public class DetailPageConfig<T> : PageConfigBase where T : SchemaBase
    {
        public List<Action<ItemViewModel, T>> LayoutBindings { get; set; }
        public List<ActionConfig<T>> Actions { get; set; }
    }
}
