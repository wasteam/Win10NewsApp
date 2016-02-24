using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Diagnostics;
using System.Windows.Input;
using AppStudio.DataProviders;
using AppStudio.Uwp.Actions;
using AppStudio.Uwp.Cache;
using AppStudio.Uwp.Commands;
using AppStudio.Uwp.DataSync;
using Windows.ApplicationModel.DataTransfer;
using Windows.Devices.Input;
using Windows.Graphics.Display;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using AppStudio.Uwp.Navigation;
using Windows10News.Config;
using Windows10News.Services;
using Windows.ApplicationModel.Resources;
using AppStudio.Uwp;

namespace Windows10News.ViewModels
{
    public class DetailViewModel : PageViewModelBase
    {
		private static ResourceLoader _resourceLoader;        
        private ComposedItemViewModel _selectedItem;
        private bool _isFullScreen;
        private bool _showInfo;
        private bool _showInfoLastValue;
        private bool _supportSlideShow;
        private bool _supportFullScreen;
        private bool _isRestoreScreenButtonVisible;
        private DispatcherTimer _slideShowTimer;
        private DispatcherTimer _mouseMovedTimer;
        private MouseCapabilities _mouseCapabilities;
        private ObservableCollection<ItemViewModel> _relatedItems = new ObservableCollection<ItemViewModel>();
		private string _relatedContentTitle;
        private string _relatedContentStatus;

        private Func<ItemViewModel, Task> LoadDataInternal;

        private DetailViewModel()
        {
            Items = new ObservableCollection<ComposedItemViewModel>();
            ShowInfo = true;
            IsRestoreScreenButtonVisible = false;

            ZoomMode = ZoomMode.Enabled;
            if (!Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                FullScreenService.FullScreenModeChanged += FullScreenModeChanged;
                FullScreenService.FullScreenPlayActionsChanged += FullScreenPlayActionsChanged;
                if (HasMouseConnected)
                {
                    MouseDevice.GetForCurrentView().MouseMoved += MouseMoved;
                }
            }
        }

		private static ResourceLoader ResourceLoader
        {
            get
            {
                if (_resourceLoader == null)
                {
                    _resourceLoader = new ResourceLoader();
                }
                return _resourceLoader;
            }
        }

        public static DetailViewModel CreateNew<TSchema, TRelatedSchema>(SectionConfigBase<TSchema, TRelatedSchema> sectionConfig) where TSchema : SchemaBase where TRelatedSchema : SchemaBase
        {
            var vm = new DetailViewModel
            {
                Title = sectionConfig.DetailPage.Title,
                SectionName = sectionConfig.Name
            };
            vm.RelatedContentTitle = sectionConfig.RelatedContent?.ListPage?.Title;

            var settings = new CacheSettings
            {
                Key = sectionConfig.Name,
                Expiration = vm.CacheExpiration,
                NeedsNetwork = sectionConfig.NeedsNetwork,
                UseStorage = sectionConfig.NeedsNetwork,
            };
            //we save a reference to the load delegate in order to avoid export TSchema outside the view model
            vm.LoadDataInternal = async (selectedItem) =>
            {
                TSchema sourceSelected = null;

                await AppCache.LoadItemsAsync<TSchema>(settings, sectionConfig.LoadDataAsyncFunc, (content) => vm.ParseDetailItems(sectionConfig.DetailPage, content, selectedItem, out sourceSelected));
                if (sectionConfig.RelatedContent != null)
                {
                    var settingsRelated = new CacheSettings
                    {
                        Key = $"{sectionConfig.Name}_related_{selectedItem.Id}",
                        Expiration = vm.CacheExpiration,
                        NeedsNetwork = sectionConfig.RelatedContent.NeedsNetwork
                    };

					vm.RelatedContentStatus = ResourceLoader.GetString("LoadingRelatedContent");
                    await AppCache.LoadItemsAsync<TRelatedSchema>(settingsRelated, () => sectionConfig.RelatedContent.LoadDataAsync(sourceSelected), (content) => vm.ParseRelatedItems(sectionConfig.RelatedContent.ListPage, content));
					if (vm.RelatedItems == null || vm.RelatedItems.Count == 0)
                    {
                        vm.RelatedContentStatus = ResourceLoader.GetString("ThereIsNotRelatedContent");                        
                    }
                    else
                    {
                        vm.RelatedContentStatus = string.Empty;
                    }
                }
            };

            return vm;
        }

