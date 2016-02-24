using System;
using System.Windows.Input;
using AppStudio.Uwp;
using AppStudio.Uwp.Commands;
using AppStudio.Uwp.Navigation;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows10News.Navigation;
using Windows10News.Services;

namespace Windows10News.ViewModels
{
    public class ShellViewModel : ObservableBase
    {
        private AppNavigation _navigation;
        private bool _navPanelOpened;
        private bool _checkSizeChanged = true;
        private SplitViewDisplayMode _splitViewDisplayMode;
        private bool _isFullScreen;        

        public ShellViewModel()
        {
            Navigation = new AppNavigation();
            Navigation.LoadNavigation();
            if (Window.Current.Bounds.Width < 800)
            {
                this.SplitViewDisplayMode = SplitViewDisplayMode.Overlay;
            }
            else
            {
                this.SplitViewDisplayMode = SplitViewDisplayMode.CompactOverlay;
            }
            Window.Current.SizeChanged += ((args, e) => { WindowSizeChanged(e.Size.Width); });
            NavigationService.NavigatedToPage += NavigationService_NavigatedToPage;
            FullScreenService.FullScreenModeChanged += FullScreenModeChanged;
            SystemNavigationManager.GetForCurrentView().BackRequested += ((sender, e) =>
            {
                if (NavigationService.CanGoBack())
                {
                    FullScreenService.ExitFullScreenMode();
                    e.Handled = true;
                    NavigationService.GoBack();
                }
            });
        }

        public bool IsFullScreen
        {
            get { return _isFullScreen; }
            set { SetProperty(ref _isFullScreen, value); }
        }

        public AppNavigation Navigation
        {
            get { return _navigation; }
            set { SetProperty(ref _navigation, value); }
        }

        public bool NavPanelOpened
        {
            get { return _navPanelOpened; }
            set { SetProperty(ref _navPanelOpened, value); }
        }

        public SplitViewDisplayMode SplitViewDisplayMode
        {
            get { return _splitViewDisplayMode; }
            set { SetProperty(ref _splitViewDisplayMode, value); }
        }

        public ICommand ItemSelected
        {
            get
            {
                return new RelayCommand<NavigationNode>(n =>
                {
                    n.Selected();
                });
            }
        }

        public ICommand NavPanelClick
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                    () =>
                    {
                        NavPanelOpened = !NavPanelOpened;
                    });
                });
            }
        }

        public ICommand GoBackCommand
        {
            get
            {
                return NavigationService.GoBackCommand;
            }
        }

        private void WindowSizeChanged(double width)
        {
            if (!_checkSizeChanged)
            {
                return;
            }
            if (width < 800)
            {
                this.SplitViewDisplayMode = SplitViewDisplayMode.Overlay;
            }
            else
            {
                this.SplitViewDisplayMode = SplitViewDisplayMode.CompactOverlay;
            }
        }

        private void FullScreenModeChanged(object sender, bool isFullScreenModeEnabled)
        {
            IsFullScreen = isFullScreenModeEnabled;
            if (IsFullScreen)
            {
                _checkSizeChanged = false;
                this.SplitViewDisplayMode = SplitViewDisplayMode.Overlay;
            }
            else
            {
                _checkSizeChanged = true;
                WindowSizeChanged(Window.Current.Bounds.Width);
            }
        }

        private void NavigationService_NavigatedToPage(object sender, NavigatedEventArgs e)
        {
            var navigatedNode = Navigation.FindPage(e.Page);
            if (navigatedNode != null)
            {
                Navigation.Active = navigatedNode;
            }
            else
            {
                Navigation.Active = null;
            }

            if (NavPanelOpened)
            {
                NavPanelOpened = false;
            }
            if (NavigationService.CanGoBack())
            {
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            }
            else
            {
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
            }
            OnPropertyChanged("GoBackCommand");            
        }
        
    }
}
