using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppStudio.Uwp;
using AppStudio.Uwp.Actions;
using AppStudio.Uwp.Commands;
using AppStudio.Uwp.Navigation;
using AppStudio.DataProviders;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using AppStudio.DataProviders.Rss;
using AppStudio.DataProviders.Twitter;
using AppStudio.DataProviders.Instagram;
using AppStudio.DataProviders.LocalStorage;
using Windows10News.Sections;


namespace Windows10News.ViewModels
{
    public class MainViewModel : PageViewModel
    {
        public MainViewModel(int visibleItems) : base()
        {
            PageTitle = "Windows 10 News";
            WhatsGoingOn = new ListViewModel<RssDataConfig, RssSchema>(new WhatsGoingOnConfig(), visibleItems);
            RecentNews = new ListViewModel<RssDataConfig, RssSchema>(new RecentNewsConfig(), visibleItems);
            Apps = new ListViewModel<RssDataConfig, RssSchema>(new AppsConfig(), visibleItems);
            InsiderProgram = new ListViewModel<RssDataConfig, RssSchema>(new InsiderProgramConfig(), visibleItems);
            WhatArePeopleTalkingAbout = new ListViewModel<TwitterDataConfig, TwitterSchema>(new WhatArePeopleTalkingAboutConfig(), visibleItems);
            DoMore = new ListViewModel<InstagramDataConfig, InstagramSchema>(new DoMoreConfig(), visibleItems);
            Actions = new List<ActionInfo>();

            if (GetViewModels().Any(vm => !vm.HasLocalData))
            {
                Actions.Add(new ActionInfo
                {
                    Command = new RelayCommand(Refresh),
                    Style = ActionKnownStyles.Refresh,
                    Name = "RefreshButton",
                    ActionType = ActionType.Primary
                });
            }
        }

        public string PageTitle { get; set; }
        public ListViewModel<RssDataConfig, RssSchema> WhatsGoingOn { get; private set; }
        public ListViewModel<RssDataConfig, RssSchema> RecentNews { get; private set; }
        public ListViewModel<RssDataConfig, RssSchema> Apps { get; private set; }
        public ListViewModel<RssDataConfig, RssSchema> InsiderProgram { get; private set; }
        public ListViewModel<TwitterDataConfig, TwitterSchema> WhatArePeopleTalkingAbout { get; private set; }
        public ListViewModel<InstagramDataConfig, InstagramSchema> DoMore { get; private set; }

        public RelayCommand<INavigable> SectionHeaderClickCommand
        {
            get
            {
                return new RelayCommand<INavigable>(item =>
                    {
                        NavigationService.NavigateTo(item);
                    });
            }
        }

        public DateTime? LastUpdated
        {
            get
            {
                return GetViewModels().Select(vm => vm.LastUpdated)
                            .OrderByDescending(d => d).FirstOrDefault();
            }
        }

        public List<ActionInfo> Actions { get; private set; }

        public bool HasActions
        {
            get
            {
                return Actions != null && Actions.Count > 0;
            }
        }

        public async Task LoadDataAsync()
        {
            var loadDataTasks = GetViewModels().Select(vm => vm.LoadDataAsync());

            await Task.WhenAll(loadDataTasks);

            OnPropertyChanged("LastUpdated");
        }

		public override void UpdateCommonProperties(SplitViewDisplayMode splitViewDisplayMode)
        {
            base.UpdateCommonProperties(splitViewDisplayMode);
            if (splitViewDisplayMode == SplitViewDisplayMode.Overlay)
            {
                AppBarRow = 3;
                AppBarColumn = 0;
                AppBarColumnSpan = 2;
            }
        }

        private async void Refresh()
        {
            var refreshDataTasks = GetViewModels()
                                        .Where(vm => !vm.HasLocalData)
                                        .Select(vm => vm.LoadDataAsync(true));

            await Task.WhenAll(refreshDataTasks);

            OnPropertyChanged("LastUpdated");
        }

        private IEnumerable<DataViewModelBase> GetViewModels()
        {
            yield return WhatsGoingOn;
            yield return RecentNews;
            yield return Apps;
            yield return InsiderProgram;
            yield return WhatArePeopleTalkingAbout;
            yield return DoMore;
        }
    }
}