        public static DetailViewModel CreateNew<TSchema>(SectionConfigBase<TSchema> sectionConfig) where TSchema : SchemaBase
        {
            var vm = new DetailViewModel
            {
                Title = sectionConfig.DetailPage.Title,
                SectionName = sectionConfig.Name
            };

            var settings = new CacheSettings
            {
                Key = sectionConfig.Name,
                Expiration = vm.CacheExpiration,
                NeedsNetwork = sectionConfig.NeedsNetwork,
                UseStorage = sectionConfig.NeedsNetwork,
            };
            //we save a reference to the load delegate in order to avoid export TSchema outside the view model
            vm.LoadDataInternal = async (selectedItem) =>
            {
                await AppCache.LoadItemsAsync<TSchema>(settings, sectionConfig.LoadDataAsyncFunc, (content) => vm.ParseDetailItems(sectionConfig.DetailPage, content, selectedItem));
            };

            return vm;
        }

        public async Task LoadDataAsync(ItemViewModel selectedItem)
        {
            try
            {
                HasLoadDataErrors = false;
                IsBusy = true;

                await LoadDataInternal(selectedItem);
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

        public ComposedItemViewModel SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                SetProperty(ref _selectedItem, value);
            }
        }
        private ZoomMode _zoomMode;
        public ZoomMode ZoomMode
        {
            get { return _zoomMode; }
            set { SetProperty(ref _zoomMode, value); }
        }
        public ObservableCollection<ComposedItemViewModel> Items { get; protected set; }


        public ObservableCollection<ItemViewModel> RelatedItems
        {
            get { return _relatedItems; }
            private set { SetProperty(ref _relatedItems, value); }
        }
		
        public string RelatedContentTitle
        {
            get { return _relatedContentTitle; }
            set { SetProperty(ref _relatedContentTitle, value); }
        }

		
        public string RelatedContentStatus
        {
            get { return _relatedContentStatus; }
            set { SetProperty(ref _relatedContentStatus, value); }
        }

		public RelayCommand<ItemViewModel> RelatedItemClickCommand
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

        public bool IsFullScreen
        {
            get { return _isFullScreen; }
            set { SetProperty(ref _isFullScreen, value); }
        }
        public bool ShowInfo
        {
            get { return _showInfo; }
            set { SetProperty(ref _showInfo, value); }
        }
        public bool SupportSlideShow
        {
            get { return _supportSlideShow; }
            set { SetProperty(ref _supportSlideShow, value); }
        }
        public bool SupportFullScreen
        {
            get { return _supportFullScreen; }
            set { SetProperty(ref _supportFullScreen, value); }
        }
        public bool IsRestoreScreenButtonVisible
        {
            get { return _isRestoreScreenButtonVisible; }
            set { SetProperty(ref _isRestoreScreenButtonVisible, value); }
        }

        public DispatcherTimer SlideShowTimer
        {
            get
            {
                if (_slideShowTimer == null)
                {
                    _slideShowTimer = new DispatcherTimer()
                    {
                        Interval = TimeSpan.FromMilliseconds(1500)
                    };
                    _slideShowTimer.Tick += PresentationTimeEvent;
                }
                return _slideShowTimer;
            }
        }
        public DispatcherTimer MouseMovedTimer
        {
            get
            {
                if (_mouseMovedTimer == null)
                {
                    _mouseMovedTimer = new DispatcherTimer() { Interval = TimeSpan.FromMilliseconds(3000) };
                    _mouseMovedTimer.Tick += ((e, o) =>
                    {
                        _mouseMovedTimer.Stop();
                        IsRestoreScreenButtonVisible = false;
                    });
                }
                return _mouseMovedTimer;
            }
        }

        private bool HasMouseConnected
        {
            get
            {
                if (_mouseCapabilities == null)
                {
                    _mouseCapabilities = new MouseCapabilities();
                }
                return _mouseCapabilities.MousePresent > 0;
            }
        }

