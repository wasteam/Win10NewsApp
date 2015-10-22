using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using AppStudio.Uwp.Actions;
using AppStudio.Uwp.Cache;
using AppStudio.Uwp.Commands;
using AppStudio.Uwp.DataSync;
using AppStudio.Uwp.Navigation;
using AppStudio.DataProviders;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Xaml.Controls;
using Windows10News.Config;

namespace Windows10News.ViewModels
{
    public class ListViewModel<TConfig, TSchema> : DataViewModelBase<TConfig, TSchema>, INavigable where TSchema : SchemaBase
    {
        private int _visibleItems;
        private SectionConfigBase<TConfig, TSchema> _sectionConfig;
        private ObservableCollection<ItemViewModel> _items;
        private bool _hasMoreItems;

        public ListViewModel(SectionConfigBase<TConfig, TSchema> sectionConfig, int visibleItems = 0)
            : base(sectionConfig)
        {
            _visibleItems = visibleItems;
            _items = new ObservableCollection<ItemViewModel>();

            _sectionConfig = sectionConfig;

            Title = sectionConfig.ListPage.Title;
            NavigationInfo = _sectionConfig.ListNavigationInfo;
            PageTitle = _sectionConfig.PageTitle;

            if (!DataProvider.IsLocal)
            {
                Actions.Add(new ActionInfo
                {
                    Command = Refresh,
                    Style = ActionKnownStyles.Refresh,
                    Name = "RefreshButton",
                    ActionType = ActionType.Primary
                });
            }
        }

        public RelayCommand<ItemViewModel> ItemClickCommand
        {
            get
            {
                return new RelayCommand<ItemViewModel>(
                (item) =>
                {
                    NavigationService.NavigateTo(item);
                });
            }
        }

        public RelayCommand<INavigable> SectionHeaderClickCommand
        {
            get
            {
                return new RelayCommand<INavigable>(
                (item) =>
                {
                    NavigationService.NavigateTo(item);
                });
            }
        }

        public NavigationInfo NavigationInfo { get; set; }

        public string PageTitle { get; set; }

        public ObservableCollection<ItemViewModel> Items
        {
            get { return _items; }
            private set { SetProperty(ref _items, value); }
        }

        public bool HasMoreItems
        {
            get { return _hasMoreItems; }
            private set { SetProperty(ref _hasMoreItems, value); }
        }


        public ICommand Refresh
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    await LoadDataAsync(true);
                });
            }
        }

        public void ShareContent(DataRequest dataRequest, bool supportsHtml = true)
        {
            if (Items != null && Items.Count > 0)
            {
                ShareContent(dataRequest, Items[0], supportsHtml);
            }
        }

        public override void UpdateCommonProperties(SplitViewDisplayMode splitViewDisplayMode)
        {
            base.UpdateCommonProperties(splitViewDisplayMode);
            if (splitViewDisplayMode == SplitViewDisplayMode.Overlay)
            {
                AppBarRow = 5;
                AppBarColumn = 0;
                AppBarColumnSpan = 2;
            }
        }

        protected override void ParseItems(CachedContent<TSchema> content, ItemViewModel selectedItem)
        {
            var parsedItems = new List<ItemViewModel>();
            IEnumerable<TSchema> sourceVisibleItems = null;
            if (_visibleItems == 0)
            {
                sourceVisibleItems = content.Items;
            }
            else
            {
                sourceVisibleItems = content.Items.Take(_visibleItems);
            }
            foreach (var item in sourceVisibleItems)
            {
                var parsedItem = new ItemViewModel
                {
                    Id = item._id,
                    NavigationInfo = _sectionConfig.ListPage.NavigationInfo(item)
                };
                _sectionConfig.ListPage.LayoutBindings(parsedItem, item);
                parsedItems.Add(parsedItem);
            }

            Items.Sync(parsedItems);
            HasMoreItems = content.Items.Count() > Items.Count;
        }
    }
}
