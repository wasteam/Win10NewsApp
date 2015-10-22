using System;
using System.Collections.Generic;
using AppStudio.DataProviders;
using AppStudio.DataProviders.Core;
using AppStudio.DataProviders.Twitter;
using AppStudio.Uwp.Actions;
using AppStudio.Uwp.Commands;
using AppStudio.Uwp.Navigation;
using Windows10News.Config;
using Windows10News.ViewModels;

namespace Windows10News.Sections
{
    public class WhatArePeopleTalkingAboutConfig : SectionConfigBase<TwitterDataConfig, TwitterSchema>
    {
        public override DataProviderBase<TwitterDataConfig, TwitterSchema> DataProvider
        {
            get
            {
                return new TwitterDataProvider(new TwitterOAuthTokens
                {
                    ConsumerKey = "OszocwdQB1zaFzXHlQCn4rVkZ",
                    ConsumerSecret = "tehGYqkm7390zhdtDxoyEvvsuqgC3JTCsycn6E5pkQXxgzE4Av",
                    AccessToken = "3223747883-e1DPeXqEoDm1JpkpTHHaHUPpw1jfGMw9CGOIK0F",
                    AccessTokenSecret = "gq7nf0LCqtgdXTA6by3gx7kSkfrqG3MnXYwFTHvJW16mp"
                });
            }
        }

        public override TwitterDataConfig Config
        {
            get
            {
                return new TwitterDataConfig
                {
                    QueryType = TwitterQueryType.Search,
                    Query = @"Windows10"
                };
            }
        }

        public override NavigationInfo ListNavigationInfo
        {
            get 
            {
                return NavigationInfo.FromPage("WhatArePeopleTalkingAboutListPage");
            }
        }

        public override ListPageConfig<TwitterSchema> ListPage
        {
            get 
            {
                return new ListPageConfig<TwitterSchema>
                {
                    Title = "What are people talking about",

                    LayoutBindings = (viewModel, item) =>
                    {
                        viewModel.Title = item.UserName.ToSafeString();
                        viewModel.SubTitle = item.Text.ToSafeString();
                        viewModel.Description = null;
                        viewModel.Image = item.UserProfileImageUrl.ToSafeString();
                    },
                    NavigationInfo = (item) =>
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

        public override string PageTitle
        {
            get { return "What are people talking about"; }
        }
    }
}