        public ICommand FullScreenCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    if (SupportFullScreen || SupportSlideShow)
                    {
                        FullScreenService.EnterFullScreenMode();
                        ZoomMode = ZoomMode.Enabled;
                    }
                });
            }
        }
        public ICommand ShowPresentationCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    if (SupportFullScreen || SupportSlideShow)
                    {
                        FullScreenService.EnterFullScreenMode(true);
                        ZoomMode = ZoomMode.Disabled;
                    }
                });
            }
        }
        public ICommand ShowInfoCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    if (!IsFullScreen)
                    {
                        ShowInfo = !ShowInfo;
                    }
                });
            }
        }
        public ICommand DisableFullScreenCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    if (SupportFullScreen)
                    {
                        FullScreenService.ExitFullScreenMode();
                    }
                });
            }
        }

        public void ShareContent(DataRequest dataRequest, bool supportsHtml = true)
        {
            ShareContent(dataRequest, SelectedItem, supportsHtml);
        }

        private void ParseDetailItems<TSchema>(DetailPageConfig<TSchema> detailConfig, CachedContent<TSchema> content, ItemViewModel selectedItem) where TSchema : SchemaBase
        {
            TSchema sourceSelected;
            ParseDetailItems(detailConfig, content, selectedItem, out sourceSelected);
        }

        private void ParseDetailItems<TSchema>(DetailPageConfig<TSchema> detailConfig, CachedContent<TSchema> content, ItemViewModel selectedItem, out TSchema sourceSelected) where TSchema : SchemaBase
        {
            sourceSelected = content.Items.FirstOrDefault(i => i._id == selectedItem.Id);

            foreach (var item in content.Items)
            {
                var composedItem = new ComposedItemViewModel
                {
                    Id = item._id
                };

                foreach (var binding in detailConfig.LayoutBindings)
                {
                    var parsedItem = new ItemViewModel
                    {
                        Id = item._id
                    };
                    binding(parsedItem, item);

                    composedItem.Add(parsedItem);
                }

                composedItem.Actions = detailConfig.Actions
                                                        .Select(a => new ActionInfo
                                                        {
                                                            Command = a.Command,
                                                            CommandParameter = a.CommandParameter(item),
                                                            Style = a.Style,
                                                            Text = a.Text,
                                                            ActionType = ActionType.Primary
                                                        })
                                                        .ToList();

                Items.Add(composedItem);
            }
            if (selectedItem != null)
            {
                SelectedItem = Items.FirstOrDefault(i => i.Id == selectedItem.Id);
            }

        }

        private void ParseRelatedItems<TSchema>(ListPageConfig<TSchema> listConfig, CachedContent<TSchema> content) where TSchema : SchemaBase
        {
            var parsedItems = new List<ItemViewModel>();

            foreach (var item in content.Items)
            {
                var parsedItem = new ItemViewModel
                {
                    Id = item._id,
                    NavigationInfo = listConfig.DetailNavigation(item)
                };
                listConfig.LayoutBindings(parsedItem, item);
                parsedItems.Add(parsedItem);
            }

            RelatedItems.Sync(parsedItems);
        }

        private void FullScreenPlayActionsChanged(object sender, EventArgs e)
        {
            if (SupportSlideShow)
            {
                SlideShowTimer.Start();
            }
        }
        private void FullScreenModeChanged(object sender, bool isFullScreen)
        {
            if (SupportFullScreen)
            {
                //this.ShowInfo = !isFullScreen;
                this.IsFullScreen = isFullScreen;
                if (isFullScreen)
                {
                    this._showInfoLastValue = this.ShowInfo;
                    this.ShowInfo = false;
                    IsRestoreScreenButtonVisible = true;
                    if (HasMouseConnected)
                    {
                        MouseMovedTimer.Start();
                    }
                }
                else
                {
                    this.ShowInfo = this._showInfoLastValue;
                    IsRestoreScreenButtonVisible = false;
                }
            }
            if (SupportSlideShow)
            {
                if (!isFullScreen)
                {
                    SlideShowTimer.Stop();
                    ZoomMode = ZoomMode.Enabled;
                }
            }
        }
        private void PresentationTimeEvent(object sender, object e)
        {
            if (Items != null && Items.Count > 1 && SelectedItem != null)
            {
                var index = Items.IndexOf(SelectedItem);
                if (index < Items.Count - 1)
                {
                    index++;
                }
                else
                {
                    index = 0;
                }
                SelectedItem = Items[index];
            }
        }
        private void MouseMoved(MouseDevice sender, MouseEventArgs args)
        {
            if (IsFullScreen)
            {
                IsRestoreScreenButtonVisible = true;
                MouseMovedTimer.Start();
            }
        }
    }
}
