//---------------------------------------------------------------------------
//
// <copyright file="InsiderProgramListPage.xaml.cs" company="Microsoft">
//    Copyright (C) 2015 by Microsoft Corporation.  All rights reserved.
// </copyright>
//
// <createdOn>10/22/2015 10:25:19 AM</createdOn>
//
//---------------------------------------------------------------------------

using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using AppStudio.DataProviders.Rss;
using Windows10News.Sections;
using Windows10News.ViewModels;

namespace Windows10News.Views
{
    public sealed partial class InsiderProgramListPage : Page
    {
        public InsiderProgramListPage()
        {
            this.ViewModel = new ListViewModel<RssDataConfig, RssSchema>(new InsiderProgramConfig());
            this.InitializeComponent();
            new Microsoft.ApplicationInsights.TelemetryClient().TrackPageView(this.GetType().FullName);
        }

        public ListViewModel<RssDataConfig, RssSchema> ViewModel { get; set; }


        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            await this.ViewModel.LoadDataAsync();

            base.OnNavigatedTo(e);
        }

    }
}
