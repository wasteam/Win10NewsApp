


using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using AppStudio.DataProviders;
using AppStudio.DataProviders.Core;
using AppStudio.DataProviders.Twitter;
using AppStudio.Uwp.Actions;
using AppStudio.Uwp.Commands;
using AppStudio.Uwp.Navigation;
using AppStudio.Uwp;
using System.Linq;
using Windows10News.Config;
using Windows10News.ViewModels;

namespace Windows10News.Sections
{
    public class WhatArePeopleTalkingAboutConfig : SectionConfigBase<TwitterSchema>
    {
		private readonly TwitterDataProvider _dataProvider = new TwitterDataProvider(new TwitterOAuthTokens
        {
			ConsumerKey = "hIPQRTmDy99YUaPXHJp3LgxNt",
                    ConsumerSecret = "hG4vPA0D36RSr1cN5NoBibkkkGXTD9BqEZjhCuPrwh2QwKO8hR",
                    AccessToken = "3223747883-vCjyCYxwQ1vrU9D6II4gdyGoQ1ZaYmQpAcF3lkI",
                    AccessTokenSecret = "S4L19c8QrFyrlmRJWM2P352JoyhW4mbsClW2LcVIVwAFd"
        });

		public override Func<Task<IEnumerable<TwitterSchema>>> LoadDataAsyncFunc
        {
            get
            {
                var config = new TwitterDataConfig
                {
                    QueryType = TwitterQueryType.Search,
                    Query = @"Windows10"
                };

                return () => _dataProvider.LoadDataAsync(config, MaxRecords);
            }
        }

        public override ListPageConfig<TwitterSchema> ListPage
        {
            get 
            {
                return new ListPageConfig<TwitterSchema>
                {
                    Title = "What are people talking about",

					PageTitle = "What are people talking about",

                    ListNavigationInfo = NavigationInfo.FromPage("WhatArePeopleTalkingAboutListPage"),

                    LayoutBindings = (viewModel, item) =>
                    {
                        viewModel.Title = item.UserName.ToSafeString();
                        viewModel.SubTitle = item.Text.ToSafeString();
                        viewModel.ImageUrl = ItemViewModel.LoadSafeUrl(item.UserProfileImageUrl.ToSafeString());
                    },
                    DetailNavigation = (item) =>
                    {
                        return new NavigationInfo
                        {
                            NavigationType = NavigationType.DeepLink,
                            TargetUri = new Uri(item.Url)
                        };
                    }
                };
            }
        }

        public override DetailPageConfig<TwitterSchema> DetailPage
        {
            get { return null; }
        }
    }
}
