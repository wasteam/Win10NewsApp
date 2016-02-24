using AppStudio.DataProviders;
using AppStudio.Uwp.Cache;
using AppStudio.Uwp.Commands;
using AppStudio.Uwp.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using AppStudio.Uwp.DataSync;
using System.Diagnostics;
using AppStudio.Uwp.Actions;
using Windows10News.Config;

namespace Windows10News.ViewModels
{
    public class GroupedListViewModel : PageViewModelBase
    {
        private List<ItemViewModel> _items = new List<ItemViewModel>();
        private OrderType _orderType;

        private Func<bool, Task<DateTime?>> LoadDataInternal;

        public string PageTitle { get; set; }

        private ObservableCollection<GroupedItemViewModel> _groupedItems = new ObservableCollection<GroupedItemViewModel>();
        public ObservableCollection<GroupedItemViewModel> Items
        {
            get { return _groupedItems; }
            private set { SetProperty(ref _groupedItems, value); }
        }

        private bool _hasItems;
        public bool HasItems
        {
            get { return _hasItems; }
            private set { SetProperty(ref _hasItems, value); }
        }

        private GroupedListViewModel()
        {
        }

        public static GroupedListViewModel CreateNew<TSchema>(SectionConfigBase<TSchema> sectionConfig) where TSchema : SchemaBase
        {
            var vm = new GroupedListViewModel
            {
                SectionName = sectionConfig.Name,
                Title = sectionConfig.ListPage.Title,
                PageTitle = sectionConfig.ListPage.PageTitle,
                HasLocalData = !sectionConfig.NeedsNetwork,
                _orderType = sectionConfig.ListPage.OrderType
            };

            var settings = new CacheSettings
            {
                Key = sectionConfig.Name,
                Expiration = vm.CacheExpiration,
                NeedsNetwork = sectionConfig.NeedsNetwork,
                UseStorage = sectionConfig.NeedsNetwork,
            };
            //we save a reference to the load delegate in order to avoid export TSchema outside the view model
            vm.LoadDataInternal = (refresh) => AppCache.LoadItemsAsync<TSchema>(settings, sectionConfig.LoadDataAsyncFunc, (content) => vm.ParseItems(sectionConfig.ListPage, content), refresh);

            if (sectionConfig.NeedsNetwork)
            {
                vm.Actions.Add(new ActionInfo
                {
                    Command = vm.Refresh,
                    Style = ActionKnownStyles.Refresh,
                    Name = "RefreshButton",
                    ActionType = ActionType.Primary
                });
            }

            return vm;
        }

        public async Task LoadDataAsync(bool forceRefresh = false)
        {
            try
            {
                HasLoadDataErrors = false;
                IsBusy = true;

                _items.Clear();

                LastUpdated = await LoadDataInternal(forceRefresh);

                if (HasItems)
                {
                    GroupItems();
                }

            }
            catch (Exception ex)
            {
                HasLoadDataErrors = true;
                Debug.WriteLine(ex.ToString());
            }
            finally
            {
                IsBusy = false;
            }
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

        private void ParseItems<TSchema>(ListPageConfig<TSchema> listConfig, CachedContent<TSchema> content) where TSchema : SchemaBase
        {
            foreach (var item in content.Items)
            {
                var parsedItem = new ItemViewModel
                {
                    Id = item._id,
                    NavigationInfo = listConfig.DetailNavigation(item)
                };
                listConfig.LayoutBindings(parsedItem, item);
                _items.Add(parsedItem);
            }

            HasItems = _items.Count > 0;
        }

        private void GroupItems()
        {
            var gItems = _items
                            .GroupBy(i => i.GroupBy)
                            .OrderBy(gi => gi.Key)
                            .Select(gi => new GroupedItemViewModel
                            {
                                Header = GetHeader(gi.Key, _items),
                                Items = new ObservableCollection<ItemViewModel>(gi.OrderBy(i => i.OrderBy, _orderType))
                            })
                            .ToList();

            _groupedItems.Sync(gItems);
        }

        private static string GetHeader(object key, IEnumerable<ItemViewModel> items)
        {
            var currentItem = items.FirstOrDefault(i => i.GroupBy == key);
            if (currentItem != null)
            {
                return currentItem.Header;
            }
            else
            {
                return string.Empty;
            }
        }
    }
}