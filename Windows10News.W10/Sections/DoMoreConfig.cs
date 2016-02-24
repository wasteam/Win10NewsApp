


using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using AppStudio.DataProviders;
using AppStudio.DataProviders.Core;
using AppStudio.DataProviders.Instagram;
using AppStudio.Uwp.Actions;
using AppStudio.Uwp.Commands;
using AppStudio.Uwp.Navigation;
using AppStudio.Uwp;
using System.Linq;
using Windows10News.Config;
using Windows10News.ViewModels;

namespace Windows10News.Sections
{
    public class DoMoreConfig : SectionConfigBase<InstagramSchema>
    {
		private readonly InstagramDataProvider _dataProvider = new InstagramDataProvider(new InstagramOAuthTokens
        {
			ClientId = "182eae2654c74bd1a5739def5541d524"
        });

		public override Func<Task<IEnumerable<InstagramSchema>>> LoadDataAsyncFunc
        {
            get
            {
                var config = new InstagramDataConfig
                {
                    QueryType = InstagramQueryType.Id,
                    Query = @"524549267"
                };

                return () => _dataProvider.LoadDataAsync(config, MaxRecords);
            }
        }

        public override ListPageConfig<InstagramSchema> ListPage
        {
            get 
            {
                return new ListPageConfig<InstagramSchema>
                {
                    Title = "Do more",

					PageTitle = "Do more",

                    ListNavigationInfo = NavigationInfo.FromPage("DoMoreListPage"),

                    LayoutBindings = (viewModel, item) =>
                    {
                        viewModel.Title = item.Title.ToSafeString();
                        viewModel.ImageUrl = ItemViewModel.LoadSafeUrl(item.ImageUrl.ToSafeString());
                    },
                    DetailNavigation = (item) =>
                    {
                        return NavigationInfo.FromPage("DoMoreDetailPage", true);
                    }
                };
            }
        }

        public override DetailPageConfig<InstagramSchema> DetailPage
        {
            get
            {
                var bindings = new List<Action<ItemViewModel, InstagramSchema>>();
                bindings.Add((viewModel, item) =>
                {
                    viewModel.PageTitle = item.Title.ToSafeString();
                    viewModel.Title = item.Title.ToSafeString();
                    viewModel.Description = item.Author.ToSafeString();
                    viewModel.ImageUrl = ItemViewModel.LoadSafeUrl(item.ImageUrl.ToSafeString());
                    viewModel.Content = null;
					viewModel.Source = item.SourceUrl;
                });

                var actions = new List<ActionConfig<InstagramSchema>>
                {
                    ActionConfig<InstagramSchema>.Link("Go To Source", (item) => item.SourceUrl.ToSafeString()),
                };

                return new DetailPageConfig<InstagramSchema>
                {
                    Title = "Do more",
                    LayoutBindings = bindings,
                    Actions = actions
                };
            }
        }
    }
}
