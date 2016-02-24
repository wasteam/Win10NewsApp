using System;
using System.Collections.Generic;
using AppStudio.Uwp;
using AppStudio.Uwp.Commands;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows10News.Sections;
namespace Windows10News.ViewModels
{
    public class SearchViewModel : ObservableBase
    {
        public SearchViewModel() : base()
        {
			PageTitle = "Windows 10 News";
            WhatsGoingOn = ListViewModel.CreateNew(Singleton<WhatsGoingOnConfig>.Instance);
            RecentNews = ListViewModel.CreateNew(Singleton<RecentNewsConfig>.Instance);
            Apps = ListViewModel.CreateNew(Singleton<AppsConfig>.Instance);
            Business = ListViewModel.CreateNew(Singleton<BusinessConfig>.Instance);
            InsiderProgram = ListViewModel.CreateNew(Singleton<InsiderProgramConfig>.Instance);
            Devs = ListViewModel.CreateNew(Singleton<DevsConfig>.Instance);
            WhatArePeopleTalkingAbout = ListViewModel.CreateNew(Singleton<WhatArePeopleTalkingAboutConfig>.Instance);
            DoMore = ListViewModel.CreateNew(Singleton<DoMoreConfig>.Instance);
                        
        }

        private string _searchText;
        private bool _hasItems = true;

        public string SearchText
        {
            get { return _searchText; }
            set { SetProperty(ref _searchText, value); }
        }

        public bool HasItems
        {
            get { return _hasItems; }
            set { SetProperty(ref _hasItems, value); }
        }

		public ICommand SearchCommand
        {
            get
            {
                return new RelayCommand<string>(
                async (text) =>
                {
                    await SearchDataAsync(text);
                }, SearchViewModel.CanSearch);
            }
        }      
        public ListViewModel WhatsGoingOn { get; private set; }
        public ListViewModel RecentNews { get; private set; }
        public ListViewModel Apps { get; private set; }
        public ListViewModel Business { get; private set; }
        public ListViewModel InsiderProgram { get; private set; }
        public ListViewModel Devs { get; private set; }
        public ListViewModel WhatArePeopleTalkingAbout { get; private set; }
        public ListViewModel DoMore { get; private set; }
        
		public string PageTitle { get; set; }
        public async Task SearchDataAsync(string text)
        {
            this.HasItems = true;
            SearchText = text;
            var loadDataTasks = GetViewModels()
                                    .Select(vm => vm.SearchDataAsync(text));

            await Task.WhenAll(loadDataTasks);
			this.HasItems = GetViewModels().Any(vm => vm.HasItems);
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
		private void CleanItems()
        {
            foreach (var vm in GetViewModels())
            {
                vm.CleanItems();
            }
        }
		public static bool CanSearch(string text) { return !string.IsNullOrWhiteSpace(text) && text.Length >= 3; }
    }
}
