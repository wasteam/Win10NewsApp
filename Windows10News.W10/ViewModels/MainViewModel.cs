using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Windows.Input;
using AppStudio.Uwp;
using AppStudio.Uwp.Actions;
using AppStudio.Uwp.Navigation;
using AppStudio.Uwp.Commands;
using AppStudio.DataProviders;

using AppStudio.DataProviders.Rss;
using AppStudio.DataProviders.Twitter;
using AppStudio.DataProviders.Instagram;
using AppStudio.DataProviders.LocalStorage;
using Windows10News.Sections;


namespace Windows10News.ViewModels
{
    public class MainViewModel : ObservableBase
    {
        public MainViewModel(int visibleItems) : base()
        {
            PageTitle = "Windows 10 News";
            WhatsGoingOn = ListViewModel.CreateNew(Singleton<WhatsGoingOnConfig>.Instance, visibleItems);
            RecentNews = ListViewModel.CreateNew(Singleton<RecentNewsConfig>.Instance, visibleItems);
            Apps = ListViewModel.CreateNew(Singleton<AppsConfig>.Instance, visibleItems);
            Business = ListViewModel.CreateNew(Singleton<BusinessConfig>.Instance, visibleItems);
            InsiderProgram = ListViewModel.CreateNew(Singleton<InsiderProgramConfig>.Instance, visibleItems);
            Devs = ListViewModel.CreateNew(Singleton<DevsConfig>.Instance, visibleItems);
            WhatArePeopleTalkingAbout = ListViewModel.CreateNew(Singleton<WhatArePeopleTalkingAboutConfig>.Instance, visibleItems);
            DoMore = ListViewModel.CreateNew(Singleton<DoMoreConfig>.Instance, visibleItems);

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
			Actions.Add(new ActionInfo
            {
                Command = new RelayCommand(() => NavigationService.NavigateTo(new Uri("http://appstudio.windows.com/home/appprivacyterms", UriKind.Absolute))),
                Style = ActionKnownStyles.Link,
                Name = "PrivacyButton",
                ActionType = ActionType.Secondary
            });
        }

        public string PageTitle { get; set; }
        public ListViewModel WhatsGoingOn { get; private set; }
        public ListViewModel RecentNews { get; private set; }
        public ListViewModel Apps { get; private set; }
        public ListViewModel Business { get; private set; }
        public ListViewModel InsiderProgram { get; private set; }
        public ListViewModel Devs { get; private set; }
        public ListViewModel WhatArePeopleTalkingAbout { get; private set; }
        public ListViewModel DoMore { get; private set; }

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
		public ICommand SearchCommand
        {
            get
            {
                return new RelayCommand<string>(
                (text) =>
                {
                    NavigationService.NavigateToPage("SearchPage", text);
                }, SearchViewModel.CanSearch);
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

        private async void Refresh()
        {
            var refreshDataTasks = GetViewModels()
                                        .Where(vm => !vm.HasLocalData).Select(vm => vm.LoadDataAsync(true));

            await Task.WhenAll(refreshDataTasks);

            OnPropertyChanged("LastUpdated");
        }

        private IEnumerable<ListViewModel> GetViewModels()
        {
            yield return WhatsGoingOn;
            yield return RecentNews;
            yield return Apps;
            yield return Business;
            yield return InsiderProgram;
            yield return Devs;
            yield return WhatArePeopleTalkingAbout;
            yield return DoMore;
        }
    }
}
