using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Diagnostics;
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
    public class ListViewModel : PageViewModelBase, INavigable
    {
        private ObservableCollection<ItemViewModel> _items = new ObservableCollection<ItemViewModel>();
        private bool _hasMoreItems;
		private bool _hasItems;
        private int _visibleItems;

        private Func<bool, Func<ItemViewModel, bool>, Task<DateTime?>> LoadDataInternal;

        private ListViewModel()
        {
        }

        public static ListViewModel CreateNew<TSchema>(SectionConfigBase<TSchema> sectionConfig, int visibleItems = 0) where TSchema : SchemaBase
        {
            var vm = new ListViewModel
            {
				SectionName = sectionConfig.Name,
                Title = sectionConfig.ListPage.Title,
                NavigationInfo = sectionConfig.ListPage.ListNavigationInfo,
                PageTitle = sectionConfig.ListPage.PageTitle,
                _visibleItems = visibleItems,
                HasLocalData = !sectionConfig.NeedsNetwork
            };

            var settings = new CacheSettings
            {
                Key = sectionConfig.Name,
                Expiration = vm.CacheExpiration,
                NeedsNetwork = sectionConfig.NeedsNetwork,
                UseStorage = sectionConfig.NeedsNetwork,
            };
            //we save a reference to the load delegate in order to avoid export TSchema outside the view model
            vm.LoadDataInternal = (refresh, filterFunc) => AppCache.LoadItemsAsync<TSchema>(settings, sectionConfig.LoadDataAsyncFunc, (content) => vm.ParseItems(sectionConfig.ListPage, content, filterFunc), refresh);

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

                LastUpdated = await LoadDataInternal(forceRefresh, null);
            }
            catch (Exception ex)
            {
                Microsoft.ApplicationInsights.TelemetryClient telemetry = new Microsoft.ApplicationInsights.TelemetryClient();
                telemetry.TrackException(ex);
                HasLoadDataErrors = true;
                Debug.WriteLine(ex.ToString());
            }
            finally
            {
                IsBusy = false;
            }
        }

		public async Task SearchDataAsync(string searchTerm)
        {
            if (!string.IsNullOrEmpty(searchTerm))
            {
                try
                {
                    HasLoadDataErrors = false;
                    IsBusy = true;
                    LastUpdated = await LoadDataInternal(true, i => i.ContainsString(searchTerm));
                }
                catch (Exception ex)
                {
					Microsoft.ApplicationInsights.TelemetryClient telemetry = new Microsoft.ApplicationInsights.TelemetryClient();
					telemetry.TrackException(ex);
                    HasLoadDataErrors = true;
                    Debug.WriteLine(ex.ToString());
                }
                finally
                {
                    IsBusy = false;
                }
            }
        }

		public async Task FilterDataAsync(List<string> itemsId)
        {
            if (itemsId != null && itemsId.Any())
            {
                try
                {
                    HasLoadDataErrors = false;
                    IsBusy = true;
                    LastUpdated = await LoadDataInternal(true, i => itemsId.Contains(i.Id));
                }
                catch (Exception ex)
                {
					Microsoft.ApplicationInsights.TelemetryClient telemetry = new Microsoft.ApplicationInsights.TelemetryClient();
					telemetry.TrackException(ex);
                    HasLoadDataErrors = true;
                    Debug.WriteLine(ex.ToString());
                }
                finally
                {
                    IsBusy = false;
                }
            }
        }

		internal void CleanItems()
        {
            this.Items.Clear();
            this.HasItems = false;
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

		public bool HasItems
        {
            get { return _hasItems; }
            private set { SetProperty(ref _hasItems, value); }
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

        private void ParseItems<TSchema>(ListPageConfig<TSchema> listConfig, CachedContent<TSchema> content, Func<ItemViewModel, bool> filterFunc) where TSchema : SchemaBase
        {
            var parsedItems = new List<ItemViewModel>();
            foreach (var item in GetVisibleItems(content, _visibleItems))
            {
                var parsedItem = new ItemViewModel
                {
                    Id = item._id,
                    NavigationInfo = listConfig.DetailNavigation(item)
                };
                listConfig.LayoutBindings(parsedItem, item);
                if (filterFunc == null)
                {
                    parsedItems.Add(parsedItem);
                }
                else if (filterFunc(parsedItem))
                {
                    parsedItems.Add(parsedItem);
                }
            }
            Items.Sync(parsedItems);
            HasItems = Items.Count > 0;
            HasMoreItems = content.Items.Count() > Items.Count;
        }

        private IEnumerable<TSchema> GetVisibleItems<TSchema>(CachedContent<TSchema> content, int visibleItems) where TSchema : SchemaBase
        {
            if (visibleItems == 0)
            {
                return content.Items;
            }
            else
            {
                return content.Items
                                .Take(visibleItems);
            }
        }
    }
}
