using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using AppStudio.DataProviders;
using AppStudio.Uwp.Actions;
using AppStudio.Uwp.Cache;
using AppStudio.Uwp.Commands;
using Windows.ApplicationModel.DataTransfer;
using Windows.Devices.Input;
using Windows.Graphics.Display;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows10News.Config;
using Windows10News.Services;

namespace Windows10News.ViewModels
{
    public class DetailViewModel<TConfig, TSchema> : DataViewModelBase<TConfig, TSchema> where TSchema : SchemaBase
    {
        private SectionConfigBase<TConfig, TSchema> _sectionConfig;
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

        public DetailViewModel(SectionConfigBase<TConfig, TSchema> sectionConfig)
            : base(sectionConfig)
        {
            Items = new ObservableCollection<ComposedItemViewModel>();
            _sectionConfig = sectionConfig;
            ShowInfo = true;
            IsRestoreScreenButtonVisible = false;
            Title = _sectionConfig.DetailPage.Title;
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

        public ComposedItemViewModel SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                SetProperty(ref _selectedItem, value);
            }
        }
        public ZoomMode ZoomMode { get; set; }
        public ObservableCollection<ComposedItemViewModel> Items { get; protected set; }
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
        public override void UpdateCommonProperties(SplitViewDisplayMode splitViewDisplayMode)
        {
            base.UpdateCommonProperties(splitViewDisplayMode);
            if (splitViewDisplayMode == SplitViewDisplayMode.Overlay)
            {
                AppBarRow = 2;
                AppBarColumn = 0;
                AppBarColumnSpan = 2;
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
        protected override void ParseItems(CachedContent<TSchema> content, ItemViewModel selectedItem)
        {

            foreach (var item in content.Items)
            {
                var composedItem = new ComposedItemViewModel
                {
                    Id = item._id
                };

                foreach (var binding in _sectionConfig.DetailPage.LayoutBindings)
                {
                    var parsedItem = new ItemViewModel
                    {
                        Id = item._id
                    };
                    binding(parsedItem, item);

                    composedItem.Add(parsedItem);
                }

                composedItem.Actions = _sectionConfig.DetailPage.Actions
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
